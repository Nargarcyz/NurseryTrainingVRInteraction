using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct MetallicBoxData{
        public bool canBeOpened;
        public List<SceneGameObjectReference> toolsList;
    }

    [CreateAssetMenu(fileName = "MetallicBoxSceneObject", menuName = "NT/Scene/MetallicBox")]
    public class MetallicBoxSceneObject : SceneObject<MetallicBoxData> {
        
        public override List<string> GetCallbacks()
        {
            return new List<string>() { "Grabbed" };
        }

        public override bool CanHoldItem(SceneGameObject obj)
        {
            return true;
        }

        public override void HoldItem(SceneGameObject obj)
        {
            obj.gameObject.SetActive(true);
        }
    }
}