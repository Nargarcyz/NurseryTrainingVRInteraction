using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{

    public struct HandInteractableData
    {

    }
    [CreateAssetMenu(fileName = "HandInteractable", menuName = "NT/Scene/HandInteractable")]
    public class HandInteractableObject : SceneObject<HandInteractableData>
    {
        public override List<string> GetCallbacks()
        {
            return new List<string>() { "OnUsed", "OnGrabbed" };
        }
    }
}
