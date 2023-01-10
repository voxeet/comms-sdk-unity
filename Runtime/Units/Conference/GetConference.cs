using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("Get Conference")]
    [UnitCategory("Dolby.io Comms")]
    public class GetConferenceUnit : DolbyUnit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger;

        [DoNotSerialize]
        public ValueOutput Conference;

        protected override void Definition()
        {
            InputTrigger = ControlInput(nameof(InputTrigger), Leave);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Conference = ValueOutput<Conference>(nameof(Conference));
        }

        private ControlOutput Leave(Flow flow)
        {
            Conference conference = Sdk.Conference.GetCurrentAsync().Result;
            flow.SetValue(Conference, conference);

            return OutputTrigger;
        }
    }
}