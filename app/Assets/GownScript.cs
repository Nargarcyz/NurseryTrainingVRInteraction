using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.Controllables.ArtificialBased;
public class GownScript : MonoBehaviour
{

    private VRTK_InteractableObject handle1;
    private VRTK_InteractableObject handle2;
    private VRTK_InteractableObject linkedObject;
    // Start is called before the first frame update
    void Start()
    {
        handle1 = transform.Find("Handle1").GetComponent<VRTK_InteractableObject>();
        handle2 = transform.Find("Handle2").GetComponent<VRTK_InteractableObject>();
        // handle1.isGrabbable = false;
        // handle2.isGrabbable = false;
        // handle1.enabled = false;
        // handle2.enabled = false;

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
    private void SetHandlesActive(bool value)
    {
        // handle1.isGrabbable = value;
        // handle2.isGrabbable = value;
        // handle1.enabled = value;
        // handle2.enabled = value;
        if (value)
        {
            // handle1.ignoreCollisionsWith = new GameObject[] { linkedObject.GetGrabbingObject() };
            // handle1.onlyInteractWith = new GameObject[] { OtherController(linkedObject.GetGrabbingObject()) };
        }
        else
        {
            // handle1.ignoreCollisionsWith = new GameObject[] { };
            // handle1.onlyInteractWith = new GameObject[] { };
        }


    }

    protected virtual void OnEnable()
    {
        linkedObject = (linkedObject == null ? GetComponent<VRTK_InteractableObject>() : linkedObject);

        if (linkedObject != null)
        {
            linkedObject.InteractableObjectUsed += InteractableObjectUsed;
            linkedObject.InteractableObjectUngrabbed += (object sender, InteractableObjectEventArgs e) => { SetHandlesActive(false); };
            linkedObject.InteractableObjectGrabbed += (object sender, InteractableObjectEventArgs e) => { SetHandlesActive(true); };
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
}

