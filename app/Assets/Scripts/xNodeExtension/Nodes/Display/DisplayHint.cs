using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NT.Nodes.Display
{
    public class DisplayHint : FlowNode
    {
        public string hintText;


        public object GetValue()
        {
            return null;
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            GameObject hintGameObject = GameObject.Find("Session Hints/Panel/Hint Text");
            Text textComponent = hintGameObject.GetComponent<Text>();
            textComponent.text = hintText;

            yield return null;
        }

        public override string GetDisplayName()
        {
            return "Display Hint";
        }
    }
}