using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using NT;
using System.Linq;
using System;
public class SinkSceneGameObject : SceneGameObject
{
    private GameObject boundaries;
    private GameObject faucets;
    private bool leftHandSoapy = false;
    private bool rightHandSoapy = false;

    private bool leftHandWashed = false;
    private bool rightHandWashed = false;

    private bool leftHandDried = false;
    private bool rightHandDried = false;


    protected virtual void OnEnable()
    {
        Debug.Log("Boundaries: ");
        // VRTK_SDKManager manager = GameObject.Find("[VRTK_SDKManager]").GetComponent<VRTK_SDKManager>();

        VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
        // boundaries = setup.actualBoundaries;
        Debug.Log(setup);
    }

    void Start()
    {
        MessageSystem.onMessageSent += RecieveMessage;
    }

    public ParticleSystem soapEffect;

    private void RecieveMessage(string msg)
    {
        if (msg.Contains("Exercise Started"))
        {
            VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
            if (setup != null)
            {
                setup.actualBoundaries.transform.position = transform.position;
            }
        }
        else if (msg.Contains("LeftHandAnchor soap"))
        {
            VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
            var soapParticles = Instantiate(soapEffect);
            soapParticles.transform.position = setup.actualLeftController.transform.position;
        }
        else if (msg.Contains("RightHandAnchor soap"))
        {
            VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
            var soapParticles = Instantiate(soapEffect);
            soapParticles.transform.position = setup.actualRightController.transform.position;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
        // Debug.Log(setup.actualRightController.transform.rotation);

        // if (setup != null)
        // {
        //     setup.actualBoundaries.transform.position = Vector3.zero;
        //     Debug.Log(setup);
        // }
    }
}