using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("Set Player Direction")]
    [UnitCategory("Dolby.io Comms")]
    public class SetPlayerDirectionUnit : Unit, IDolbyUnit
    {
        protected DolbyIOSDK _sdk = DolbyIOManager.Sdk;
        
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger;

        [DoNotSerialize]
        public ValueInput Direction;

        protected override void Definition()
        {
            InputTrigger = ControlInput(nameof(InputTrigger), SetDirection);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));
            Direction = ValueInput<Vector3>(nameof(Direction), new Vector3(0, 1.0f, 0));
        }

        private ControlOutput SetDirection(Flow flow)
        {
            var direction = flow.GetValue<Vector3>(Direction);

            if (_sdk.IsInitialized && _sdk.Session.IsOpen && _sdk.Conference.IsInConference)
            {
                _sdk.Conference.SetSpatialDirectionAsync
                (
                    new System.Numerics.Vector3(direction.x, direction.y, direction.z)
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