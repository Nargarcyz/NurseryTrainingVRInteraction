using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NT.Atributes;
using NT.SceneObjects;

namespace NT.Nodes.Display {
    
    public class DisplayMessage : FlowNode {

        public string messageText;

        [NTInput] public SceneGameObjectReference objectPosition;
        

        public object GetValue() {
            return null;
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            GameObject messageGameObject = GameObject.Find("ShowMessage/Bocadillo/MessageText");
            GameObject showMessageGameObject = getGameObjectParent(getGameObjectParent(messageGameObject));

            bool visible = !string.IsNullOrEmpty(messageText);
            showMessageGameObject.SetActive(visible);

            if (visible)
            {
                Text textComponent = messageGameObject.GetComponent<Text>();
                textComponent.text = messageText;
                // Centered in the object, message outside its bounding box
                SceneGameObject positionGameObject = GetInputValue<SceneGameObject>(nameof(objectPosition), null);
                showMessageGameObject.transform.position = positionGameObject.gameObject.transform.position;

                /*showMessageGameObject.transform.position = objectPosition.reference.transform.position;
                messageGameObject.transform.position = Vector3.zero;
                    // TODO: Get corresponding bounding box
                Vector3 boundingBox = new Vector3(100, 100, 0);
                messageGameObject.transform.Translate(boundingBox);*/
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
