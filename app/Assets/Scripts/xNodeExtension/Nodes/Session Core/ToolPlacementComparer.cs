using NT.Graph;
using NT.SceneObjects;
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
        [HideInInspector]
        private List<Tools> options;

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

            // Dynamic list of Tools
            UpdateOptionsList();
            SessionManager.Instance.OnSceneGameObjectsChanged.AddListener(UpdateOptionsList);
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            // LOGICA COMPROBACION
            var sgo = GetNodeGameObject() as SteelTableASceneGameObject;    // TODO: Cambiar por clase en capa superior (p.e. ToolSurfaceSceneGameObject --> (hereda) SteelTableASceneGameObject)
            if (sgo != null) {
                GameObject surfaceGameobject = sgo.surface.gameObject;
                List<GameObject> children = GetGameObjectChildren(surfaceGameobject);
                //children.Sort();
                // The transforms of the GameObjects has its anchor point in the center (look tool.transform.position)


                BoxCollider bc = surfaceGameobject.GetComponent<BoxCollider>();
                // Divide BoxCollider
                int rowSize = GetRowSize(sgo.sceneObject);
                var rowColliders = DivideColliderByRows(bc, rowSize);
                
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

        private void UpdateOptionsList()
        {
            var tools = SessionManager.Instance.GetSceneGameObjectsWithTag("Tool");
            options = tools.Cast<ITool>().Select(t => t.GetToolType()).Distinct().ToList();
        }


        private int GetRowSize(ISceneObject sgo)
        {
            var sceneobj = sgo as SceneObject<SteelTableAData>;
            var data = sceneobj.GetDefaultData();
            var value = (SteelTableAData)data.GetValue();
            return value.rowSize;
        }
        private List<BoxCollider> DivideColliderByRows(BoxCollider bc, int rowSize)
        {
            bool rowOnX = bc.size.x >= bc.size.z;
            float minCoord = rowOnX ? (bc.center.x - bc.size.x / 2) : (bc.center.z - bc.size.z / 2);
            float incrCoord = rowOnX ? (bc.size.x / rowSize) : (bc.size.z / rowSize);

            var bcList = new List<BoxCollider>();
            for (int i = 0; i < rowSize; i++)
            {
                BoxCollider row = bc.gameObject.AddComponent<BoxCollider>();
                if (rowOnX) {
                    row.center = new Vector3(minCoord + (i*incrCoord) + (incrCoord/2), bc.center.y, bc.center.z);
                    row.size = new Vector3(bc.size.x / rowSize, bc.size.y, bc.size.z);
                } else
                {
                    row.center = new Vector3(bc.center.x, bc.center.y, minCoord + (i * incrCoord) + (incrCoord / 2));
                    row.size = new Vector3(bc.size.x, bc.size.y, bc.size.z / rowSize);
                }
                bcList.Add(row);
            }

            return bcList;
        }

        // Requires you to do all the transformations (centre, scale etc) on the parent gameobject
        // rather than the collider component (so the collider centre is 0,0,0 and scale is 1,1,1)
        private bool ColliderContainsPoint(Transform ColliderTransform, Vector3 Point)
        {
            Vector3 localPos = ColliderTransform.InverseTransformPoint(Point);
            // Mirar size para hacerlo genérico?
            return (Math.Abs(localPos.x) < 0.5f && Math.Abs(localPos.y) < 0.5f && Math.Abs(localPos.z) < 0.5f);
        }

    }

}