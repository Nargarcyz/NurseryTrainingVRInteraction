using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes.SessionCore
{

    [System.Serializable]
    public class ToolPlacementComparer : NTNode
    {
        /*
        [NTInput] public float valueA;
        [NTInput] public float valueB;

        [NTOutput] public float result;
        */

        public override object GetValue(NodePort port)
        {
            // Further modifications when use of Lists is implemented



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