using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    public class ConferenceUnit : Unit, IDolbyUnit
    {
        protected DolbyIOSDK _sdk = DolbyIOManager.Sdk;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger;

        [DoNotSerialize]
        public ValueInput Scale;

        [DoNotSerialize]
        public ValueInput Forward;

        [DoNotSerialize]
        public ValueInput Up;

        [DoNotSerialize]
        public ValueInput Right;

        protected override void Definition()
        {
            Scale = ValueInput<Vector3>(nameof(Scale), new Vector3(1, 1, 1));
            Forward = ValueInput<Vector3>(nameof(Forward), new Vector3(0, 0, 1));
            Up = ValueInput<Vector3>(nameof(Up), new Vector3(0, 1, 0));
            Right = ValueInput<Vector3>(nameof(Right), new Vector3(1, 0, 0));
        }
    }
}