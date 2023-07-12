using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

namespace DolbyIO.Comms.Unity
{
    public abstract class Credentials : MonoBehaviour, ICredentials
    {
        public abstract string GetToken();
    }
}
