using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;
using DolbyIO.Comms.Unity;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("Initialize")]
    [UnitCategory("Dolby.io Comms")]
    public class DolbyIOUnit : Unit, IDolbyUnit
    {
        private DolbyIOSDK _sdk = DolbyIOManager.Sdk;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger;

        [DoNotSerialize]
        public ValueInput AccessToken;

        [DoNotSerialize]
        public ValueInput ParticipantName;

        protected override void Definition()
        {
            InputTrigger = ControlInputCoroutine(nameof(InputTrigger), InitAndOpen);
            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            AccessToken = ValueInput<System.Object>(nameof(AccessToken), "My Access Token");
            ParticipantName = ValueInput<string>(nameof(ParticipantName), "Name");
        }

        private IEnumerator InitAndOpen(Flow flow)
        {
            System.Object token = flow.GetValue<System.Object>(AccessToken);
            if (token.GetType() == typeof(string))
            {
                _sdk.InitAsync(flow.GetValue<string>(AccessToken), () =>
                {
                    return flow.GetValue<string>(AccessToken);
                }).Wait();
            } 
            else if (token.GetType() == typeof(System.Func<string>))
            {
                System.Func<string> tokenAction = flow.GetValue<System.Func<string>>(AccessToken);
                _sdk.InitAsync(tokenAction(), () =>
                {
                    return tokenAction();
                }).Wait();
            }
            else
            {
                Debug.LogError("An access token or a method to get an access token must be provided");
            }
          
            _sdk.Conference.ParticipantAdded = new ParticipantAddedEventHandler(Participant =>
            {
                DolbyIOManager.QueueOnMainThread(() => EventBus.Trigger(EventNames.ParticipantAddedEvent, Participant));
            });

            _sdk.Conference.ParticipantUpdated = new ParticipantUpdatedEventHandler(Participant =>
            {
                DolbyIOManager.QueueOnMainThread(() => EventBus.Trigger(EventNames.ParticipantUpdatedEvent, Participant));
            });

            #nullable enable
            _sdk.Conference.ActiveSpeakerChange = new ActiveSpeakerChangeEventHandler
            (
                (string conferenceId, int count, string[]? ids) =>
                {
                    List<string> res = new List<string>();

                    if (count > 0)
                    {
                        res.AddRange(ids!);
                    }

                    DolbyIOManager.QueueOnMainThread(() => EventBus.Trigger(EventNames.ActiveSpeakerChangeEvent, res));
                }
            );

            _sdk.Conference.StatusUpdated = new ConferenceStatusUpdatedEventHandler
            (
                (ConferenceStatus status, string conferenceId) =>
                {
                    DolbyIOManager.QueueOnMainThread(() => EventBus.Trigger(EventNames.ConferenceStatusUpdatedEvent, status));
                }
            );

            _sdk.MediaDevice.AudioDeviceChanged = new AudioDeviceChangedEventHandler
            (
                (AudioDevice device, bool noDevice) =>
                {
                    DolbyIOManager.QueueOnMainThread(() => EventBus.Trigger(EventNames.AudioDeviceChangedEvent, device));
                }
            );

            _sdk.MediaDevice.AudioDeviceAdded = new AudioDeviceAddedEventHandler
            (
                (AudioDevice device) =>
                {
                    DolbyIOManager.QueueOnMainThread(() => EventBus.Trigger(EventNames.AudioDeviceAddedEvent, device));
                }
            );

            _sdk.InvalidTokenError = new InvalidTokenErrorEventHandler
            (
                (string reason, string description) => 
                {
                    DolbyIOManager.QueueOnMainThread(() => EventBus.Trigger(EventNames.InvalidTokenErrorEvent, $"${reason}: ${description}"));
                }
            );

            _sdk.SignalingChannelError = new SignalingChannelErrorEventHandler
            (
                (string message) => 
                {
                    DolbyIOManager.QueueOnMainThread(() => EventBus.Trigger(EventNames.InvalidTokenErrorEvent, $"${message}"));
                }
            );

            var userInfo = new UserInfo();
            userInfo.Name = flow.GetValue<string>(ParticipantName);

            _sdk.Session.OpenAsync(userInfo).Wait();

            yield return OutputTrigger;
        }
    }
}