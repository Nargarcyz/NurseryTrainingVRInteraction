using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using NT;


public class GlovesSceneGameObjects : SceneGameObject
{
    public Color glovesColor;
    public VRTK_InteractableObject linkedObject;

    protected virtual void OnEnable()
    {
        linkedObject = (linkedObject == null ? GetComponent<VRTK_InteractableObject>() : linkedObject);

        if (linkedObject != null)
        {
            linkedObject.InteractableObjectUsed += InteractableObjectUsed;
        }
    }

    protected virtual void InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
    {
        MessageSystem.SendMessage("Gloves On");
        gameObject.SetActive(false);
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if(other.GetComponentInParent<VRTK.VRTK_AvatarHandController>() != null)
    //     {
    //         VRTK_AvatarHandController[] hands = FindObjectsOfType<VRTK_AvatarHandController>();

    //         foreach(var h in hands)
    //         {
    //             h.GetComponentInChildren<SkinnedMeshRenderer>().material.color = glovesColor;      
    //         }
    //         //MessageSystem.SendMessage(data.id + "Gloves On");
    //         MessageSystem.SendMessage("Gloves On");
    //         gameObject.SetActive(false);
    //     }
    // }
}
