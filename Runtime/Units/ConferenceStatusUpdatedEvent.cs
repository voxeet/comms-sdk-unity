using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("On Conference Status Updated Event")]
    [UnitCategory("Events\\Dolby.io Comms")]
    public class ConferenceStatusUpdatedEvent : EventUnit<ConferenceStatus>
    {
        [DoNotSerialize]
        public ValueOutput Status { get; private set; }
        protected override bool register => true;

        // Adding an EventHook with the name of the event to the list of visual scripting events.
        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventNames.ConferenceStatusUpdatedEvent);
        }

        protected override void Definition()
        {
            base.Definition();
            // Setting the value on our port.
            Status = ValueOutput<ConferenceStatus>(nameof(Status));
        }
        
        // Setting the value on our port.
        protected override void AssignArguments(Flow flow, ConferenceStatus data)
        {
            flow.SetValue(Status, data);
        }
    }
}