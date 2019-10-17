using NT;
using NT.SceneObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SteelTableAGameObject : SceneGameObject
{
    [HideInInspector]
    public List<VRTK_SnapDropZone> objectPositions;
    [Range(2,10)]
    public int gridSize;
    public GameObject coverVisual;
    public bool coverVisible;


    private void Start()
    {
        coverVisual.SetActive(coverVisible);
        objectPositions = new List<VRTK_SnapDropZone>(gridSize);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cover")
        {
            other.gameObject.SetActive(false);
            SetCover();
        }
    }


    public void OnObjectSnapped(object sender, SnapDropZoneEventArgs e)
    {
        SteelTableAData d = (SteelTableAData)data.data.GetValue();
        VRTK_SnapDropZone dropZone = ((VRTK_SnapDropZone)sender);
        SceneGameObjectReference sor = new SceneGameObjectReference(e.snappedObject.gameObject.GetComponentInChildren<SceneGameObject>());

        // Lógica de asignar el objeto en la posición
        // recibimos slotN, nos quedamos con N
        string s = dropZone.gameObject.name.Split(new string[] { "slot" }, StringSplitOptions.None)[1];
        int posicion = Int32.Parse(s);
    }


    public void SetCover()
    {
        coverVisual.gameObject.SetActive(true);
        MessageSystem.SendMessage(data.id + "Cover On");
    }

}