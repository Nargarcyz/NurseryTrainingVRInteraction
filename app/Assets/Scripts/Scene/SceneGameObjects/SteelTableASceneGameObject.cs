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
    public GameObject triggerCollider;


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
            interactable.gameObject.transform.SetParent(surface.transform);
            Debug.Log("ENTER TRIGGER");
        }
    }

    /*private void OnTriggerStay(Collider other)
    {
        // No realizar las comprobaciones en cada frame
        if (timer <= 0)
        {
           timer = 5f;
            // Logica de superficie con interacción VR
            GameObject collisionObject = other.gameObject;
            GameObject parentObject = other.gameObject.transform.parent.gameObject;
            if (other.tag == "Tool")
            {
                VRTK_InteractableObject interactable = collisionObject.GetComponentInParent<VRTK_InteractableObject>();
                if (!interactable.IsGrabbed())
                {
                    Rigidbody rb = collisionObject.GetComponentInParent<Rigidbody>();
                    if (rb.velocity == Vector3.zero)
                    {
                        parentObject.transform.SetParent(surface.transform);
                    }
                }
            }
        } else
        {
            timer -= Time.deltaTime;
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        GameObject collisionObject = other.gameObject;
        // Logica de superficie con interacción VR
        if (other.tag == "Tool")
        {
            VRTK_InteractableObject interactable = collisionObject.GetComponentInParent<VRTK_InteractableObject>();
            //if (interactable.IsGrabbed())
                interactable.gameObject.transform.SetParent(null);
                Debug.Log("EXIT TRIGGER");
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