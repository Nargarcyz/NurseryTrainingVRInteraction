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
        // TODO: Cambiar por subtipo contenedor
        [NTInput] public SceneGameObjectReference table;

        [NTInputSelect] public Dictionary<string, MutableTuple<Tools,Tools>> reglas;

        [HideInInspector]
        private List<Tools> options;

        private const string LIST_TOOLS = "#List#reglas#less_than#";

        [ContextMenu("Add rule")]
        public void AddRule()
        {
            string id = Guid.NewGuid().ToString();
            string fieldname = $"{LIST_TOOLS}{id}";
            var port = this.AddInstanceInput(typeof(MutableTuple<Tools, Tools>), fieldName: fieldname);
            reglas.Add(fieldname, new MutableTuple<Tools, Tools>(default,default));
        }

        public void DeleteInstanceInput(string portName)
        {
            this.RemoveInstancePort(portName);
            reglas.Remove(portName);
        }

        protected override void Init() {
            if (reglas == null)
            {
                reglas = new Dictionary<string, MutableTuple<Tools, Tools>>();
            }
            // Dynamic list of Tools
            UpdateOptionsList();
            SessionManager.Instance.OnSceneGameObjectsChanged.AddListener(UpdateOptionsList);
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            // Realizar lógica comprobación
            CheckSurfaceSatisfiesRules();
            yield return null;
        }

        #region Node Basic Configuration
        public override string GetDisplayName()
        {
            return "Tool Placement Comparer";
        }
        public override object GetValue(NodePort port)
        {
            return false;
        }

        public override int GetWidth()
        {
            return 300;
        }
        #endregion

        #region Tool Ordering Logic
        private void CheckSurfaceSatisfiesRules()
        {
            var sgo = GetNodeGameObject() as SteelTableASceneGameObject;    // TODO: Cambiar por clase en capa superior (p.e. ToolSurfaceSceneGameObject --> (hereda) SteelTableASceneGameObject)
            if (sgo != null)
            {
                GameObject surfaceGameobject = sgo.surface.gameObject;
                var tools = GetGameObjectChildren(surfaceGameobject);
                //children.Sort();
                // The transforms of the GameObjects has its anchor point in the center (look tool.transform.position)


                BoxCollider bc = surfaceGameobject.GetComponent<BoxCollider>();
                // Divide BoxCollider
                int rowSize = GetRowSize(sgo);
                bool rowOnX = bc.size.x >= bc.size.z;
                var rowColliders = DivideBoxColliderByRows(bc, rowSize, rowOnX).ToList();
                var rowGameObjects = ClassifyToolsByRows(tools, rowColliders, rowOnX);
                var rowTools = GetToolTypeFromGameObjects(rowGameObjects);

                // Check rules
                var results = CheckRulesInRows(reglas.Values.ToList(), rowTools);
                // Save results
                ExerciseFileLogger.Instance.LogResult("Comprobación de reglas en mesa quirúrgica", results);

                // Clean colliders (if wanted)  --> Could be created once at the beginning
                RemoveColliders(rowColliders);
            }
        }

        private SceneGameObject GetNodeGameObject()
        {
            //var nodeGraph = this.graph as SceneObjectGraph;
            //return SessionManager.Instance.GetSceneGameObject(nodeGraph.linkedNTVariable);
            var tableGameObject = GetInputValue<SceneGameObject>(nameof(table), null);
            return tableGameObject;
        }

        private IEnumerable<GameObject> GetGameObjectChildren(GameObject go)
        {
            foreach (Transform child in go.transform)
            {
                yield return child.gameObject;
            }
        }

        private void UpdateOptionsList()
        {
            var tools = SessionManager.Instance.GetSceneGameObjectsWithTag("Tool");
            options = tools.Cast<ITool>().Select(t => t.GetToolType()).Distinct().ToList();
        }

        private int GetRowSize(SceneGameObject sgo)
        {
            var dataValue = sgo.data.data.GetDefaultValue();
            if (dataValue is SteelTableAData d)
            {
                return d.rowSize;
            }
            else
            {
                var sceneObj = sgo.sceneObject as SceneObject<SteelTableAData>;
                var data = sceneObj.GetDefaultData();
                var value = (SteelTableAData)data.GetValue();
                return value.rowSize;
            }
        }

        private IEnumerable<BoxCollider> DivideBoxColliderByRows(BoxCollider bc, int rowSize, bool rowOnX)
        {
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

        private List<List<GameObject>> ClassifyToolsByRows (IEnumerable<GameObject> Tools, IEnumerable<BoxCollider> BoxColliders, bool rowOnX)
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


        private List<string> CheckRulesInRows(IEnumerable<MutableTuple<Tools, Tools>> rules, List<List<Tools>> rows)
        {
            int[,] unfulfilled = new int[rows.Count, rules.Count()];
            // Check how many times each rule is not accomplished
            foreach ((var rule, int i) in rules.Select((value, i) => (value, i)))
            {
                foreach ((var row, int j) in rows.Select((value, j) => (value, j)))
                {
                    int quantity = CountRuleUnaccomplishedInRow(rule, row);
                    unfulfilled[i, j] += quantity;
                }
            }
            // Return results as text
            List<string> results = new List<string>();
            foreach ((var row, int j) in rows.Select((value, j) => (value, j)))
            {
                foreach ((var rule, int i) in rules.Select((value, i) => (value, i)))
                {
                    if (unfulfilled[i, j] > 0)
                    {
                        results.Add(string.Format("Fila {0}\t\tRegla {1} no cumplida {2} veces.",
                            j,
                            rule.ToString(),
                            unfulfilled[i, j]));
                    }
                }
            }

            return results;
        }



        private List<List<Tools>> GetToolTypeFromGameObjects(List<List<GameObject>> toolGameObjects)
        {
            float t1 = Time.realtimeSinceStartup; // DEBUG
            List<List<Tools>> result = new List<List<Tools>>();
            foreach (var row in toolGameObjects)
            {
                List<Tools> temp = new List<Tools>();
                foreach (var gameObject in row)
                {
                    var toolScene = gameObject.GetComponentInChildren<ToolSceneGameObject>();
                    temp.Add(toolScene.toolType);
                }
                result.Add(temp);
            }
            float t2 = Time.realtimeSinceStartup; // DEBUG

            //var result2 = toolGameObjects.Select(x => x.Select(y => y.GetComponentInChildren<ToolSceneGameObject>().toolType));
            var result2 = toolGameObjects.Select(x => x.Select(y => y.GetComponentInChildren<ToolSceneGameObject>().toolType).ToList()).ToList();
            float t3 = Time.realtimeSinceStartup; // DEBUG

            Console.WriteLine(String.Format("Time Loop: {0}\nTime LINQ: {1}", t2-t1, t3-t2));
            //return result2;
            return result;
        }

        private int CountRuleUnaccomplishedInRow(MutableTuple<Tools,Tools> rule, List<Tools> row)
        {
            int count = 0;
            bool secondFound = false;
            // When Tool2 is found, each time we found Tool1 afterwards counts as error
            foreach(var tool in row)
            {
                if (secondFound)
                {
                    if (rule.Item1 == tool)
                    {
                        count++;
                    }
                }
                else
                {
                    secondFound = rule.Item2 == tool;
                }
            }
            return count;
        }

        #endregion
    }

}