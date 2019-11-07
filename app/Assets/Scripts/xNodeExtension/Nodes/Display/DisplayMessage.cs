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
                // Place message outside the object's bounding box
                SceneGameObject positionGameObject = GetInputValue<SceneGameObject>(nameof(objectPosition), null);
                parentMessageGameObject.transform.position = positionGameObject.gameObject.transform.position;

                // TODO: Mover dependiendo del bounding box (desde MESH o RENDER)
                BoxCollider collider = positionGameObject.gameObject.GetComponentInChildren<BoxCollider>();
                Vector3 offset = (collider == null) ? new Vector3(1, 1, 0) : collider.size;
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
