using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NT.Atributes;
using XNode;
namespace NT.Nodes.Flow
{
    public class WhileNode : NTNode
    {
        [NTInput] public bool condition;

        [NTInput] public DummyConnection flowIn;
        // [NTOutput] public DummyConnection loopBody;
        [NTOutput] public DummyConnection flowOut;

        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            bool cond = GetInputValue<bool>(nameof(this.condition), this.condition);
            while (cond)
            {

                cond = GetInputValue<bool>(nameof(this.condition), this.condition);
                Debug.Log($"Condition: {cond}");
                // if (cond)
                // {
                //     string portName = nameof(loopBody);
                //     NTNode node = GetNode(portName);
                //     NodePort port = GetPort(portName);
                //     var executeNode = new NodeExecutionContext { node = node, inputPort = port?.Connection, outputPort = port };
                //     // executeNode.node.Enter();
                //     yield return new YieldNode(executeNode);
                //     // executeNode.node.Exit();
                // }
                yield return null;

                // string portName = condition ? nameof(loopBody) : nameof(flowOut);

            }
        }

        public override string GetDisplayName()
        {
            return "While";
        }

        public override NodeExecutionContext NextNode(NodeExecutionContext context)
        {
            NTNode node = GetNode(nameof(flowOut));
            NodePort port = GetPort(nameof(flowOut));

            return new NodeExecutionContext { node = node, inputPort = port?.Connection, outputPort = port };
            // return base.NextNode(context);
        }
    }
}
