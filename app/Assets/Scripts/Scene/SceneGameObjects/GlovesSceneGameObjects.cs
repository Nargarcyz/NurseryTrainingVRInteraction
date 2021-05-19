using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using NT;


public class GlovesSceneGameObjects : SceneGameObject
{
    public Color glovesColor;
    public VRTK_InteractableObject linkedObject;

    public GameObject rightGlove;
    public GameObject leftGlove;

    private void Start()
    {
        MessageSystem.onMessageSent += ReceiveMessage;
    }
    private void OnDestroy()
    {
        MessageSystem.onMessageSent -= ReceiveMessage;
    }

    private void ReceiveMessage(string msg)
    {
        if (msg.Contains("Exercise Started"))
        {
            var rigidbodies = GetComponentsInChildren<Rigidbody>();
            foreach (var r in rigidbodies)
            {
                r.useGravity = true;
                r.isKinematic = false;
            }
        }
        else if (msg.Contains("Left Hand glove on"))
        {
            DestroyImmediate(transform.Find("LeftGlove").gameObject);
        }
        else if (msg.Contains("Right Hand glove on"))
        {
            DestroyImmediate(transform.Find("RightGlove").gameObject);
        }
    }

    protected virtual void InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
    {
        // MessageSystem.SendMessage("Gloves On");
        // gameObject.SetActive(false);
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
