using NT.Atributes;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace NT.Nodes.SessionCore
{

    [System.Serializable]
    public class ToolPlacementComparer : NTNode
    {
        [NTInputSelect] public Tools tool1;
        [NTInputSelect] public Tools tool2;

        //[HideInInspector] public List<Tools> rules;

        /*
        [NTOutput] public float result;
        */
        protected override void Init() {
            /*
            this.AddInstanceInput(typeof(string), fieldName: "myDynamicInput");
            this.AddInstanceOutput(typeof(int), fieldName: "myDynamicOutput");
            */
        }

        public override object GetValue(NodePort port)
        {
            /*
            float val1 = GetInputValue<float>(nameof(valueA), this.valueA);
            float val2 = GetInputValue<float>(nameof(valueB), this.valueB);

            result = val1 + val2;

            return result;*/
            return false;
        }

        public override string GetDisplayName()
        {
            return "Tool Placement Comparer";
        }


    }

}