using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MainSurgeonSceneGameObject : SceneGameObject
{
    public Animator surgeonAnimator;

    // Start is called before the first frame update
    void Start()
    {
        surgeonAnimator.SetBool("RightHandHasTool", false);
        surgeonAnimator.SetBool("CorrectTool", false);

        //rightHandSnap.ObjectSnappedToDropZone
        //rightHandSnap.ObjectUnsnappedFromDropZone

    }

    // Update is called once per frame
    void Update()
    {
        //surgeonAnimator.SetTrigger("");
        //surgeonAnimator.SetBool("", false);

        //if (rightHandSnap.enabled && rightHandSnap.)
    }

    public void HandleObjectSnapped(SnapDropZoneEventArgs e)
    {
        surgeonAnimator.SetBool("RightHandHasTool", true);

        // Check if tool is correct

    }

    public void HandleObjectUnsnapped(SnapDropZoneEventArgs e)
    {
        surgeonAnimator.SetBool("RightHandHasTool", false);
    }
}
