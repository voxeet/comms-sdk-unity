using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("Set Audio Device Input")]
    [UnitCategory("Dolby.io Comms")]
    public class SetAudioDeviceInputUnit : Unit, IDolbyUnit
    {
        protected DolbyIOSDK _sdk = DolbyIOManager.Sdk;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger;

        public ValueInput AudioDevice;

        protected override void Definition()
        {
            InputTrigger = ControlInput(nameof(InputTrigger), SetDevice);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            AudioDevice = ValueInput<DolbyIO.Comms.AudioDevice>(nameof(AudioDevice));
        }

        private ControlOutput SetDevice(Flow flow)
        {
            var device = flow.GetValue<DolbyIO.Comms.AudioDevice>(AudioDevice);

            _sdk.MediaDevice.SetPreferredAudioInputDeviceAsync(device).Wait();

            return OutputTrigger;
        }
    }
}