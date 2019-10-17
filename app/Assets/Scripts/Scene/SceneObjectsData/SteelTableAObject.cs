using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{
    [System.Serializable]
    public struct SteelTableAData
    {
        public List<SceneGameObjectReference> slots;
    }

    [CreateAssetMenu(fileName = "SteelTableAObject", menuName = "NT/Scene/SteelTableA")]
    public class SteelTableAObject : SceneObject<TableData>
    {
        // TODO: Revisar funcionamiento
        public override List<string> GetCallbacks()
        {
            return new List<string>() { "Cover On" };
        }

    }
}