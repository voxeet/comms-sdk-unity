using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("Leave Conference")]
    [UnitCategory("Dolby.io Comms")]
    public class LeaveConferenceUnit : DolbyUnit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger;

        protected override void Definition()
        {
            InputTrigger = ControlInput(nameof(InputTrigger), Leave);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));
        }

        private ControlOutput Leave(Flow flow)
        {
            Sdk.Conference.LeaveAsync().Wait();

            return OutputTrigger;
        }
    }
}