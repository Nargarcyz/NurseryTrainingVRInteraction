using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{

    [System.Serializable]
    public struct EquipmentAssistantData
    {
        public bool userInRange;
        public bool userInPosition;
        public bool gownCorrectlyPut;
        public bool glovesCorrectlyPut;
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


