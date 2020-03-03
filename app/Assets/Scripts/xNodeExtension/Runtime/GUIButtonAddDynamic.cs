using NT;
using NT.Nodes.SessionCore;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIButtonAddDynamic : MonoBehaviour
{
    public Button addButton;
    private UGUIBaseNode baseNode;
    private IUGUIDynamicListNode dynamicNode;

    void Start()
    {
        GetCustomNode();
        addButton.onClick.AddListener(AddDynamicElement);
    }

    public void AddDynamicElement()
    {
        dynamicNode.AddRule();
        baseNode.GetRuntimeGraph().Refresh();
    }

    private void GetCustomNode()
    {
        baseNode = this.gameObject.GetComponentInParent<UGUIBaseNode>();
        if (baseNode.node is IUGUIDynamicListNode)
        {
            dynamicNode = (IUGUIDynamicListNode)baseNode.node;
        }
    }
}
