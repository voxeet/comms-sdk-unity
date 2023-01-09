using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("On Participant Updated Event")]
    [UnitCategory("Events\\Dolby.io Comms")]
    public class ParticipantUpdatedEvent : EventUnit<Participant>
    {
        [DoNotSerialize]
        public ValueOutput Participant { get; private set; }
        protected override bool register => true;

        // Adding an EventHook with the name of the event to the list of visual scripting events.
        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventNames.ParticipantUpdatedEvent);
        }

        protected override void Definition()
        {
            base.Definition();
            // Setting the value on our port.
            Participant = ValueOutput<Participant>(nameof(Participant));
        }
        // Setting the value on our port.
        protected override void AssignArguments(Flow flow, Participant data)
        {
            flow.SetValue(Participant, data);
        }
    }
}