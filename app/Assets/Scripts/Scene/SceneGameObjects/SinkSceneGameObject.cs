using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using NT;
using System.Linq;
using System;
public class SinkSceneGameObject : SceneGameObject
{
    protected virtual void OnEnable()
    {
        Debug.Log("Boundaries: ");
        VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
        Debug.Log(setup);
    }

    void Start()
    {
        MessageSystem.onMessageSent += RecieveMessage;
    }
    private void OnDestroy()
    {
        MessageSystem.onMessageSent -= RecieveMessage;
    }

    public ParticleSystem soapEffect;

    private void RecieveMessage(string msg)
    {

        if (msg.Contains("Exercise Started"))
        {

        }
        else if (msg.Contains("Go to sink"))
        {
            VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
            if (setup != null && this.transform != null)
                setup.actualBoundaries.transform.position = transform.position;
        }
        else if (msg.Contains("Go to room"))
        {
            VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
            if (setup != null)
                setup.actualBoundaries.transform.position = Vector3.zero;
        }
    }
}