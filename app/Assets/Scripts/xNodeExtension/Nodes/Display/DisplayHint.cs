using NT.Atributes;
using System.Collections;
using TMPro;
using UnityEngine;
using HMS;
namespace NT.Nodes.Display
{
    public class DisplayHint : FlowNode
    {
        [NTInput] public string hintText;


        public object GetValue()
        {
            return null;
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            // GameObject hintGameObject = GameObject.Find("Session Hints/Panel/Hint Text");
            // Debug.Log("Hint object = ");
            // Debug.Log(hintGameObject);
            // TextMeshPro textComponent = hintGameObject.GetComponent<TextMeshPro>();
            string hint = GetInputValue<string>(nameof(this.hintText), this.hintText);
            // textComponent.text = hint;
            Debug.Log("<color=red>Sending Hint</color>");
            HintMessageSystem.SendHint(hint);
            yield return null;
        }

        public override string GetDisplayName()
        {
            return "Display Hint";
        }
    }
}