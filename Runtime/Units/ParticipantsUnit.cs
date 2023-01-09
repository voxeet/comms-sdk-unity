using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;
using DolbyIO.Comms;
using DolbyIO.Comms.Unity;

namespace DolbyIO.Comms.Unity
{
    [UnitTitle("Get Participants")]
    [UnitCategory("Dolby.io Comms")]
    public class ParticipantsUnit : Unit, IDolbyUnit
    {
        private DolbyIOSDK _sdk = DolbyIOManager.Sdk;

        [DoNotSerialize]
        public ValueInput ParticipantIds;

        [DoNotSerialize]
        public ValueOutput Participants;

        protected override void Definition()
        {
            ParticipantIds = ValueInput(nameof(ParticipantIds), new string[0]);
            Participants = ValueOutput<List<Participant>>(nameof(Participants), GetParticipants);
        }

        private List<Participant> GetParticipants(Flow flow)
        {
            var participants = _sdk.Conference.GetParticipantsAsync().Result;
            string[] ids = flow.GetValue<string[]>(ParticipantIds);

            if (ids.Length > 0)
            {
                participants = participants.FindAll(p => Array.Find(ids, id => p.Id.Equals(id)) != null);
            }

            flow.SetValue(Participants, participants);

            return participants;
        }
    }
}