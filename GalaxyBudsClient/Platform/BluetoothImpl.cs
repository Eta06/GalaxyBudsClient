using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using GalaxyBudsClient.Bluetooth;
using GalaxyBudsClient.Generated.I18N;
using GalaxyBudsClient.Message;
using GalaxyBudsClient.Message.Encoder;
using GalaxyBudsClient.Model;
using GalaxyBudsClient.Model.Config;
using GalaxyBudsClient.Model.Constants;
using GalaxyBudsClient.Model.Specifications;
using GalaxyBudsClient.Scripting;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using Task = System.Threading.Tasks.Task;

namespace GalaxyBudsClient.Platform;

public sealed class BluetoothImpl : ReactiveObject, IDisposable
{ 
    private static readonly object Padlock = new();
    private static BluetoothImpl? _instance;
    public static BluetoothImpl Instance
    {
        get
        {
            lock (Padlock)
            {
                return _instance ??= new BluetoothImpl();
            }
        }
    }

    public static void Reallocate()
    {
        Log.Debug("BluetoothImpl: Reallocating");
        _instance?.Dispose();
        _instance = null;
        _instance = new BluetoothImpl();
    }

    private readonly IBluetoothService _backend;
    
    public event EventHandler? Connected;
    public event EventHandler? Connecting;
    public event EventHandler<string>? Disconnected;
    public event EventHandler<SppMessage>? MessageReceived;
    public event EventHandler<InvalidPacketException>? InvalidDataReceived;
    public event EventHandler<byte[]>? NewDataReceived;
    public event EventHandler<BluetoothException>? BluetoothError;
    
    public Models CurrentModel => Device.Current?.Model ?? Models.NULL;
    public IDeviceSpec DeviceSpec => DeviceSpecHelper.FindByModel(CurrentModel) ?? new StubDeviceSpec();
    public static bool HasValidDevice => Settings.Data.Devices.Count > 0 && 
                                         Settings.Data.Devices.Any(x => x.Model != Models.NULL);
    
    [Reactive] public string DeviceName { private set; get; } = "Galaxy Buds";
    [Reactive] public bool IsConnected { private set; get; }
    [Reactive] public string LastErrorMessage { private set; get; } = string.Empty;
    [Reactive] public bool SuppressDisconnectionEvents { set; get; }
    [Reactive] public bool ShowDummyDevices { set; get; }

    public DeviceManager Device { get; } = new();
    
    private readonly List<byte> _incomingData = [];
    private static readonly ConcurrentQueue<byte[]> IncomingQueue = new();
    private readonly CancellationTokenSource _loopCancelSource = new();
    private CancellationTokenSource _connectCancelSource = new();
    private readonly Task? _loop;

    private BluetoothImpl()
    {
        try
        {
            // We don't want to initialize the backend in design mode. It would conflict with the actual application.
            if (!Design.IsDesignMode)
            {
#if Windows
                if (PlatformUtils.IsWindows && Settings.Data.UseBluetoothWinRt
                                            && PlatformUtils.IsWindowsContractsSdkSupported)
                {
                    Log.Debug("BluetoothImpl: Using WinRT.BluetoothService");
                    _backend = new Bluetooth.WindowsRT.BluetoothService();
                }
                else if (PlatformUtils.IsWindows)
                {
                    Log.Debug("BluetoothImpl: Using Windows.BluetoothService");
                    _backend = new Bluetooth.Windows.BluetoothService();
                }
#elif Linux
                if (PlatformUtils.IsLinux)

                {
                    Log.Debug("BluetoothImpl: Using Linux.BluetoothService");
                    _backend = new Bluetooth.Linux.BluetoothService();
                }
#elif OSX
                if (PlatformUtils.IsOSX)
                {
                    Log.Debug("BluetoothImpl: Using OSX.BluetoothService");
                    _backend = new ThePBone.OSX.Native.BluetoothService();
                }
#endif
            }
        }
        catch (PlatformNotSupportedException)
        {
            Log.Error("BluetoothImpl: Critical error while preparing bluetooth backend");
        }

        if (_backend == null)
        {
            Log.Warning("BluetoothImpl: Using Dummy.BluetoothService");
            _backend = new Dummy.BluetoothService();
        }
        
        _loop = Task.Run(DataConsumerLoop, _loopCancelSource.Token);
            
        _backend.Connecting += (_, _) => Connecting?.Invoke(this, EventArgs.Empty);
        _backend.BluetoothErrorAsync += (_, exception) => OnBluetoothError(exception); 
        _backend.NewDataAvailable += OnNewDataAvailable;
        _backend.RfcommConnected += OnRfcommConnected;
        _backend.Disconnected += OnDisconnected;
            
        MessageReceived += SppMessageReceiver.Instance.MessageReceiver;
        InvalidDataReceived += OnInvalidDataReceived;
    }

    public async void Dispose()
    {
        try
        {
            await _backend.DisconnectAsync();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "BluetoothImpl.Dispose: Error while disconnecting");
        }

        MessageReceived -= SppMessageReceiver.Instance.MessageReceiver;
            
        await _loopCancelSource.CancelAsync();
        await Task.Delay(50);

        try
        {
            _loop?.Dispose();
            _loopCancelSource.Dispose();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "BluetoothImpl.Dispose: Error while disposing children");
        }
    }
    
    private void OnInvalidDataReceived(object? sender, InvalidPacketException e)
    {
        LastErrorMessage = e.ErrorCode.ToString();
        if (IsConnected)
        {
            _ = DisconnectAsync()
                .ContinueWith(_ => Task.Delay(500))
                .ContinueWith(_ => ConnectAsync());
        }
    }
        
    private void OnBluetoothError(BluetoothException exception)
    {
        if (SuppressDisconnectionEvents) 
            return;
        
        LastErrorMessage = exception.ErrorMessage ?? exception.Message;
        IsConnected = false;
        BluetoothError?.Invoke(this, exception);
    }
        
    public async Task<IEnumerable<BluetoothDevice>> GetDevicesAsync()
    {
        try
        {
            if (ShowDummyDevices)
            {
                return (await _backend.GetDevicesAsync()).Concat(BluetoothDevice.DummyDevices());
            }
            return await _backend.GetDevicesAsync();
        }
        catch (BluetoothException ex)
        {
            OnBluetoothError(ex);
        }

        return Array.Empty<BluetoothDevice>();
    }

    private async Task<string> GetDeviceNameAsync()
    {
        var fallbackName = CurrentModel.GetModelMetadataAttribute()?.Name ?? Strings.Unknown;
        try
        {
            var devices = await _backend.GetDevicesAsync();
            var device = devices.FirstOrDefault(d => d.Address == Device.Current?.MacAddress);
            return device?.Name ?? fallbackName;
        }
        catch (BluetoothException ex)
        {
            Log.Error(ex, "BluetoothImpl.GetDeviceName: Error while fetching device name");
            return fallbackName;
        }
    }
        
    public async Task<bool> ConnectAsync(Device? device = null, bool noRetry = false)
    {
        // Create new cancellation token source if the previous one has already been used
        if(_connectCancelSource.IsCancellationRequested)
            _connectCancelSource = new CancellationTokenSource();
        
        device ??= Device.Current;

        if (!HasValidDevice || device == null)
        {
            Log.Error("BluetoothImpl: Connection attempt without valid device");
            return false;
        }
        
        /* Load from configuration */
        try
        {
            DeviceName = await GetDeviceNameAsync();
            device.Name = DeviceName;
                        
            await _backend.ConnectAsync(device.MacAddress, DeviceSpec.ServiceUuid.ToString(), _connectCancelSource.Token);
            return true;
        }
        catch (BluetoothException ex)
        {
            OnBluetoothError(ex);
            return false;
        }
        catch (TaskCanceledException)
        {
            Log.Warning("BluetoothImpl: Connection task cancelled");
            return false;
        }
    }

    public async Task DisconnectAsync()
    {
        try
        {
            // Cancel the connection attempt if it's still in progress
            try
            {
                await _connectCancelSource.CancelAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "BluetoothImpl: Error while cancelling connection attempt");
            } 

            await _backend.DisconnectAsync();
            Disconnected?.Invoke(this, "User requested disconnect");
            IsConnected = false;
            LastErrorMessage = string.Empty;
        }
        catch (BluetoothException ex)
        {
            OnBluetoothError(ex);
        }
    }
        
    public async Task SendAsync(SppMessage msg)
    {
        if (!IsConnected)
            return;
            
        try
        {
            Log.Verbose("<< Outgoing: {Msg}", msg);
                
            foreach(var hook in ScriptManager.Instance.MessageHooks)
            {
                hook.OnMessageSend(ref msg);
            }

            var raw = msg.Encode();
                
            foreach(var hook in ScriptManager.Instance.RawStreamHooks)
            {
                hook.OnRawDataSend(ref raw);
            }
                
            await _backend.SendAsync(raw);
        }
        catch (BluetoothException ex)
        {
            OnBluetoothError(ex);
        }
    }
        
    public async Task SendResponseAsync(MsgIds id, params byte[]? payload)
    {
        await SendAsync(new SppMessage{Id = id, Payload = payload ?? Array.Empty<byte>(), Type = MsgTypes.Response});
    }

    public async Task SendRequestAsync(MsgIds id, params byte[]? payload)
    {
        await SendAsync(new SppMessage{Id = id, Payload = payload ?? Array.Empty<byte>(), Type = MsgTypes.Request});
    }
        
    public async Task SendRequestAsync(MsgIds id, bool payload)
    {
        await SendRequestAsync(id, payload ? [0x01] : [0x00]);
    }
    
    public async Task SendAsync(BaseMessageEncoder encoder)
    {
        await SendAsync(encoder.Encode());
    }
        
    public void UnregisterDevice(Device? device = null)
    {
        var mac = device?.MacAddress ?? Device.Current?.MacAddress;
        var toRemove = Settings.Data.Devices.FirstOrDefault(x => x.MacAddress == mac);
        if(toRemove == null)
            return;
        
        // Disconnect if the device is currently connected
        if(mac == Device.Current?.MacAddress)
            _ = DisconnectAsync();

        Settings.Data.Devices.Remove(toRemove);
        DeviceMessageCache.Instance.Clear();
        
        Device.Current = Settings.Data.Devices.FirstOrDefault();
    }
    
    private void OnDisconnected(object? sender, string reason)
    {
        if (!SuppressDisconnectionEvents)
        {
            LastErrorMessage = Strings.Connlost;
            IsConnected = false;
            Disconnected?.Invoke(this, reason);
        }
    }

    private void OnRfcommConnected(object? sender, EventArgs e)
    {
        _ = Task.Delay(150).ContinueWith(_ =>
        {
            if (HasValidDevice)
            {
                Connected?.Invoke(this, EventArgs.Empty);
                IsConnected = true;
            }
            else
            {
                Log.Error("BluetoothImpl: Suppressing Connected event, device not properly registered");
            }
        });
    }
    
    private void OnNewDataAvailable(object? sender, byte[] frame)
    {
        NewDataReceived?.Invoke(this, frame);
        
        /* Discard data if not properly registered */
        if (!HasValidDevice)
        {
            return;
        }

        IsConnected = true;
        IncomingQueue.Enqueue(frame);
    }

    private void DataConsumerLoop()
    {
        while (true)
        {
            try
            {
                _loopCancelSource.Token.ThrowIfCancellationRequested();
                Task.Delay(50).Wait(_loopCancelSource.Token);
            }
            catch (OperationCanceledException)
            {
                _incomingData.Clear();
                throw;
            }
                
            lock (IncomingQueue)
            {
                if (IncomingQueue.IsEmpty) continue;
                while (IncomingQueue.TryDequeue(out var frame))
                {
                    _incomingData.AddRange(frame);
                }
            }

            try
            {
                foreach (var message in SppMessage.DecodeRawChunk(_incomingData, CurrentModel))
                {
                    MessageReceived?.Invoke(this, message);
                }
            }
            catch (InvalidPacketException ex)
            {
                InvalidDataReceived?.Invoke(this, ex);
            }
        }
    }
}