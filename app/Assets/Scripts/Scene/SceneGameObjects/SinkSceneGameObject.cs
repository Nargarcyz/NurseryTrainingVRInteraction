using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using NT;

public class SinkSceneGameObject : SceneGameObject
{
    private GameObject boundaries;

    protected virtual void OnEnable()
    {
        Debug.Log("Boundaries: ");
        // VRTK_SDKManager manager = GameObject.Find("[VRTK_SDKManager]").GetComponent<VRTK_SDKManager>();

        VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
        // boundaries = setup.actualBoundaries;
        Debug.Log(setup);
    }


    private void OnTriggerEnter(Collider other)
    {
        VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
        if (setup != null)
        {
            setup.actualBoundaries.transform.position = Vector3.zero;
            Debug.Log(setup);
        }
    }
}