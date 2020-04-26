using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{

    [System.Serializable]
    public struct TeammateData
    {

    }

    [CreateAssetMenu(fileName = "TeammateObject", menuName = "NT/Scene/Teammate")]
    public class TeammateObject : SceneObject<TeammateData>
    {

        /*public override List<string> GetCallbacks()
        {
            return new List<string>() { "Cover On" };
        }*/

    }
}