using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using NT;
using NT.SceneObjects;
public class EquipmentAssistantSceneGameObject : SceneGameObject
{
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
            Debug.Log("<color=yellow>YES</color>");

            MessageSystem.SendMessage("");
            var e = new EquipmentAssistantObject();
            var l = e.GetCallbacks();
            MessageSystem.SendMessage(l[0]);
        }
    }

}
