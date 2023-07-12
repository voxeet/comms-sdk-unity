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

        private Material  _material;
        private Texture2D _texture;

        public VideoRenderer(Material material)
        {
            Width = Height = 0;
            _material = material;
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

            _material.mainTexture = _texture;
        }

        public void Clear()
        {
            _material.mainTexture = Texture2D.blackTexture;
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