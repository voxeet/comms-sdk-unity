using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("On Audio Device Changed Event")]
    [UnitCategory("Events\\Dolby.io Comms")]
    public class AudioDeviceChangedEvent : EventUnit<DolbyIO.Comms.AudioDevice>
    {
        [DoNotSerialize]
        public ValueOutput AudioDevice { get; private set; }
        protected override bool register => true;

        // Adding an EventHook with the name of the event to the list of visual scripting events.
        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventNames.AudioDeviceChangedEvent);
        }

        protected override void Definition()
        {
            base.Definition();
            // Setting the value on our port.
            AudioDevice = ValueOutput<DolbyIO.Comms.AudioDevice>(nameof(AudioDevice));
        }
        
        // Setting the value on our port.
        protected override void AssignArguments(Flow flow, DolbyIO.Comms.AudioDevice data)
        {
            flow.SetValue(AudioDevice, data);
        }
    }
}