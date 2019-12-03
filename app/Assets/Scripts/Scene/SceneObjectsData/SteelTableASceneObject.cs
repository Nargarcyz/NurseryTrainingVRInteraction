using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{
    [System.Serializable]
    public struct SteelTableAData
    {
        public int rowSize;
        [Range(1, 10)]
        public int colSize;
    }

    [CreateAssetMenu(fileName = "SteelTableASceneObject", menuName = "NT/Scene/SteelTableA")]
    public class SteelTableASceneObject : SceneObject<SteelTableAData>
    {
        // TODO: Revisar funcionamiento
        public override List<string> GetCallbacks()
        {
            return new List<string>() { "Cover On" };
        }

    }
}