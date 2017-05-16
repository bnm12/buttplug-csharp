﻿using System;
using Buttplug.Logging;

namespace Buttplug.Core
{
    internal abstract class DeviceSubtypeManager
    {
        protected readonly ButtplugLog BpLogger;

        public event EventHandler<DeviceAddedEventArgs> DeviceAdded;

        protected DeviceSubtypeManager()
        {
            BpLogger = ButtplugLogManager.GetLogger(LogProvider.GetCurrentClassLogger());
            BpLogger.Trace($"Setting up Device Manager {GetType().Name}");
        }

        protected void InvokeDeviceAdded(DeviceAddedEventArgs args)
        {
            //Can't invoke this from child classes? Weird.
            DeviceAdded?.Invoke(this, args);
        }

        public abstract void StartScanning();

        public abstract void StopScanning();
    }
}