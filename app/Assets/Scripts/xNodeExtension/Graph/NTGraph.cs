using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using NT.Nodes.Messages;
using NT.Variables;
using UnityEditor;
using UnityEngine;

using XNode;

namespace NT.Graph
{
    public class NTGraph : NodeGraph
    {
        public List<NodeGroupGraph> packedNodes = new List<NodeGroupGraph>();
        [NonSerialized] public IVariableDelegate variableDelegate;

        public virtual List<string> GetCallbacks()
        {
            return new List<string>();
        }

        public List<CallbackNode> callbackNodes = new List<CallbackNode>();

        public Dictionary<string, List<CallbackNode>> callbackNodesDict = new Dictionary<string, List<CallbackNode>>();

        public override Node AddNode(System.Type type)
        {
            Node node = base.AddNode(type);
            if (node is CallbackNode)
            {
                callbackNodes.Add((CallbackNode)node);
            }
            return node;
        }


        public void StartExecution()
        {
            MessageSystem.onMessageSent -= MessageRecieved;
            MessageSystem.onMessageSent += MessageRecieved;
            Debug.Log(name);
            GenerateCallbackDict();

            if (packedNodes != null)
            {
                foreach (var pack in packedNodes)
                {
                    pack.StartExecution();
                }
            }
        }

        public virtual void GenerateCallbackDict()
        {
            callbackNodesDict = new Dictionary<string, List<CallbackNode>>();

            callbackNodes = new List<CallbackNode>();

            foreach (Node n in nodes)
            {
                if (n is CallbackNode)
                {
                    callbackNodes.Add((CallbackNode)n);
                }
            }

            foreach (CallbackNode cn in callbackNodes)
            {
                if (cn != null && !string.IsNullOrEmpty(cn.GetCallbackKey()))
                {
                    List<CallbackNode> callbacksInKey = new List<CallbackNode>();

                    if (callbackNodesDict.ContainsKey(cn.GetCallbackKey()))
                    {
                        callbacksInKey = callbackNodesDict[cn.GetCallbackKey()];
                        callbacksInKey.Add(cn);
                        callbackNodesDict[cn.GetCallbackKey()] = callbacksInKey;

                        Debug.Log("Add callback " + cn.GetCallbackKey() + " __ " + name);
                    }
                    else
                    {
                        callbacksInKey.Add(cn);
                        callbackNodesDict[cn.GetCallbackKey()] = callbacksInKey;
                        Debug.Log("<color=green> Add callback " + cn.GetCallbackKey() + " __ " + name + "</color>");
                    }
                }
            }
        }

        public virtual void MessageRecieved(string message)
        {
            Debug.Log("<color=magenta> Message recieved!   " + message + " on graph" + name + "</color>");

            // FIX FOR CALLBACKS ON SCENEGAMEOBJECTS NOT PROPERLY TRIGGERING
            var key = message;
            // if (this.GetType() == typeof(SceneGraph))
            // {
            //     key = message;
            // }
            // else 
            // if (this.GetType() == typeof(SceneObjectGraph))
            // {
            //     key = ((SceneObjectGraph)this).linkedNTVariable + message;
            // }
            // FIX FOR CALLBACKS ON SCENEGAMEOBJECTS NOT PROPERLY TRIGGERING
            if (!string.IsNullOrEmpty(key) && callbackNodesDict.ContainsKey(key))
            {
                List<CallbackNode> nodesToExecute = callbackNodesDict[key];
                foreach (CallbackNode cn in nodesToExecute)
                {
                    CoroutineRunner.Instance.StartCoroutine(StartExecutionFlow(cn));
                }
            }
        }

        public void AddGroupedNodes(NodeGroupGraph group)
        {
            packedNodes.Add(group);
        }

        public void RemoveGroupedNodes(NodeGroupGraph group)
        {
            for (int i = group.nodes.Count - 1; i >= 0; i--)
            {
                group.RemoveNode(group.nodes[i]);
            }

            packedNodes.Remove(group);
        }

        public Node DuplicateGroupedNodes(NodeGroupGraph group)
        {
            // TODO: Duplicate correctly

            return null;
        }

        public IEnumerator StartExecutionFlow(CallbackNode callbackNode)
        {
            NodeExecutionContext nodeExecutionContext = new NodeExecutionContext { node = callbackNode };

            while (nodeExecutionContext.node != null)
            {

                Debug.Log("<color=green> Execute node:  " + nodeExecutionContext.node + " ... GRAPH: " + name + "</color>");
                nodeExecutionContext.node.Enter();

                yield return new YieldNode(nodeExecutionContext);

                Debug.Log("<color=red> Finished node:  " + nodeExecutionContext.node + "</color>");

                yield return new WaitForSeconds(0.25f);

                nodeExecutionContext.node.Exit();

                nodeExecutionContext = nodeExecutionContext.node.NextNode(nodeExecutionContext);
            }

            yield return null;
        }

        Dictionary<Type, Type> dataToNtVatiable = new Dictionary<Type, Type>();

        public Type GetVariableFor(Type t)
        {
            if (dataToNtVatiable.ContainsKey(t)) return dataToNtVatiable[t];


            foreach (Type nodeType in ReflectionUtilities.variableNodeTypes)
            {
                if (nodeType.IsGenericTypeDefinition)
                {
                    continue;
                }

                Type dataType = ((NTVariable)Activator.CreateInstance(nodeType)).GetDataType();

                if (dataType == t)
                {
                    dataToNtVatiable.Add(t, nodeType);
                    return nodeType;
                }
            }

            Debug.Log("Not found??");
            return null;
        }

    }
}
