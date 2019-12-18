using NT.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace NT.Nodes.SessionCore
{

    [System.Serializable]
    public class ToolPlacementComparer : FlowNode
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

        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            // LOGICA COMPROBACION
            var sgo = GetNodeGameObject() as SteelTableASceneGameObject;    // TODO: Cambiar por clase en capa superior (p.e. ToolSurfaceSceneGameObject --> (hereda) SteelTableASceneGameObject)
            if (sgo != null) {

                List<GameObject> children = GetGameObjectChildren(sgo.transform.gameObject);
                //children.Sort();

                
                /*var currentGameobject = sgo.gameObject;
                BoxCollider bc = currentGameobject.GetComponentInParent<BoxCollider>();*/
            }

            yield return null;
        }

        public override object GetValue(NodePort port)
        {
            return false;
        }


        private SceneGameObject GetNodeGameObject()
        {
            var nodeGraph = this.graph as SceneObjectGraph;
            return SessionManager.Instance.GetSceneGameObject(nodeGraph.linkedNTVariable);
        }

        private List<GameObject> GetGameObjectChildren(GameObject go)
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in go.transform)
            {
                children.Add(child.gameObject);
            }
            return children;
        }

        public override string GetDisplayName()
        {
            return "Tool Placement Comparer";
        }


    }

}