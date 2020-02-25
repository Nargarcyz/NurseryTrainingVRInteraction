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
        GameObject buttonPressed = EventSystem.current.currentSelectedGameObject;

        var port = dynamicPort.GetComponentInChildren<UGUIPort>();
        var currentNode = SessionManager.Instance.sceneGraph.nodes.Where(n => n.Ports.Any(p => p.fieldName == port.fieldName)).FirstOrDefault();
        // Delete Port in Node
        if (currentNode is ToolPlacementComparer tpc)
        {
            tpc.DeleteInstanceInput(port);
        }
        else
        {
            Debug.LogError("Node not found when trying to delete port");
        }

        // Update Graph (Runtime Graph -> Refresh)


        // Delete GameObject

        // Clean reference in ToolComparer

    }
}
