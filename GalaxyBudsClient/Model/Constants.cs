﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using GalaxyBudsClient.Generated.I18N;
using GalaxyBudsClient.Generated.Model.Attributes;
using GalaxyBudsClient.Model.Attributes;
using GalaxyBudsClient.Platform;

namespace GalaxyBudsClient.Model
{
    namespace Constants
    {
        public static class Uuids
        {
            public static readonly Guid Buds = new("{00001102-0000-1000-8000-00805f9b34fd}");
            public static readonly Guid BudsPlus = new("{00001101-0000-1000-8000-00805F9B34FB}");
            public static readonly Guid BudsLive = new("{00001101-0000-1000-8000-00805F9B34FB}");
            public static readonly Guid BudsPro = new("{00001101-0000-1000-8000-00805F9B34FB}");
            public static readonly Guid Buds2 = new("{2e73a4ad-332d-41fc-90e2-16bef06523f2}");
            public static readonly Guid Buds2Pro = new("{2e73a4ad-332d-41fc-90e2-16bef06523f2}");
            public static readonly Guid BudsFe = new("{2e73a4ad-332d-41fc-90e2-16bef06523f2}");
        }
      
        [CompiledEnum]
        public enum BixbyLanguages
        {
            [Description("de-DE")]
            de = 0,
            [Description("en-GB")] 
            enGb = 1,
            [Description("en-US")]
            enUs = 2,
            [Description("es-ES")]
            es = 3,
            [Description("fr-FR")]
            fr = 4,
            [Description("it-IT")]
            it = 5,
            [Description("ko-KR")]
            kr = 6,
            [Description("pt-BR")]
            br = 7,
            [Description("zh-CN")]
            cn = 8,
            [Description("hu-HU")]
            hu = 9
        }
        
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [CompiledEnum]
        public enum Locales
        {
            [Description("English")]
            en,
            [Description("German")]
            de,
            [Description("Spanish")]
            es,
            [Description("Portuguese")]
            pt,
            [Description("Italian")]
            it,
            [Description("Korean")]
            ko,
            [Description("Japanese")]
            ja,
            [Description("Russian")]
            ru,
            [Description("Ukrainian")]
            ua,
            [Description("Czech")]
            cz,
            [Description("Turkish")]
            tr,
            [Description("Simplified-Chinese")]
            cn,
            [Description("Traditional-Chinese")]
            tw,
            [Description("Indonesian")]
            in_,
            [Description("Vietnamese")]
            vn_,
            [Description("Greek")]
            gr,
            [Description("Romanian")]
            ro,
            [Description("Hebrew")]
            il,
            [Description("French")]
            fr,
            [Description("Magyar")]
            hu,
			[Description("Nederlands")]
			nl,
            [IgnoreDataMember, Description("custom_language.xaml")]
            custom
        }
        
        public enum SpatialAudioControl
        {
            Attach = 0,
            Detach = 1,
            AttachSuccess = 2,
            DetachSuccess = 3,
            KeepAlive = 4,
            WearOnOff = 5,
            QuerySensorSupported = 6,
            SpatialBufOn = 7,
            SpatialBufOff = 8,
            QueryGyroBiasExistence = 9,
            ManualGyrocalStart = 10,
            ManualGyrocalCancel = 11,
            ManualGyrocalQueryReady = 12,
            ResetGyroInUseBias = 13,
            
            DebugResetBiasAll = 64,
            DebugResetBiasInUse = 65,
            DebugResetPrintTimestamp = 66
        }
        
        public enum SpatialAudioData
        {
            Unknown,
            BudGrv = 32,
            WearOn = 33,
            WearOff = 34,
            BudGyrocal = 35,
            BudSensorStuck = 36,
            SensorSupported = 37,
            GyroBiasExistence = 38,
            ManualGyrocalReady = 39,
            ManualGyrocalNotReady = 40,
            BudGyrocalFail = 41
        }
        
        [CompiledEnum]
        public enum TemperatureUnits
        {
            [Description("F")]
            Fahrenheit,
            [Description("C")]
            Celsius
        }
        
        [CompiledEnum]
        public enum Themes
        {    
            [LocalizableDescription(Keys.DarkmodeDisabled)]
            Light,
            [LocalizableDescription(Keys.DarkmodeEnabled)]
            Dark,
            [LocalizableDescription(Keys.DarkmodeBlurEnabled)]
            DarkBlur,
            [RequiresPlatform(PlatformUtils.Platforms.Windows, 22000 /* Windows 11 */)]
            [LocalizableDescription(Keys.DarkmodeMicaEnabled)]
            DarkMica,
            
            [IgnoreDataMember, LocalizableDescription(Keys.DarkmodeSystem)]
            System // <- Disabled
        }

        [CompiledEnum]
        public enum PopupPlacement
        {
            [LocalizableDescription(Keys.ConnpopupPlacementTl)]
            TopLeft,
            [LocalizableDescription(Keys.ConnpopupPlacementTc)]
            TopCenter,
            [LocalizableDescription(Keys.ConnpopupPlacementTr)]
            TopRight,
            [LocalizableDescription(Keys.ConnpopupPlacementBl)]
            BottomLeft,
            [LocalizableDescription(Keys.ConnpopupPlacementBc)]
            BottomCenter,
            [LocalizableDescription(Keys.ConnpopupPlacementBr)]
            BottomRight
        }
        
        [CompiledEnum]
        public enum VoiceDetectTimeouts
        {    
            [LocalizableDescription(Keys.NcVoicedetectTimeoutItem)]
            Sec5 = 5,
            [LocalizableDescription(Keys.NcVoicedetectTimeoutItem)]
            Sec10 = 10,
            [LocalizableDescription(Keys.NcVoicedetectTimeoutItem)]
            Sec15 = 15
        }
        
        [CompiledEnum]
        public enum Models
        {
            [ModelMetadata(Name = "", FwPattern = "", BuildPrefix = "")]
            NULL = 0,
            [ModelMetadata(Name = "Galaxy Buds", FwPattern = "R170", BuildPrefix = "R170")]
            Buds = 1,
            [ModelMetadata(Name = "Galaxy Buds+", FwPattern = "SM-R175", BuildPrefix = "R175")]
            BudsPlus = 2,
            [ModelMetadata(Name = "Galaxy Buds Live", FwPattern = "SM-R180", BuildPrefix = "R180")]
            BudsLive = 3,
            [ModelMetadata(Name = "Galaxy Buds Pro", FwPattern = "SM-R190", BuildPrefix = "R190")]
            BudsPro = 4,
            [ModelMetadata(Name = "Galaxy Buds2", FwPattern = "SM-R177", BuildPrefix = "R177")]
            Buds2 = 5,
            [ModelMetadata(Name = "Galaxy Buds2 Pro", FwPattern = "SM-R510", BuildPrefix = "R510")]
            Buds2Pro = 6,
            [ModelMetadata(Name = "Galaxy Buds FE", FwPattern = "SM-R400N", BuildPrefix = "R400N")]
            BudsFe = 7
        }

        [CompiledEnum]
        public enum Colors
        {
            Unknown = 0,
            
            BudsPlusBlue = 258,
            BudsPlusPink = 259,
            BudsPlusBlack = 260,
            BudsPlusWhite = 261,
            BudsPlusThomBrown = 262,
            BudsPlusRed = 263,
            BudsPlusDeepBlue = 264,
            BudsPlusOlympic = 265, // NOTE: Unreleased color
            BudsPlusPurple = 266,
        
            BudsLiveBlack = 278,
            BudsLiveWhite = 279,
            BudsLiveBronze = 280,
            BudsLiveRed = 281,
            BudsLiveBlue = 282,
            BudsLiveThomBrown = 283,
            BudsLiveGrey = 284,
        
            BudsProBlack = 298,
            BudsProSilver = 299,
            BudsProViolet = 300,
            BudsProWhite = 301,
        
            Buds2White = 313,
            Buds2Black = 314,
            Buds2Yellow = 315, // NOTE: Unreleased color
            Buds2Green = 316,
            Buds2Violet = 317,
            Buds2ThomBrown = 318,
            Buds2MaisonKitsune = 319,
            Buds2AbsoluteBlack = 320,
            Buds2Grey = 321,
        
            Buds2ProGrey = 326,
            Buds2ProWhite = 327,
            Buds2ProViolet = 328,
        
            BudsFeGraphite = 330,
            BudsFeWhite = 331
        }

        [CompiledEnum]
        public enum PlacementStates
        {
            [LocalizableDescription(Keys.PlacementDisconnected)]
            Disconnected = 0,
            [LocalizableDescription(Keys.PlacementWearing)]
            Wearing = 1,
            [LocalizableDescription(Keys.PlacementNotWearing)]
            Idle = 2,
            [LocalizableDescription(Keys.PlacementInCase)]
            Case = 3,
            ClosedCase = 4,
            
            // Custom states
            [LocalizableDescription(Keys.PlacementCharging)]
            Charging = 100
        }

        public enum LegacyWearStates
        {
            None = 0,
            R = 1,
            L = 16,
            Both = 17
        }
        public enum DevicesInverted
        {
            L = 1,
            R = 0
        }
        
        [CompiledEnum]
        public enum Devices
        {
            L = 0,
            R = 1
        }
        
        public enum SppRoleStates
        {
            Done = 0,
            Changing = 1,
            Unknown = 2
        }
        
        public enum NoiseControlModes
        {
            Off = 0,
            AmbientSound = 2,
            NoiseReduction = 1
        }
   
        [CompiledEnum]
        public enum NoiseControlCycleModes
        {
            [LocalizableDescription(Keys.TouchpadNoiseControlModeAncAmb)]
            AncAmb = 0,
            [LocalizableDescription(Keys.TouchpadNoiseControlModeAncOff)]
            AncOff = 1,
            [LocalizableDescription(Keys.TouchpadNoiseControlModeAmbOff)]
            AmbOff = 2,
            [IgnoreDataMember, LocalizableDescription(Keys.Unknown)]
            Unknown = 99
        }
        
        [CompiledEnum]
        public enum TouchOptions
        {
            [LocalizableDescription(Keys.TouchoptionVoice)]
            VoiceAssistant,
            [LocalizableDescription(Keys.TouchoptionQuickambientsound)]
            QuickAmbientSound,
            [LocalizableDescription(Keys.TouchoptionVolume)]
            Volume,
            [LocalizableDescription(Keys.TouchoptionAmbientsound)]
            AmbientSound,
            [LocalizableDescription(Keys.Anc)]
            Anc,
            [LocalizableDescription(Keys.TouchoptionSwitchNoisecontrols)]
            NoiseControl,
            [LocalizableDescription(Keys.TouchoptionSpotify)]
            SpotifySpotOn,
            [IgnoreDataMember, LocalizableDescription(Keys.TouchoptionCustom)]
            OtherL,
            [IgnoreDataMember, LocalizableDescription(Keys.TouchoptionCustom)]
            OtherR
        }
        
        public enum ClientDeviceTypes
        {
            Samsung = 1,
            Other = 2
        }
        
        public enum EqPresets
        {
            BassBoost = 0,
            Soft = 1,
            Dynamic = 2,
            Clear = 3,
            TrebleBoost = 4
        }
        
        public enum AmbientTypes
        {
            Default = 0,
            VoiceFocus = 1
        }
        
        [CompiledEnum]
        public enum DynamicTrayIconModes
        {
            [LocalizableDescription(Keys.SettingsDynTrayModeOff)]
            Disabled = 0,
            [LocalizableDescription(Keys.SettingsDynTrayModeBatteryMin)]
            BatteryMin = 1,
            [LocalizableDescription(Keys.SettingsDynTrayModeBatteryAvg)]
            BatteryAvg = 2
        }
    }
}
