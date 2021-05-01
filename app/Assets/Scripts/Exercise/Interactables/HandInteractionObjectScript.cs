using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using NT;

public class HandInteractionObjectScript : MonoBehaviour
{
    private VRTK_InteractableObject linkedObject;
    private GameObject otherHand = null;
    private GameObject grabbingHand = null;
    private VRTK_SDKSetup setup;
    public string messageAction = "";

    protected virtual void OnEnable()
    {
        linkedObject = (linkedObject == null ? GetComponent<VRTK_InteractableObject>() : linkedObject);

        if (linkedObject != null)
        {
            linkedObject.InteractableObjectUsed += InteractableObjectUsed;
            linkedObject.InteractableObjectUngrabbed += (object sender, InteractableObjectEventArgs e) => { otherHand = null; grabbingHand = null; };
            linkedObject.InteractableObjectGrabbed += (object sender, InteractableObjectEventArgs e) => { grabbingHand = linkedObject.GetGrabbingObject(); };
        }
    }

    private void Start()
    {
        setup = VRTK_SDKManager.GetLoadedSDKSetup();
    }

    private GameObject getController(GameObject o)
    {
        if (o.transform.IsChildOf(setup.actualLeftController.transform))
        {
            return setup.actualLeftController;
        }
        else if (o.transform.IsChildOf(setup.actualRightController.transform))
        {
            return setup.actualRightController;
        }
        else
        {
            return null;
        }

    }



    protected virtual void InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
    {

        var grabbingController = linkedObject.GetGrabbingObject();
        Debug.Log("<color=red>Other Hand = " + otherHand.name + "</color>");

        // MessageSystem.SendMessage(otherHand.name + " soap");
        MessageSystem.SendMessage(otherHand.name + " " + messageAction);
        // if (other.gameObject == setup.actualRightController)
        // {

        // }
        // MessageSystem.SendMessage("Soap Used");
        // gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (!linkedObject.IsGrabbed()) return;
        if (grabbingHand == null) return;
        setup = VRTK_SDKManager.GetLoadedSDKSetup();
        if (setup == null) return;
        var controller = getController(other.gameObject);
        Debug.Log(controller);

        // if (controller != null && controller != linkedObject.GetGrabbingObject())
        if (controller != null && controller != grabbingHand)
        {
            otherHand = controller;
            Debug.Log(otherHand);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        // if (!linkedObject.IsGrabbed()) return;
        if (grabbingHand == null) return;
        setup = VRTK_SDKManager.GetLoadedSDKSetup();
        if (setup == null) return;
        var controller = getController(other.gameObject);

        // Debug.Log(setup.actualLeftController);
        if (controller != null && controller != linkedObject.GetGrabbingObject())
        {
            otherHand = null;
        }
    }
}
