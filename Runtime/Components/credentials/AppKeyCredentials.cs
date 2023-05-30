using UnityEngine;
using System.Collections;

namespace DolbyIO.Comms.Unity
{
    [AddComponentMenu("Dolby.io Comms/Credentials/App Key", 151)]
    public class AppKeyCredentials : Credentials
    {
        [Tooltip("Dolby.io App Key.")]
        public string AppKey;

        [Tooltip("Dolby.io App Secret.")]
        public string AppSecret;

        public override string GetToken()
        {
            return DolbyIOManager.GetToken(AppKey, AppSecret).Result;
        }
    }
}



