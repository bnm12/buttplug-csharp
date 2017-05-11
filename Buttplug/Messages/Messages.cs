﻿using Buttplug.Core;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Buttplug.Messages
{
    public class Ok : ButtplugMessage, IButtplugMessageOutgoingOnly
    {
        public Ok(uint aId) : base(aId)
        { }
    }

    public class Ping : ButtplugMessage
    {
        public Ping(uint aId) : base(aId)
        { }
    }

    public class Test : ButtplugMessage
    {
        private string _testStringImpl;

        [JsonProperty(Required = Required.Always)]
        public string TestString
        {
            get => _testStringImpl;
            set
            {
                if (value == "Error")
                {
                    throw new ArgumentException("Got an Error Message");
                }
                _testStringImpl = value;
            }
        }

        public Test(string aString, uint aId) : base(aId)
        {
            TestString = aString;
        }
    }

    public class Error : ButtplugMessage, IButtplugMessageOutgoingOnly
    {
        [JsonProperty(Required = Required.Always)]
        public string ErrorMessage { get; }

        public Error(string aErrorMessage, uint aId) : base(aId)
        {
            ErrorMessage = aErrorMessage;
        }
    }

    public class DeviceMessageInfo
    {
        public string DeviceName { get; }
        public uint DeviceIndex { get; }

        public DeviceMessageInfo(uint aIndex, string aName)
        {
            DeviceName = aName;
            DeviceIndex = aIndex;
        }
    }

    public class DeviceList : ButtplugMessage, IButtplugMessageOutgoingOnly
    {
        public DeviceMessageInfo[] Devices;

        public DeviceList(DeviceMessageInfo[] aDeviceList, uint aId) : base(aId)
        {
            Devices = aDeviceList;
        }
    }

    public class DeviceAdded : ButtplugDeviceMessage, IButtplugMessageOutgoingOnly
    {
        public string DeviceName { get; }

        public DeviceAdded(uint aIndex, string aName) : base(ButtplugConsts.SYSTEM_MSG_ID, aIndex)
        {
            DeviceName = aName;
        }
    }

    public class DeviceRemoved : ButtplugMessage, IButtplugMessageOutgoingOnly
    {
        public uint DeviceIndex { get; }

        public DeviceRemoved(uint aIndex) : base(ButtplugConsts.SYSTEM_MSG_ID)
        {
            DeviceIndex = aIndex;
        }
    }

    public class RequestDeviceList : ButtplugMessage
    {
        public RequestDeviceList(uint aId = ButtplugConsts.DEFAULT_MSG_ID) : base(aId)
        {
        }
    }

    public class StartScanning : ButtplugMessage
    {
        public StartScanning(uint aId = ButtplugConsts.DEFAULT_MSG_ID) : base(aId)
        {
        }
    }

    public class StopScanning : ButtplugMessage
    {
        public StopScanning(uint aId = ButtplugConsts.DEFAULT_MSG_ID) : base(aId)
        {
        }
    }

    public class ScanningFinished : ButtplugMessage, IButtplugMessageOutgoingOnly
    {
        public ScanningFinished() : base(ButtplugConsts.SYSTEM_MSG_ID)
        {
        }
    }

    public class RequestLog : ButtplugMessage
    {
        private static readonly Dictionary<string, NLog.LogLevel> Levels = new Dictionary<string, LogLevel>()
        {
            { "Off", NLog.LogLevel.Off },
            { "Fatal", NLog.LogLevel.Fatal },
            { "Error", NLog.LogLevel.Error },
            { "Warn", NLog.LogLevel.Warn },
            { "Info", NLog.LogLevel.Info },
            { "Debug", NLog.LogLevel.Debug },
            { "Trace", NLog.LogLevel.Trace }
        };

        public LogLevel LogLevelObj;
        private string _logLevelImpl;

        [JsonProperty(Required = Required.Always)]
        public string LogLevel
        {
            get => _logLevelImpl;
            set
            {
                if (value is null || !Levels.Keys.Contains(value))
                {
                    throw new ArgumentException($"Log level {value} is not valid.");
                }
                _logLevelImpl = value;
                LogLevelObj = Levels[_logLevelImpl];
            }
        }

        // JSON.Net gets angry if it doesn't have a default initializer.
        public RequestLog() : base(ButtplugConsts.DEFAULT_MSG_ID)
        {
        }

        public RequestLog(uint aId = ButtplugConsts.DEFAULT_MSG_ID) : base(aId) => LogLevel = "Off";

        public RequestLog(string aLogLevel, uint aId = ButtplugConsts.DEFAULT_MSG_ID) : base(aId)
        {
            LogLevel = aLogLevel;
        }
    }

    public class Log : ButtplugMessage, IButtplugMessageOutgoingOnly
    {
        public string LogLevel { get; }
        public string LogMessage { get; }

        public Log(string aLogLevel, string aLogMessage) : base(ButtplugConsts.SYSTEM_MSG_ID)
        {
            LogLevel = aLogLevel;
            LogMessage = aLogMessage;
        }
    }

    public class RequestServerInfo : ButtplugMessage
    {
        public RequestServerInfo(uint aId = ButtplugConsts.DEFAULT_MSG_ID) : base(aId)
        { }
    }

    public class ServerInfo : ButtplugMessage, IButtplugMessageOutgoingOnly
    {
        public int MajorVersion { get; }
        public int MinorVersion { get; }
        public int BuildVersion { get; }

        public ServerInfo(uint aId = ButtplugConsts.DEFAULT_MSG_ID) : base(aId)
        {
            MajorVersion = Assembly.GetAssembly(typeof(ServerInfo)).GetName().Version.Major;
            MinorVersion = Assembly.GetAssembly(typeof(ServerInfo)).GetName().Version.Minor;
            BuildVersion = Assembly.GetAssembly(typeof(ServerInfo)).GetName().Version.Build;
        }
    }

    public class FleshlightLaunchRawCmd : ButtplugDeviceMessage
    {
        private ushort _speedImpl;

        [JsonProperty(Required = Required.Always)]
        public ushort Speed
        {
            get => _speedImpl;
            set
            {
                if (value > 99)
                {
                    throw new ArgumentException("FleshlightLaunchRawMessage cannot have a speed higher than 99!");
                }
                _speedImpl = value;
            }
        }

        private ushort _positionImpl;

        [JsonProperty(Required = Required.Always)]
        public ushort Position
        {
            get => _positionImpl;
            set
            {
                if (value > 99)
                {
                    throw new ArgumentException("FleshlightLaunchRawMessage cannot have a position higher than 99!");
                }
                _positionImpl = value;
            }
        }

        public FleshlightLaunchRawCmd(uint aDeviceIndex, ushort aSpeed, ushort aPosition, uint aId = ButtplugConsts.DEFAULT_MSG_ID) : base(aId, aDeviceIndex)
        {
            Speed = aSpeed;
            Position = aPosition;
        }
    }

    public class LovenseRawCmd : ButtplugDeviceMessage
    {
        public LovenseRawCmd(uint aDeviceIndex, string aDeviceCmd, uint aId = ButtplugConsts.DEFAULT_MSG_ID) : base(aId, aDeviceIndex)
        {
        }
    }

    public class KiirooRawCmd : ButtplugDeviceMessage
    {
        private ushort _positionImpl;

        [JsonProperty(Required = Required.Always)]
        public ushort Position
        {
            get => _positionImpl;
            set
            {
                if (value > 4)
                {
                    throw new ArgumentException("KiirooRawCmd Position cannot be greater than 4");
                }
                _positionImpl = value;
            }
        }

        public KiirooRawCmd(uint aDeviceIndex, ushort aPosition, uint aId = ButtplugConsts.DEFAULT_MSG_ID) : base(aId, aDeviceIndex)
        {
            Position = aPosition;
        }
    }

    public class SingleMotorVibrateCmd : ButtplugDeviceMessage
    {
        private double _speedImpl;

        [JsonProperty(Required = Required.Always)]
        public double Speed
        {
            get => _speedImpl;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("SingleMotorVibrateMessage Speed cannot be less than 0!");
                }
                if (value > 1)
                {
                    throw new ArgumentException("SingleMotorVibrateMessage Speed cannot be greater than 1!");
                }
                _speedImpl = value;
            }
        }

        public SingleMotorVibrateCmd(uint aDeviceIndex, double aSpeed, uint aId = ButtplugConsts.DEFAULT_MSG_ID) : base(aId, aDeviceIndex)
        {
            Speed = aSpeed;
        }
    }
}