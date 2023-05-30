using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

namespace DolbyIO.Comms.Unity
{
    [AddComponentMenu("Dolby.io Comms/Credentials/Access Token", 150)]
    public class AccessTokenCredentials : Credentials
    {
        public string AccessToken;

        public override string GetToken()
        {
            return AccessToken;
        }
    }
}


