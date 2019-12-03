using NT;
using NT.SceneObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SteelTableASceneGameObject : SceneGameObject
{
    public GameObject coverVisual;
    public GameObject surface;
    public bool coverVisible;


    private void Start()
    {
        coverVisual.SetActive(coverVisible);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cover")
        {
            other.gameObject.SetActive(false);
            SetCover();
        }
        // Logica de superficie con interacción VR
        if (other.tag == "Tool")
        {
            VRTK_InteractableObject interactable = other.gameObject.GetComponentInParent<VRTK_InteractableObject>();
            if (!interactable.IsGrabbed())
            {
                TranslateToSurface(other.gameObject);
                other.gameObject.transform.rotation = Quaternion.identity;

                other.gameObject.transform.SetParent(this.gameObject.transform);
            }
        }
    }


    /*public void OnObjectSnapped(object sender, SnapDropZoneEventArgs e)
    {
        SteelTableAData d = (SteelTableAData)data.data.GetValue();
        VRTK_SnapDropZone dropZone = ((VRTK_SnapDropZone)sender);
        SceneGameObjectReference sor = new SceneGameObjectReference(e.snappedObject.gameObject.GetComponentInChildren<SceneGameObject>());

        // Lógica de asignar el objeto en la posición
        // recibimos slotN, nos quedamos con N
        string s = dropZone.gameObject.name.Split(new string[] { "slot" }, StringSplitOptions.None)[1];
        int posicion = Int32.Parse(s);
    }*/

    private void OnTriggerStay(Collider other)
    {
        // Logica de superficie con interacción VR
        if (other.tag == "Tool")
        {
            VRTK_InteractableObject interactable = other.gameObject.GetComponentInParent<VRTK_InteractableObject>();
            if (!interactable.IsGrabbed())
            {
                other.gameObject.transform.SetParent(this.gameObject.transform);
            }
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
    }*/

    private void TranslateToSurface(GameObject obj)
    {
        BoxCollider collider = surface.GetComponent<BoxCollider>();
        float posY = this.transform.InverseTransformPoint(collider.transform.TransformPoint(collider.center + collider.size / 2)).y;
        obj.transform.position = new Vector3(obj.transform.position.x, posY, obj.transform.position.z);
    }


    public void SetCover()
    {
        coverVisual.gameObject.SetActive(true);
        MessageSystem.SendMessage(data.id + "Cover On");
    }

}