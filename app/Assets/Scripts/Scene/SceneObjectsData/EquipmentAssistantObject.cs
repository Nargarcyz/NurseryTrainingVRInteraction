using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{

    [System.Serializable]
    public struct EquipmentAssistantData
    {

    }

    [CreateAssetMenu(fileName = "EquipmentAssistant", menuName = "NT/Scene/EquipmentAssistant")]
    public class EquipmentAssistantObject : SceneObject<EquipmentAssistantData>
    {
        public override List<string> GetCallbacks()
        {
            return new List<string>() { "OnEquipmentPut" };
        }
    }
}


