using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("Conference")]
    [UnitCategory("Dolby.io Comms")]
    public class SpatialConferenceUnit : ConferenceUnit
    {
        [DoNotSerialize]
        public ValueInput ConferenceAlias;

        [DoNotSerialize]
        public ValueInput SpatialAudioStyle;

        protected override void Definition()
        {
            base.Definition();

            InputTrigger = ControlInputCoroutine(nameof(InputTrigger), Join);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            ConferenceAlias = ValueInput<string>(nameof(ConferenceAlias), "Conference alias");
            SpatialAudioStyle = ValueInput<DolbyIO.Comms.SpatialAudioStyle>(nameof(SpatialAudioStyle), DolbyIO.Comms.SpatialAudioStyle.Shared);
        }

        private IEnumerator Join(Flow flow)
        {
            var options = new ConferenceOptions();
            options.Alias = flow.GetValue<string>(ConferenceAlias);
            options.Params.SpatialAudioStyle = flow.GetValue<SpatialAudioStyle>(SpatialAudioStyle);

            var conference = _sdk.Conference.CreateAsync(options).Result;

            var joinOptions = new JoinOptions();
            joinOptions.Constraints.Audio = true;
            joinOptions.Connection.SpatialAudio = true;

            _sdk.Conference.JoinAsync(conference, joinOptions).Wait();

            var scale = flow.GetValue<Vector3>(Scale);
            var forward = flow.GetValue<Vector3>(Forward);
            var up = flow.GetValue<Vector3>(Up);
            var right = flow.GetValue<Vector3>(Right);

            _sdk.Conference.SetSpatialEnvironmentAsync
            (
                new System.Numerics.Vector3(scale.x, scale.y, scale.z),
                new System.Numerics.Vector3(forward.x, forward.y, forward.z),
                new System.Numerics.Vector3(up.x, up.y, up.z),
                new System.Numerics.Vector3(right.x, right.y, right.z)
            ).Wait();

            yield return OutputTrigger;
        }
    }
}