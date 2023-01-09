using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("Get Audio Devices")]
    [UnitCategory("Dolby.io Comms")]
    public class AudioDeviceUnit : Unit, IDolbyUnit
    {
        protected DolbyIOSDK _sdk = DolbyIOManager.Sdk;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger;

        [DoNotSerialize]
        public ValueInput DeviceDirection;

        [DoNotSerialize]
        public ValueOutput AudioDevices;
        
        protected override void Definition()
        {
            InputTrigger = ControlInput(nameof(InputTrigger), GetDevices);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            DeviceDirection = ValueInput<DolbyIO.Comms.DeviceDirection>(nameof(DeviceDirection), DolbyIO.Comms.DeviceDirection.Input);
            AudioDevices = ValueOutput<List<DolbyIO.Comms.AudioDevice>>(nameof(AudioDevices));
        }

        private ControlOutput GetDevices(Flow flow)
        {
            var direction = flow.GetValue<DolbyIO.Comms.DeviceDirection>(DeviceDirection);
            var devices = _sdk.MediaDevice.GetAudioDevicesAsync().Result;

            flow.SetValue(AudioDevices, devices.FindAll(d => d.Direction == direction));

            return OutputTrigger;
        }
    }
}