using UnityEngine;
using System.Collections;
using DolbyIO.Comms;

namespace DolbyIO.Comms.Unity
{
    public class VideoController : MonoBehaviour
    {
        [HideInInspector]
        public VideoRenderer Renderer;

        public bool Show = false;

        void Awake()
        {
            Renderer = new VideoRenderer(gameObject);
        }

        private void Update()
        {
            gameObject.GetComponent<Renderer>().enabled = Show;
        }
    }
}