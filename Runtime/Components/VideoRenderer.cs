using System;
using DolbyIO.Comms;
using DolbyIO.Comms.Unity;
using UnityEngine;

namespace DolbyIO.Comms.Unity
{
    public class VideoRenderer : VideoSink
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private GameObject _display;
        private Texture2D _texture;

        public VideoRenderer(GameObject display)
        {
            Width = Height = 0;
            _display = display;
        }

        private void createTexture()
        {
            if (!_texture)
            {
                _texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false, true);
            }
            else
            {
                _texture.Reinitialize(Width, Height, TextureFormat.ARGB32, false);
            }
            _display.GetComponent<Renderer>().material.mainTexture = _texture;
        }

        public void Clear()
        {
            _display.GetComponent<Renderer>().material.mainTexture = Texture2D.blackTexture;
            Width = 0;
            Height = 0;
        }

        public void Render(VideoFrame frame)
        {
            DolbyIOManager.QueueOnMainThread(() =>
            {
                if (frame.Width != Width || frame.Height != Height)
                {
                    Width = frame.Width;
                    Height = frame.Height;

                    createTexture();
                }

                if (_texture)
                {
                    _texture.LoadRawTextureData(frame.DangerousGetHandle(), frame.Width * frame.Height * 4);
                    _texture.Apply();
                }

                frame.Dispose();
            });
        }

        public override void OnFrame(VideoFrame frame)
        {
            Render(frame);
        }
    }
}