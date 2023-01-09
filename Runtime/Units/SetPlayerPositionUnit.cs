using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("Set Player Position")]
    [UnitCategory("Dolby.io Comms")]
    public class SetPlayerPositionUnit : Unit, IDolbyUnit
    {
        protected DolbyIOSDK _sdk = DolbyIOManager.Sdk;
        
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger;

        [DoNotSerialize]
        public ValueInput Position;

        protected override void Definition()
        {
            InputTrigger = ControlInput(nameof(InputTrigger), SetPosition);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));
            Position = ValueInput<Vector3>(nameof(Position), new Vector3(0, 1.0f, 0));
        }

        private ControlOutput SetPosition(Flow flow)
        {
            var position = flow.GetValue<Vector3>(Position);

            if (_sdk.IsInitialized && _sdk.Session.IsOpen && _sdk.Conference.IsInConference)
            {
                _sdk.Conference.SetSpatialPositionAsync
                (
                    _sdk.Session.User.Id,
                    new System.Numerics.Vector3(position.x, position.y, position.z)
                )
                .ContinueWith(t =>
                {
                    Debug.LogError(t.Exception);
                }, TaskContinuationOptions.OnlyOnFaulted);
            }

            return OutputTrigger;
        }
    }
}