using NT.Nodes.SessionCore;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIButtonDeleteDynamic : MonoBehaviour
{
    public GameObject dynamicPort;

    public void DeleteDynamicElement()
    {
        string portName = dynamicPort.name;
        var currentNode = SessionManager.Instance.sceneGraph.nodes.Where(n => n.ports.Any(p => p.Key.Equals(portName))).FirstOrDefault();
        var baseNode = this.gameObject.GetComponentInParent<UGUIBaseNode>();

        if (currentNode is ToolPlacementComparer tpc)
        {
            tpc.DeleteInstanceInput(portName);
            // Visual Update Runtime Graph
            baseNode.GetRuntimeGraph().Refresh(); // TODO: Revisar borrado datos en los seleccionables!! (guardado GUIProperty)
        }
        else
        {
            Debug.LogError("Node not found when trying to delete port");
        }
    }
}
