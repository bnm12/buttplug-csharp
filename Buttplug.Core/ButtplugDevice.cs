using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Buttplug.Core.Messages;
using JetBrains.Annotations;
using static Buttplug.Core.Messages.Error;

namespace Buttplug.Core
{
    /// <summary>
    /// An abstract representation of a Buttplug device
    /// </summary>
    public abstract class ButtplugDevice : IButtplugDevice
    {
        /// <summary>
        /// Gets the name of the device
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Get the indentifier of the device
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets the index of the device
        /// </summary>
        public uint Index { get; set; }

        /// <summary>
        /// Is the device connected?
        /// </summary>
        public bool IsConnected => !_isDisconnected;

        /// <summary>
        /// Event handler for signalling the device has been removed
        /// </summary>
        [CanBeNull]
        public event EventHandler DeviceRemoved;

        /// <summary>
        /// Event handler for signalling when the device has data to share
        /// </summary>
        [CanBeNull]
        public event EventHandler<MessageReceivedEventArgs> MessageEmitted;

        /// <summary>
        /// Gets the logger
        /// </summary>
        [NotNull]
        protected readonly IButtplugLog BpLogger;

        /// <summary>
        /// Gets the message handler functions
        /// </summary>
        [NotNull]
        protected readonly Dictionary<Type, ButtplugDeviceWrapper> MsgFuncs;

        private bool _isDisconnected;

        /// <summary>
        /// A container class for message functions and attributes
        /// </summary>
        public class ButtplugDeviceWrapper
        {
            /// <summary>
            /// The function to call when a message of the particular type is received
            /// </summary>
            public Func<ButtplugDeviceMessage, Task<ButtplugMessage>> Function;

            /// <summary>
            /// A list of attributes ascoiated with the message
            /// </summary>
            public MessageAttributes Attrs;

            /// <summary>
            /// Initializes a new instance of the <see cref="ButtplugDeviceWrapper"/> class.
            /// </summary>
            /// <param name="aFunction">The method to call for the message</param>
            /// <param name="aAttrs">The message attributes</param>
            public ButtplugDeviceWrapper(Func<ButtplugDeviceMessage, Task<ButtplugMessage>> aFunction,
                                         MessageAttributes aAttrs = null)
            {
                Function = aFunction;
                Attrs = aAttrs ?? new MessageAttributes();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtplugDevice"/> class.
        /// </summary>
        /// <param name="aLogManager">The log manager</param>
        /// <param name="aName">The device name</param>
        /// <param name="aIdentifier">The device identifier</param>
        protected ButtplugDevice([NotNull] IButtplugLogManager aLogManager,
            [NotNull] string aName,
            [NotNull] string aIdentifier)
        {
            BpLogger = aLogManager.GetLogger(GetType());
            MsgFuncs = new Dictionary<Type, ButtplugDeviceWrapper>();
            Name = aName;
            Identifier = aIdentifier;
        }

        /// <summary>
        /// Gets allowed message types for this device
        /// </summary>
        /// <returns>An enumerable of messahe types</returns>
        public IEnumerable<Type> GetAllowedMessageTypes()
        {
            return MsgFuncs.Keys;
        }

        /// <summary>
        /// Gets message attributes for a specific message
        /// </summary>
        /// <param name="aMsg">The message</param>
        /// <returns>Any attributes for the message</returns>
        public MessageAttributes GetMessageAttrs(Type aMsg)
        {
            if (MsgFuncs.TryGetValue(aMsg, out var wrapper))
            {
                return wrapper.Attrs ?? new MessageAttributes();
            }

            return new MessageAttributes();
        }

        /// <summary>
        /// Invokes the DeviceRemoved event handler.
        /// Required to disconnect devices from the lower levels.
        /// </summary>
        protected void InvokeDeviceRemoved()
        {
            _isDisconnected = true;
            DeviceRemoved?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Interprets a Buttplug device message by ensuring it is supported
        /// by this device, then invoking the handler method and returing the
        /// resulting Buttplug message.
        /// </summary>
        /// <param name="aMsg">The device message to handle</param>
        /// <returns>The message response</returns>
        public async Task<ButtplugMessage> ParseMessage([NotNull] ButtplugDeviceMessage aMsg)
        {
            if (_isDisconnected)
            {
                return BpLogger.LogErrorMsg(aMsg.Id, ErrorClass.ERROR_DEVICE,
                    $"{Name} has disconnected and can no longer process messages.");
            }

            if (!MsgFuncs.ContainsKey(aMsg.GetType()))
            {
                return BpLogger.LogErrorMsg(aMsg.Id, ErrorClass.ERROR_DEVICE,
                    $"{Name} cannot handle message of type {aMsg.GetType().Name}");
            }

            // We just checked whether the key exists above, so we're ok.
            // ReSharper disable once PossibleNullReferenceException
            return await MsgFuncs[aMsg.GetType()].Function.Invoke(aMsg);
        }

        /// <summary>
        /// Initiaizes a device (perform any one-tme-setup stuff for example)
        /// </summary>
        /// <returns>A message resonse to the initialisation</returns>
        public virtual Task<ButtplugMessage> Initialize()
        {
            return Task.FromResult<ButtplugMessage>(new Ok(ButtplugConsts.SystemMsgId));
        }

        /// <summary>
        /// Disconnects the device
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// Invokes the EmitMessage event handler.
        /// Required to allow events to be raised for this device from the lower levels.
        /// </summary>
        /// <param name="aMsg">The message to emit from the device</param>
        protected void EmitMessage(ButtplugMessage aMsg)
        {
            MessageEmitted?.Invoke(this, new MessageReceivedEventArgs(aMsg));
        }
    }
}