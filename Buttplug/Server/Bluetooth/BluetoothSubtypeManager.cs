﻿// <copyright file="BluetoothSubtypeManager.cs" company="Nonpolynomial Labs LLC">
// Buttplug C# Source Code File - Visit https://buttplug.io for more info about the project.
// Copyright (c) Nonpolynomial Labs LLC. All rights reserved.
// Licensed under the BSD 3-Clause license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Buttplug.Core.Logging;
using Buttplug.Server.Bluetooth.Devices;
using JetBrains.Annotations;

namespace Buttplug.Server.Bluetooth
{
    public abstract class BluetoothSubtypeManager : DeviceSubtypeManager
    {
        [NotNull]
        [ItemNotNull]
        protected readonly List<IBluetoothDeviceInfo> BuiltinDevices;

        protected BluetoothSubtypeManager([NotNull] IButtplugLogManager aLogManager)
            : base(aLogManager)
        {
            // This used to go through all assemblies looking for IBluetoothDeviceInfo, but that
            // ended up constantly breaking due to Reflection issues on different platforms/setups.
            // Now we just build a new info array on manager load, keeps things simple, and outside
            // info can be added via AddInfo calls.
            BuiltinDevices = new List<IBluetoothDeviceInfo>
            {
                new FleshlightLaunchBluetoothInfo(),
                new KiirooBluetoothInfo(),
                new KiirooGen2VibeBluetoothInfo(),
                new KiirooOnyx2BluetoothInfo(),
                new YoucupsBluetoothInfo(),
                new LovenseBluetoothInfo(),
                new MagicMotionBluetoothInfo(),
                new VibratissimoBluetoothInfo(),
                new VorzeSABluetoothInfo(),
                new WeVibeBluetoothInfo(),
                new MysteryVibeBluetoothInfo(),
                new LiBoBluetoothInfo(),
                new PicobongBluetoothInfo(),
            };
        }
    }
}
