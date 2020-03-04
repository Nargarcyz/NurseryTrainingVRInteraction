using NT.Nodes.SessionCore;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIButtonDeleteDynamic : MonoBehaviour
{
    public GameObject dynamicPort;

    public void DeleteDynamicElement()
    {
        //var port = dynamicPort.GetComponentInChildren<UGUIPort>();
        //var currentNode = SessionManager.Instance.sceneGraph.nodes.Where(n => n.Ports.Any(p => p.fieldName == port.fieldName)).FirstOrDefault();


        var currentNode = SessionManager.Instance.sceneGraph.nodes.Where(n => n.ports.Any(p => p.Key.StartsWith("#List#reglas#less_than#"))).FirstOrDefault();
        string portName = currentNode.ports.Select(p => p.Key).Where(p => p.StartsWith("#List#reglas#less_than#")).FirstOrDefault();
        var baseNode = this.gameObject.GetComponentInParent<UGUIBaseNode>();

        if (currentNode is ToolPlacementComparer tpc)
        {
            //tpc.DeleteInstanceInput(port);
            tpc.DeleteInstanceInput(portName);
            // Visual Update Runtime Graph
            baseNode.GetRuntimeGraph().Refresh();
        }
        else
        {
            Debug.LogError("Node not found when trying to delete port");
        }
    }
}
