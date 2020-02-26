using NT.Nodes.SessionCore;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIButtonAddDynamic : MonoBehaviour
{
    public Button addButton;
    private IUGUIDynamicListNode node;

    protected void Init()
    {
        this.node = this.gameObject.GetComponentInParent<IUGUIDynamicListNode>();
        addButton.onClick.AddListener(AddDynamicElement);
    }

    public void AddDynamicElement()
    {
        node.AddRule();
    }
}
