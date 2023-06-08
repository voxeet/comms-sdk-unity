using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    public enum ParticipantFilter
    {
        Name,
        ParticipantId,
        ExternalId
    }

    [AddComponentMenu("Dolby.io Comms/Video/Video Controller", 350)]
    public class VideoController : MonoBehaviour
    {
        private DolbyIOSDK _sdk = DolbyIOManager.Sdk;

        [HideInInspector]
        public VideoRenderer VideoRenderer;

        public ConferenceController Conference;

        [Tooltip("Filter the video to display by.")]
        public ParticipantFilter FilterBy = ParticipantFilter.ParticipantId;

        [Tooltip("The filter value.")]
        public string Filter;

        [Tooltip("Whether to display local or remote video.")]
        public bool IsLocal = false;

        [Tooltip("Wether to display screenshare.")]
        public bool IsScreenShare = false;
        
        private VideoTrack _videoTrack;

        void Awake()
        {
            if (Conference)
            {
                Conference.RegisterVideoController(this);
            }
        }

        void Start()
        {
            var renderer = gameObject.GetComponent<Renderer>();

            if (renderer)
            {
                VideoRenderer = new VideoRenderer(renderer.material);
            }
        }

        private void Update()
        {

        }

        internal void UpdateTrack(VideoTrack track)
        {
            if (!_videoTrack.Equals(track))
            {
                _videoTrack = track;

                if (track.ParticipantId != null)
                {
                    _sdk.Video.Remote.SetVideoSinkAsync(track, VideoRenderer).ContinueWith(t =>
                    {
                        Debug.LogWarning(t.Exception.Message);
                    },
                    TaskContinuationOptions.OnlyOnFaulted);
                }
            }
        }
    }
}