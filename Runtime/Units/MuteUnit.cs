using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("Mute Participant")]
    [UnitCategory("Dolby.io Comms")]
    public class MuteUnit : Unit, IDolbyUnit
    {
        protected DolbyIOSDK _sdk = DolbyIOManager.Sdk;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger;

        [DoNotSerialize]
        public ValueInput ParticipantId;

        [DoNotSerialize]
        public ValueInput Muted;

        protected override void Definition()
        {
            InputTrigger = ControlInput(nameof(InputTrigger), Mute);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));
            
            ParticipantId = ValueInput<string>(nameof(ParticipantId), "");
            Muted = ValueInput<bool>(nameof(Muted), false);

        }

        private ControlOutput Mute(Flow flow)
        {
            string id = flow.GetValue<string>(ParticipantId);
            bool muted = flow.GetValue<bool>(Muted);

            if (String.IsNullOrWhiteSpace(id))
            {
                _sdk.Audio.Local.MuteAsync(muted).Wait();

            } 
            else
            {
                _sdk.Audio.Remote.MuteAsync(muted, id).Wait();
            }

            return OutputTrigger;
        }
    }
}