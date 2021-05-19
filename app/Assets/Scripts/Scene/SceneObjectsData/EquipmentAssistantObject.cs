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
        public bool gownOn;
        public bool gownCorrectlyPut;
        public bool glovesCorrectlyPut;
        public bool rightGloveOn;
        public bool leftGloveOn;

        public void DisplayValues()
        {
            Debug.Log($"userInRange = {userInRange}");
            Debug.Log($"userInPosition = {userInPosition}");
            Debug.Log($"gownOn = {gownOn}");
            Debug.Log($"gownCorrectlyPut = {gownCorrectlyPut}");
            Debug.Log($"glovesCorrectlyPut = {glovesCorrectlyPut}");
            Debug.Log($"rightGloveOn = {rightGloveOn}");
            Debug.Log($"leftGloveOn = {leftGloveOn}");
        }
    }

    [CreateAssetMenu(fileName = "EquipmentAssistant", menuName = "NT/Scene/EquipmentAssistant")]
    public class EquipmentAssistantObject : SceneObject<EquipmentAssistantData>
    {
        public override List<string> GetCallbacks()
        {
            return new List<string>() { "UserInRange", "UserLeft", "EquipmentCorrectlyPut", "BadEquipmentProcedure" };
        }
    }
}


