using System.Collections;
using UnityEngine;
using NT.Atributes;
using NT.SceneObjects;
using TMPro;
using UnityEngine.Animations;
using VRTK;
namespace NT.Nodes.Display
{

    public class DisplayMessage : FlowNode
    {

        [NTInput] public string messageText;

        [NTInput] public SceneGameObjectReference objectPosition;
        //[Input(ShowBackingValue.Never, ConnectionType.Override)] public SceneGameObjectReference objectPosition;


        public object GetValue()
        {
            return null;
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            string message = GetInputValue<string>(nameof(this.messageText), this.messageText);
            bool visible = !string.IsNullOrEmpty(message);
            if (visible)
            {
                GameObject messageObj = null;
                var previousMessage = GameObject.Find("NewShowMessage(Clone)");
                if (previousMessage)
                {
                    GameObject.DestroyImmediate(previousMessage.gameObject);
                }
                messageObj = GameObject.Instantiate(Resources.Load("UI/NewShowMessage")) as GameObject;

                TextMeshProUGUI text = messageObj.GetComponentInChildren<TextMeshProUGUI>();
                text.text = message;
                var messageScript = messageObj.GetComponent<MessageObjectTracking>();
                messageScript.trackedObject = VRTK_SDKManager.GetLoadedSDKSetup().actualHeadset;
                messageScript.parentObject = GetInputValue<SceneGameObject>(nameof(objectPosition), null).gameObject;


            }
            yield return null;
        }

        private GameObject getGameObjectParent(GameObject g)
        {
            return g.transform.parent.gameObject;
        }

        public override string GetDisplayName()
        {
            return "Display Message";
        }
    }
}
