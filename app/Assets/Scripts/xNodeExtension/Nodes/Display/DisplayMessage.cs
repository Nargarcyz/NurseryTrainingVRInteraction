using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NT.Atributes;
using NT.SceneObjects;
using TMPro;

namespace NT.Nodes.Display {
    
    public class DisplayMessage : FlowNode {

        public string messageText;

        [NTInput] public SceneGameObjectReference objectPosition;
        //[Input(ShowBackingValue.Never, ConnectionType.Override)] public SceneGameObjectReference objectPosition;
        

        public object GetValue() {
            return null;
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            GameObject messageGameObject = GameObject.Find("ShowMessage/Bocadillo/MessageText");
            GameObject showMessageGameObject = getGameObjectParent(getGameObjectParent(messageGameObject));

            // Make object visible by modifying scale
            bool visible = !string.IsNullOrEmpty(messageText);
            //Vector3 scale = visible ? Vector3.one : Vector3.zero;
            //showMessageGameObject.transform.localScale = scale;

            if (visible)
            {
                showMessageGameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                TextMeshPro textComponent = messageGameObject.GetComponent<TextMeshPro>();
                textComponent.text = messageText;
                // Place message outside the object's bounding box
                SceneGameObject positionGameObject = GetInputValue<SceneGameObject>(nameof(objectPosition), null);
                showMessageGameObject.transform.position = positionGameObject.gameObject.transform.position;

                // TODO: Mover dependiendo del bounding box (desde MESH o RENDER)
                showMessageGameObject.transform.Translate(1, 1, 0);
                // showMessageGameObject.transform.Translate(boundingBox);*/
            }
            else
            {
                showMessageGameObject.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
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
