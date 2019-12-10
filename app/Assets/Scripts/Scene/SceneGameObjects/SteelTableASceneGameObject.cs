using NT;
using NT.SceneObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SteelTableASceneGameObject : SceneGameObject
{
    [Header("Steel Table Interaction")]
    public GameObject coverVisual;
    public bool coverVisible;
    public GameObject surface;


    private void Start()
    {
        coverVisual.SetActive(coverVisible);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collisionObject = other.gameObject;
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

}