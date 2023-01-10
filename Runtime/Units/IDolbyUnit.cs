using UnityEngine;
using Unity.VisualScripting;

using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    public interface IDolbyUnit : IUnit
    {
    }

    public abstract class DolbyUnit : Unit, IDolbyUnit
    {
        private DolbyIOSDK _sdk = DolbyIOManager.Sdk;

        protected DolbyIOSDK Sdk 
        {
            get => _sdk;
        }
    }
}