using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("Get Token")]
    [UnitCategory("Dolby.io Comms")]
    public class GetToken : Unit, IDolbyUnit
    {
        [DoNotSerialize]
        public ValueInput ApplicationKey;

        [DoNotSerialize]
        public ValueInput AplicationSecret;

        [DoNotSerialize]
        public ValueOutput TokenAction { get; private set; }

        protected override void Definition()
        {
            ApplicationKey = ValueInput<string>(nameof(ApplicationKey), "key");
            AplicationSecret = ValueInput<string>(nameof(AplicationSecret), "secret");
            TokenAction = ValueOutput<System.Func<string>>(nameof(TokenAction), GetAccessToken);
        }

        private System.Func<string> GetAccessToken(Flow flow)
        {
            var key = flow.GetValue<string>(ApplicationKey);
            var secret = flow.GetValue<string>(AplicationSecret);

            System.Func<string> action = () => { return DolbyIOManager.GetToken(key, secret).Result; };

            return action;
        }
    }
}