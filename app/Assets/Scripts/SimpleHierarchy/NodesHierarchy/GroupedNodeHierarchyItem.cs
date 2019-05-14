﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GroupedNodeHierarchyItem : GUIHierarchyItem, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public bool dragOnSurfaces = true;

    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;

    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;

        GroupedNodeHierarchyData nhd = (GroupedNodeHierarchyData) data;

        if(nhd.nodeGroup == null) return;

        m_DraggingIcon = new GameObject("icon");

        m_DraggingIcon.transform.SetParent(canvas.transform, false);
        m_DraggingIcon.transform.SetAsLastSibling();

        var image = m_DraggingIcon.AddComponent<Image>();

        image.sprite = GetComponentInChildren<Image>().sprite;
        image.SetNativeSize();
        image.raycastTarget = false;

        if (dragOnSurfaces)
            m_DraggingPlane = transform as RectTransform;
        else
            m_DraggingPlane = canvas.transform as RectTransform;

        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData data)
    {        
        if (m_DraggingIcon != null)
            SetDraggedPosition(data);
    }

    private void SetDraggedPosition(PointerEventData data)
    {
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = m_DraggingPlane.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_DraggingIcon != null)
            Destroy(m_DraggingIcon);

        GroupedNodeHierarchyData nhd = (GroupedNodeHierarchyData) data;

        if(nhd.nodeGroup == null) return;
        if(!eventData.pointerCurrentRaycast.gameObject) return;

        RuntimeGraph rg = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<RuntimeGraph>();

        if(!rg) return;

        NTScrollRect ntScrollRect = rg.GetComponentInChildren<NTScrollRect>();

        if(ntScrollRect == null) return;

        Vector2 nodePosition = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform) ntScrollRect.content.transform , 
            Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out nodePosition
        ); 
        
        nodePosition = new Vector2( nodePosition.x - 80, -nodePosition.y);

        rg.AddNodeGroup(nhd.nodeGroup , nodePosition);
    }
    
}