using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{

    [System.Serializable]
    public struct GownData
    {

    }

    [CreateAssetMenu(fileName = "Gown", menuName = "NT/Scene/Gown")]
    public class GownSceneObject : SceneObject<GownData>
    {
        public override List<string> GetCallbacks()
        {
            return new List<string>() { "OnGownOpened", "OnGownClosed", "OnGownUsed" };
        }
    }
}

