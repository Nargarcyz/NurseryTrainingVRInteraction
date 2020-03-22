using NT.Atributes;
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
    public class ToolPlacementComparer : FlowNode, IUGUIDynamicListNode
    {

        [NTInputSelect] public Dictionary<string, MutableTuple<Tools,Tools>> reglas;

        [HideInInspector]
        private List<NodePort> reglasPorts;

        [HideInInspector]
        private List<Tools> options;

        private const string LIST_TOOLS = "#List#reglas#less_than#";

        [ContextMenu("Add rule")]
        public void AddRule()
        {
            string id = Guid.NewGuid().ToString();
            string fieldname = $"{LIST_TOOLS}{id}";
            var port = this.AddInstanceInput(typeof(MutableTuple<Tools, Tools>), fieldName: fieldname);

            if (reglas == null)
            {
                reglas = new Dictionary<string, MutableTuple<Tools, Tools>>();
            }
            reglas.Add(fieldname, new MutableTuple<Tools, Tools>(default,default));
            //reglasPorts.Add(port);
        }

        public void DeleteInstanceInput(string portName)
        {
            this.RemoveInstancePort(portName);

            reglas.Remove(portName);
            //int elementIndex = reglasPorts.FindIndex(p => p.fieldName == portName);
            //reglasPorts.RemoveAt(elementIndex);
        }


        protected override void Init() {
            //if (reglas == null)
            //{
            //    reglas = new Dictionary<string, MutableTuple<Tools, Tools>>();
            //}

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
                var tools = GetGameObjectChildren(surfaceGameobject);
                //children.Sort();
                // The transforms of the GameObjects has its anchor point in the center (look tool.transform.position)


                BoxCollider bc = surfaceGameobject.GetComponent<BoxCollider>();
                // Divide BoxCollider
                int rowSize = GetRowSize(sgo.sceneObject);
                var rowColliders = DivideBoxColliderByRows(bc, rowSize);
                var rowTools = ClassifyToolsByRows(tools, rowColliders);


                RemoveColliders(rowColliders);
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

        private IEnumerable<GameObject> GetGameObjectChildren(GameObject go)
        {
            foreach (Transform child in go.transform)
            {
                yield return child.gameObject;
            }
        }

        public override string GetDisplayName()
        {
            return "Tool Placement Comparer";
        }

        #region Tool Ordering Logic

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
        private IEnumerable<BoxCollider> DivideBoxColliderByRows(BoxCollider bc, int rowSize)
        {
            bool rowOnX = bc.size.x >= bc.size.z;
            float minCoord = rowOnX ? (bc.center.x - bc.size.x / 2) : (bc.center.z - bc.size.z / 2);
            float incrCoord = rowOnX ? (bc.size.x / rowSize) : (bc.size.z / rowSize);

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
                yield return row;
            }
        }

        private List<List<GameObject>> ClassifyToolsByRows (IEnumerable<GameObject> Tools, IEnumerable<BoxCollider> BoxColliders)
        {
            var rowSize = Enumerable.Count(BoxColliders);
            // Initialize result
            List<List<GameObject>> result = new List<List<GameObject>>();
            for (int i = 0; i < rowSize; i++)
            {
                result.Add(new List<GameObject>());
            }

            // Classify tools
            foreach (var tool in Tools)
            {
                for (int i = 0; i < rowSize; i++)
                {
                    BoxCollider bc = BoxColliders.ElementAt(i);
                    Collider toolCollider = tool.GetComponentInChildren<Collider>();
                    if (BoxColliderContainsPoint(bc, toolCollider.bounds.center, true))
                    {
                        result[i].Add(tool);
                        break;
                    }
                }
            }

            // Sort tools by surface local axis
            foreach (var row in result)
            {
                BoxCollider bc = BoxColliders.ElementAt(result.IndexOf(row));
                bool rowOnX = bc.size.x >= bc.size.z;
                Transform trans = bc.transform;
                row.Sort((a, b) => rowOnX ? trans.InverseTransformPoint(a.transform.position).x.CompareTo(trans.InverseTransformPoint(b.transform.position).x)
                                            : trans.InverseTransformPoint(a.transform.position).z.CompareTo(trans.InverseTransformPoint(b.transform.position).z));
            }

            return result;
        }


        private bool BoxColliderContainsPoint(BoxCollider boxCollider, Vector3 Point, bool ignoreY)
        {
            Vector3 localPos = boxCollider.transform.InverseTransformPoint(Point);
            Vector3 halfSize = boxCollider.size / 2;
            // Revisar < o <=
            bool result = (Math.Abs(localPos.x) < halfSize.x && Math.Abs(localPos.z) < halfSize.z);
            return ignoreY ? result : (result && Math.Abs(localPos.y) < halfSize.y);
        }

        private void RemoveColliders(IEnumerable<Collider> colliders)
        {
            foreach (var collider in colliders)
            {
                Component.Destroy(collider);
            }
        }

        #endregion
    }

}