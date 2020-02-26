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
        var port = dynamicPort.GetComponentInChildren<UGUIPort>();
        var guiNode = this.gameObject.GetComponentInParent<IUGUINode>();
        var currentNode = SessionManager.Instance.sceneGraph.nodes.Where(n => n.Ports.Any(p => p.fieldName == port.fieldName)).FirstOrDefault();

        // Delete Port reference in Node
        if (currentNode is ToolPlacementComparer tpc)
        {
            // TODO: DEFINE FUNCTION IN NODE INTERFACE
            tpc.DeleteInstanceInput(port);
            // Visual Update Runtime Graph
            guiNode.GetRuntimeGraph().Refresh();
        }
        else
        {
            Debug.LogError("Node not found when trying to delete port");
        }
    }
}
