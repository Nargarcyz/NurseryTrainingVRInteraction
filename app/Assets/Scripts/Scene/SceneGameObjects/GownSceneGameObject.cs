using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.Controllables.ArtificialBased;
using NT;
public class GownSceneGameObject : SceneGameObject
{


    private VRTK_InteractableObject linkedObject;
    // Start is called before the first frame update
    void Start()
    {


    }


    private GameObject OtherController(GameObject controller)
    {
        VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
        if (controller == setup.actualLeftController)
        {
            return setup.actualRightController;
        }
        else if (controller == setup.actualRightController)
        {
            return setup.actualLeftController;
        }
        else return null;
    }


    protected virtual void OnEnable()
    {
        linkedObject = (linkedObject == null ? GetComponent<VRTK_InteractableObject>() : linkedObject);

        if (linkedObject != null)
        {
            linkedObject.InteractableObjectUsed += InteractableObjectUsed;
            // linkedObject.InteractableObjectUngrabbed += (object sender, InteractableObjectEventArgs e) => { SetHandlesActive(false); };
            // linkedObject.InteractableObjectGrabbed += (object sender, InteractableObjectEventArgs e) => { SetHandlesActive(true); };
            // }
        }
        // Update is called once per frame

    }
    private bool open = false;
    private void InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
    {
        open = !open;
        Debug.Log(open);
        if (open)
        {
            GetComponent<Animator>().Play("Open");

        }
        else
        {
            GetComponent<Animator>().Play("Close");
        }
    }
    void Update()
    {


    }

    public void GownStateChange(int state)
    {
        switch (state)
        {
            case 1:
                MessageSystem.SendMessage("Gown Open");
                break;
            case 0:
                MessageSystem.SendMessage("Gown Closed");
                break;
            default:
                break;


        }
    }
}

