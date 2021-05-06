using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using NT;
using NT.SceneObjects;
public class EquipmentAssistantSceneGameObject : SceneGameObject
{

    private bool userInRange = false;
    private EquipmentAssistantData variables;
    void Start()
    {
        variables = (EquipmentAssistantData)data.data.GetValue();
    }
    // private void OnDestroy()
    // {
    //     MessageSystem.onMessageSent -= RecieveMessage;
    // }

    private void OnTriggerEnter(Collider other)
    {
        var setup = VRTK_SDKManager.GetLoadedSDKSetup();
        if (!setup) return;
        var obj = other.gameObject;
        Debug.Log($"obj = {obj.name} setup = {setup.actualHeadset.name}");
        // if (obj.transform.IsChildOf(setup.transform))
        // {
        //     Debug.Log("<color=yellow>YES</color>");
        // }
        if (obj.transform.IsChildOf(setup.actualHeadset.transform))
        {

            variables.userInRange = true;
            data.data.SetValue(variables);
            // Debug.Log("<color=yellow>YES</color>");

            // MessageSystem.SendMessage("");
            // var e = new EquipmentAssistantObject();
            // var l = e.GetCallbacks();
            // MessageSystem.SendMessage(l[0]);
        }
    }

    private void Update()
    {
        if (variables.userInRange)
        {
            var setup = VRTK_SDKManager.GetLoadedSDKSetup();
            // this.transform.forward;
            var headsetForward = Vector3.forward;
            // var rotation = setup.actualHeadset.transform.rotation.y;
            var rotation = setup.actualHeadset.transform.eulerAngles.y;

            headsetForward = Quaternion.Euler(0, rotation, 0) * headsetForward;

            var angle = Vector3.Angle(this.transform.forward, headsetForward);

            if (angle < 90 && !variables.userInPosition)
            {
                variables.userInPosition = true;
                data.data.SetValue(variables);
            }
            else if (angle >= 90 && variables.userInPosition)
            {
                variables.userInPosition = false;
                data.data.SetValue(variables);
            }



        }
    }
}
