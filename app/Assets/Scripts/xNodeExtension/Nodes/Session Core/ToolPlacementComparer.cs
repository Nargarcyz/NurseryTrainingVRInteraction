using NT.Atributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace NT.Nodes.SessionCore
{

    [System.Serializable]
    public class ToolPlacementComparer : NTNode
    {
        //[NTInputSelect] public List<Tools> toolList1;
        //[NTInputSelect] public List<Tools> toolList2;

        [HideInInspector]
        private List<Tuple<NodePort, NodePort>> rules;

        [ContextMenu("Add rule")]
        public void AddRule()
        {
            int i = rules.Count;
            NodePort np1 = this.AddInstanceInput(typeof(Tools), fieldName:$"Less{i}");
            NodePort np2 = this.AddInstanceInput(typeof(Tools), fieldName:$"Great{i}");
            this.
            rules.Add(new Tuple<NodePort, NodePort> (np1,np2));
        }

        /*
        [NTOutput] public float result;
        */
        protected override void Init() {
            rules = new List<Tuple<NodePort, NodePort>>();
            AddRule();
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