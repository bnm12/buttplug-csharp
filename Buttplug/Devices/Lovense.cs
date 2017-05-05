﻿using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Buttplug.Messages;
using Windows.Storage.Streams;
using Buttplug.Core;

namespace Buttplug.Devices
{
    class LovenseBluetoothInfo : IBluetoothDeviceInfo
    {
        public Guid[] Services { get; } = {new Guid("6e400001-b5a3-f393-e0a9-e50e24dcca9e")};
        public string[] Names { get; } = {"LVS-S001", "LVS-Z001"};
        public Guid[] Characteristics { get; } =
        {
            // tx characteristic
            new Guid("6e400002-b5a3-f393-e0a9-e50e24dcca9e"),
            // rx characteristic
            new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e")
        };

        public ButtplugBluetoothDevice CreateDevice(BluetoothLEDevice aDevice, GattCharacteristic[] aCharacteristics)
        {
            return new Lovense(aDevice, aCharacteristics[0], aCharacteristics[1]);
        }
    }

    internal class Lovense : ButtplugBluetoothDevice
    {
        private GattCharacteristic WriteChr;
        private GattCharacteristic ReadChr;

        public Lovense(BluetoothLEDevice aDevice,
                       GattCharacteristic aWriteChr,
                       GattCharacteristic aReadChr) :
            base($"Lovense Device ({aDevice.Name})", aDevice)
        {
            this.WriteChr = aWriteChr;
            this.ReadChr = aReadChr;
        }

        public override async Task<bool> ParseMessage(IButtplugDeviceMessage msg)
        {
            switch (msg)
            {
                case SingleMotorVibrateMessage m:
                    BPLogger.Trace("Lovense toy got SingleMotorVibrateMessage");
                    var writer = new DataWriter();
                    writer.WriteString($"Vibrate:{(int)(m.Speed * 20)};");
                    IBuffer buf = writer.DetachBuffer();
                    BPLogger.Trace(buf);
                    await WriteChr.WriteValueAsync(buf);
                    return true;
            }

            return false;
        }
    }
}