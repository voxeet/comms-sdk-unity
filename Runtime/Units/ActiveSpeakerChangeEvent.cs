using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("On Active Speaker Change Event")]
    [UnitCategory("Events\\Dolby.io Comms")]
    public class ActiveSpeakerChangeEvent : EventUnit<List<string>>
    {
        [DoNotSerialize]
        public ValueOutput ParticipantIds { get; private set; }
        protected override bool register => true;

        // Adding an EventHook with the name of the event to the list of visual scripting events.
        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventNames.ActiveSpeakerChangeEvent);
        }

        protected override void Definition()
        {
            base.Definition();
            // Setting the value on our port.
            ParticipantIds = ValueOutput<List<string>>(nameof(ParticipantIds));
        }
        
        // Setting the value on our port.
        protected override void AssignArguments(Flow flow, List<string> data)
        {
            flow.SetValue(ParticipantIds, data);
        }
    }
}