using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NT.Atributes;

namespace NT.Nodes.Flow
{
    public class WhileNode : FlowNode
    {
        [NTInput] public bool condition;


        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            bool cond = GetInputValue<bool>(nameof(this.condition), this.condition);
            while (cond)
            {
                yield return null;
                Debug.Log($"Condition: {cond}");
                cond = GetInputValue<bool>(nameof(this.condition), this.condition);
            }
        }

        public override string GetDisplayName()
        {
            return "While";
        }

        public override NodeExecutionContext NextNode(NodeExecutionContext context)
        {
            return base.NextNode(context);
        }
    }
}
