using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NT.Atributes;
using XNode;
namespace NT.Nodes.Flow
{
    public class NotNode : NTNode
    {
        [NTInput] public bool input;
        [NTOutput] public bool output = true;
        public override string GetDisplayName()
        {
            return "Not";
        }

        public override object GetValue(NodePort port)
        {

            return !GetInputValue<bool>(nameof(this.input), this.input);
        }

        // public Action onStateChange;
        // public void SendSignal(NodePort output) {
        // 	// Loop through port connections
        // 	int connectionCount = output.ConnectionCount;
        // 	for (int i = 0; i < connectionCount; i++) {
        // 		NodePort connectedPort = output.GetConnection(i);

        // 		// Get connected ports logic node
        // 		LogicNode connectedNode = connectedPort.node as LogicNode;

        // 		// Trigger it
        // 		if (connectedNode != null) connectedNode.OnInputChanged();
        // 	}
        // 	if (onStateChange != null) onStateChange();
        // }

        // protected override void OnInputChanged() {
        // 	bool newInput = GetPort("input").GetInputValues<bool>().Any(x => x);

        // 	if (input != newInput) {
        // 		input = newInput;
        // 		output = !newInput;
        // 		SendSignal(GetPort("output"));
        // 	}
        // }

        // public override void OnCreateConnection(NodePort from, NodePort to) {
        // 	OnInputChanged();
        // }
    }

}
