﻿using NT;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class SteelTableASceneGameObject : SceneGameObject
{
    [Header("Steel Table Interaction")]
    public GameObject coverVisual;
    public bool coverVisible;
    public GameObject surface;

    #region Event Logging
    public UnityEvent ToolChange;
    #endregion

    private void Start()
    {
        coverVisual.SetActive(coverVisible);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collisionObject = other.gameObject;
        Debug.Log("OBJECT ENTERED");
        if (other.tag == "Cover")
        {
            collisionObject.SetActive(false);
            SetCover();
        }
        // Logica de superficie con interacción VR
        if (other.tag == "Tool")
        {
            VRTK_InteractableObject interactable = collisionObject.GetComponentInParent<VRTK_InteractableObject>();
            //if (interactable.IsGrabbed())
            interactable.gameObject.transform.SetParent(surface.transform);

            ThrowToolChangeEvent();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject collisionObject = other.gameObject;
        // Logica de superficie con interacción VR
        if (other.tag == "Tool")
        {
            VRTK_InteractableObject interactable = collisionObject.GetComponentInParent<VRTK_InteractableObject>();
            interactable.gameObject.transform.SetParent(null);
            // VRTK guarda el parent de antes de agarrar, y lo aplica al soltar. Debemos cambiarlo ahí tambien.
            interactable.GetPreviousState(out Transform preParent, out bool preKinem, out bool preGrab);
            interactable.OverridePreviousState(interactable.gameObject.transform.parent, preKinem, preGrab);

            ThrowToolChangeEvent();
        }
    }

    //private void TranslateToSurface(GameObject obj)
    //{
    //    BoxCollider collider = surface.GetComponent<BoxCollider>();
    //    float posY = this.transform.InverseTransformPoint(collider.transform.TransformPoint(collider.center + collider.size / 2)).y;
    //    obj.transform.position = new Vector3(obj.transform.position.x, posY, obj.transform.position.z);
    //}


    public void SetCover()
    {
        coverVisual.SetActive(true);
        MessageSystem.SendMessage(data.id + "Cover On");
    }

    private void ThrowToolChangeEvent()
    {
        ToolChange?.Invoke();
    }

}