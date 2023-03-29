using UnityEngine;
using System.Collections;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    public class VideoController : MonoBehaviour
    {
        [HideInInspector]
        public VideoRenderer Renderer;

        void Awake()
        {
            Renderer = new VideoRenderer(gameObject);
        }
    }
}