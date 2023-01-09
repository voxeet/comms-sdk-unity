using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("On Invalid Token Error Event")]
    [UnitCategory("Events\\Dolby.io Comms")]
    public class InvalidTokenErrorEvent : EventUnit<string>
    {
        [DoNotSerialize]
        public ValueOutput Message { get; private set; }
        protected override bool register => true;

        // Adding an EventHook with the name of the event to the list of visual scripting events.
        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventNames.InvalidTokenErrorEvent);
        }

        protected override void Definition()
        {
            base.Definition();
            // Setting the value on our port.
            Message = ValueOutput<string>(nameof(Message));
        }
        
        // Setting the value on our port.
        protected override void AssignArguments(Flow flow, string data)
        {
            flow.SetValue(Message, data);
        }
    }
}