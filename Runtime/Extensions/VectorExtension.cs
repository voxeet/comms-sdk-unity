using UnityEngine;
using System.Numerics;

namespace DolbyIO.Comms.Unity
{
    public static class UnityVector3Extensions
    {
        public static System.Numerics.Vector3 ToDolbyVector3(this UnityEngine.Vector3 v)
        {
            return new System.Numerics.Vector3(v.x, v.y, v.z);
        }
    }
}