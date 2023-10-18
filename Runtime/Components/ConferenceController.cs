using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DolbyIO.Comms;
using DolbyIO.Comms.Unity;
using Unity.VisualScripting;

namespace DolbyIO.Comms.Unity
{
    [AddComponentMenu("Dolby.io Comms/Conference Controller", 2)]
    [Inspectable]
    public class ConferenceController : MonoBehaviour
    {
        private DolbyIOSDK _sdk = DolbyIOManager.Sdk;

        private List<VideoTrack> _tracks = new List<VideoTrack>();

        private List<VideoController> _videoControllers = new List<VideoController>();

        private VideoFrameHandler _cameraVideoFrameHandler = null;
        private VideoFrameHandler _screenShareVideoFrameHandler = null;

        [Tooltip("The conference alias to join.")]
        [SerializeField]
        private string _conferenceAlias;

        public string ConferenceAlias
        {
            get => _conferenceAlias;
            set => _conferenceAlias = value;
        }

        [Tooltip("The spatial audio style.")]
        public SpatialAudioStyle AudioStyle = SpatialAudioStyle.Shared;

        [Tooltip("The scale of the 3D Environment.")]
        public Vector3 Scale = new Vector3(1.0f, 1.0f, 1.0f);

        [Tooltip("Indicates if the conference should be joined automatically.")]
        public bool AutoJoin = false;

        public GameObject VideoDevice;
        public GameObject ScreenShareSource;

        void Start()
        {
            InitialiseConference();
        }

        public void InitialiseConference(){
            if (_sdk.IsInitialized)
            {
                if(_sdk.Conference == null){
                    Debug.LogWarning("Attempting to set delegates on conference but it does not exist");
                    return;
                }

                // _sdk.Conference.StatusUpdated += ConferenceStatusUpdated;
                _sdk.Conference.VideoTrackAdded += HandleVideoTrackAdded;
                _sdk.Conference.VideoTrackRemoved += HandleVideoTrackRemoved;
                _sdk.Conference.ParticipantUpdated += HandleParticipantUpdated;

                if (AutoJoin)
                {
                    Join();   
                }
            }else{
                Debug.LogWarning("SDK not initialized");
            }
        }

        public void Join()
        {
            try
            {
                ConferenceOptions options = new ConferenceOptions();
                options.Alias = _conferenceAlias;
                options.Params.SpatialAudioStyle = AudioStyle;

                JoinOptions joinOpts = new JoinOptions();
                joinOpts.Connection.SpatialAudio = true;
                joinOpts.Constraints.Audio = true;

                Conference conference = _sdk.Conference.CreateAsync(options).Result;
                Conference joinedConference = _sdk.Conference.JoinAsync(conference, joinOpts).Result;

                _sdk.Conference.SetSpatialEnvironmentAsync
                (
                    new System.Numerics.Vector3(Scale.x, Scale.y, Scale.z),  // Scale
                    new System.Numerics.Vector3(0.0f, 0.0f, 1.0f), // Forward
                    new System.Numerics.Vector3(0.0f, 1.0f, 0.0f),  // Up
                    new System.Numerics.Vector3(1.0f, 0.0f, 0.0f)   // Right
                ).Wait();
            }
            catch (DolbyIOException e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void Leave()
        {
            try
            {
                _sdk.Conference.LeaveAsync().Wait();
            }
            catch(DolbyIOException e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void Mute(bool muted)
        {
            try
            {
                _sdk.Audio.Local.MuteAsync(muted).Wait();
            }
            catch (DolbyIOException e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void MuteRemote(bool muted, string participantId)
        {
            try
            {
                _sdk.Audio.Remote.MuteAsync(muted, participantId).Wait();
            }
            catch (DolbyIOException e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void StartVideo()
        {
            VideoDevice? device = null;

            if (VideoDevice)
            {
                var dropdown = VideoDevice.GetComponent<VideoDeviceDropdown>();
                if (dropdown)
                {
                    device = dropdown.CurrentDevice;
                }
            }

            var controller = _videoControllers.Find(c => c.IsLocal == true);
            if (controller)
            {
                _cameraVideoFrameHandler = new VideoFrameHandler();
                _cameraVideoFrameHandler.Sink = controller.VideoRenderer;
            }

            DolbyIOManager.QueueOnMainThread(() =>
            {

                _sdk.Video.Local.StartAsync(device, _cameraVideoFrameHandler).ContinueWith
                (
                    t => Debug.LogError(t.Exception),
                    TaskContinuationOptions.OnlyOnFaulted
                );

            });
        }

        public void StopVideo()
        {
            try
            {
                _sdk.Video.Local.StopAsync().Wait();
            }
            catch (DolbyIOException e)
            {
                Debug.LogError("Failed to stop video." + e);
            }
        }

        public void StartScreenShare()
        {
            try
            {
                if (ScreenShareSource)
                {
                    var dropdown = ScreenShareSource.GetComponent<ScreenShareSourceDropdown>();
                    if (dropdown.CurrentSource.Id != 0)
                    {
                        var controller = _videoControllers.Find(c => c.IsLocal && c.IsScreenShare);
                        if (controller)
                        {
                            _screenShareVideoFrameHandler = new VideoFrameHandler();
                            _screenShareVideoFrameHandler.Sink = controller.VideoRenderer;
                        }

                        _sdk.Video.Local.StartScreenShareAsync(dropdown.CurrentSource, _screenShareVideoFrameHandler)
                            .ContinueWith
                            (
                                t => Debug.LogError(t.Exception),
                                TaskContinuationOptions.OnlyOnFaulted
                            );
                    }
                    else
                    {
                        throw new Exception("No source selected");
                    }
                }
            }
            catch (DolbyIOException e)
            {
                Debug.LogError("Failed to start screen share" + e);
            }
        }

        public void StopScreenShare()
        {
            try
            {
            }
            catch (DolbyIOException e)
            {
                Debug.LogError("Failed to stop screen share" + e);
            }
        }

        /// For performance reasons, instead of propagating to video controllers the various video track events,
        /// Controllers will register themself to the Conference Controller during the Awake phase.
        internal void RegisterVideoController(VideoController controller)
        {
            // Debug.Log("Registering VideoController for : "+controller.Filter);
            _videoControllers.Add(controller);
        }

        // private void ConferenceStatusUpdated(ConferenceStatus status, string conferenceId){
        //     Debug.Log("Conference status updated: " + status);
        // }

        private void HandleVideoTrackAdded(VideoTrack track)
        {
            // Debug.Log("VideoTrack Added: " + track.ParticipantId);
            _tracks.Add(track);
            UpdateVideoControllers().ContinueWith(t =>
            {
                Debug.LogWarning(t.Exception.Message);
            },
            TaskContinuationOptions.OnlyOnFaulted);
        }

        private void HandleVideoTrackRemoved(VideoTrack track)
        {
            // Debug.Log("VideoTrack Removed: " + track.ParticipantId);
            _tracks.Remove(track);
            UpdateVideoControllers().ContinueWith(t =>
            {
                Debug.LogWarning(t.Exception.Message);
            },
            TaskContinuationOptions.OnlyOnFaulted);
        }

        private void HandleParticipantUpdated(Participant p)
        {
            // Debug.Log("Participant updated: " + p.Status);

            if (ParticipantStatus.OnAir == p.Status)
            {
                UpdateVideoControllers().ContinueWith(t =>
                {
                    Debug.LogWarning(t.Exception.Message);
                },
                TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        private async Task UpdateVideoControllers()
        {
            if (!_sdk.Conference.IsInConference)
            {
                Debug.LogWarning("Attempting to update video controllers but not in conference");
                return;
            }
            Debug.Log("Updating VideoControllers");
            var participants = await _sdk.Conference.GetParticipantsAsync();
            if(participants == null){
                Debug.LogWarning("Attempting to update video controllers but no participants found");
                return;
            }

            if(_videoControllers.Count == 0){
                Debug.LogWarning("No video controllers found");
                return;
            }
            
            foreach(var c in _videoControllers)
            {
                string participantId = "";

                switch (c.FilterBy)
                {
                    case ParticipantFilter.ParticipantId:
                        participantId = c.Filter;
                        break;
                    case ParticipantFilter.Name:
                        Participant p = participants.Find(p => p.Info.Name.Equals(c.Filter));
                        if (p != null)
                        {
                            participantId = p.Id;
                        }
                        break;
                    case ParticipantFilter.ExternalId:
                        Participant p2 = participants.Find(p => p.Info.ExternalId.Equals(c.Filter));
                        if (p2 != null)
                        {
                            participantId = p2.Id;
                        }
                        break;
                }

                if (!String.IsNullOrEmpty(participantId) && !c.IsLocal)
                {
                    // Debug.Log("Updating track for participant: " + participantId);
                    VideoTrack track = _tracks.Find(t => t.ParticipantId.Equals(participantId));
                    if(track.Equals(null)){
                        Debug.LogWarning("No video track found for participant: " + participantId);
                        return;
                    }
                    c.UpdateTrack(track);
                }else{
                    if(c.IsLocal){
                        Debug.Log("Found local video controller for : "+c.Filter);
                    }else{
                        Debug.LogWarning("No participant ID found for player: " + c.Filter);
                    }
                }
            }

        }
    }
}

