using System.Collections;
using UnityEngine;
using NT.Atributes;
using NT.SceneObjects;
using TMPro;

namespace NT.Nodes.Display {
    
    public class DisplayMessage : FlowNode {

        [NTInput] public string messageText;

        [NTInput] public SceneGameObjectReference objectPosition;
        //[Input(ShowBackingValue.Never, ConnectionType.Override)] public SceneGameObjectReference objectPosition;
        

        public object GetValue() {
            return null;
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            GameObject messageGameObject = GameObject.Find("ShowMessage/Bocadillo/MessageText");
            GameObject parentMessageGameObject = getGameObjectParent(getGameObjectParent(messageGameObject));

            // Check if message is empty
            string message = GetInputValue<string>(nameof(this.messageText), this.messageText);
            bool visible = !string.IsNullOrEmpty(message);
            // Modify visibility by changing scale
            Vector3 scale = visible ? Vector3.one : Vector3.zero;
            parentMessageGameObject.transform.localScale = scale;

            if (visible)
            {
                TextMeshPro textComponent = messageGameObject.GetComponent<TextMeshPro>();
                textComponent.text = message;
                // Place message outside the object's bounding box, towards the scene center (to avoid walls collision)
                SceneGameObject positionGameObject = GetInputValue<SceneGameObject>(nameof(objectPosition), null);
                BoxCollider collider = positionGameObject.gameObject.GetComponentInChildren<BoxCollider>();
                parentMessageGameObject.transform.position = positionGameObject.gameObject.transform.position;
                //parentMessageGameObject.transform.position = positionGameObject.transform.TransformDirection(collider.center);
                Vector3 unitXY = new Vector3(1, 1, 0);
                Vector3 offset = (collider == null) ? unitXY : collider.transform.TransformDirection(collider.size) * 1.5f;//positionGameObject.gameObject.transform.
                //offset.x = 0;
                //offset.y = 0;
                offset.z = 0;
                //offset = Vector3.Project(offset, Vector3.Cross(unitXY, collider.center));
                parentMessageGameObject.transform.Translate(offset);

                // Rotación de seguimiento alrededor del objeto de posición??
            }

            yield return null;
        }

        private GameObject getGameObjectParent(GameObject g)
        {
            return g.transform.parent.gameObject;
        }

        public override string GetDisplayName(){
            return "Display Message";
        }
    }
}
