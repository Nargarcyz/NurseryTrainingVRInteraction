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
    private bool leftHandWet = false;
    private bool rightHandWet = false;

    private bool leftHandSoapy = false;
    private bool rightHandSoapy = false;

    private bool leftHandWashed = false;
    private bool rightHandWashed = false;

    private bool leftHandClean = false;
    private bool rightHandClean = false;


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
        // else if (msg.Contains("LeftHandAnchor wet"))
        // {
        //     leftHandWet = true;
        //     if (leftHandSoapy)
        //     {
        //         leftHandSoapy = false;
        //         leftHandWashed = true;
        //     }
        // }
        // else if (msg.Contains("LeftHandAnchor soap"))
        // {
        //     if (leftHandWet)
        //     {
        //         VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
        //         var soapParticles = Instantiate(soapEffect);
        //         soapParticles.transform.position = setup.actualLeftController.transform.position;
        //         leftHandSoapy = true;
        //     }
        // }
        // else if (msg.Contains("LeftHandAnchor dry"))
        // {
        //     VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
        //     var mat = setup.actualLeftController.GetComponentInChildren<Renderer>().materials[0];
        //     mat.SetFloat("_Metallic", 0);
        //     mat.SetFloat("_Glossiness", 0.5f);
        //     leftHandWet = false;
        //     leftHandSoapy = false;
        //     if (leftHandWashed)
        //     {
        //         leftHandClean = true;
        //         if (rightHandClean)
        //         {
        //             MessageSystem.SendMessage("Hands Cleaned");

        //         }
        //     }

        // }
        // else if (msg.Contains("RightHandAnchor wet"))
        // {
        //     rightHandWet = true;
        //     if (rightHandSoapy)
        //     {
        //         rightHandSoapy = false;
        //         rightHandWashed = true;
        //         Debug.Log("<color=blue>Right Hand Washed");
        //     }
        // }
        // else if (msg.Contains("RightHandAnchor soap"))
        // {
        //     if (rightHandWet)
        //     {
        //         VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
        //         var soapParticles = Instantiate(soapEffect);
        //         soapParticles.transform.position = setup.actualRightController.transform.position;
        //         rightHandSoapy = true;
        //     }
        // }
        // else if (msg.Contains("RightHandAnchor dry"))
        // {
        //     VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
        //     var mat = setup.actualRightController.GetComponentInChildren<Renderer>().materials[0];
        //     mat.SetFloat("_Metallic", 0);
        //     mat.SetFloat("_Glossiness", 0.5f);
        //     rightHandWet = false;
        //     rightHandSoapy = false;
        //     if (rightHandWashed)
        //     {
        //         rightHandClean = true;
        //         if (leftHandClean)
        //         {
        //             MessageSystem.SendMessage("Hands Cleaned");

        //         }
        //     }
        // }


        else if (msg.Contains("Go to sink"))
        {
            VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
            if (setup != null && this.transform != null)
            {
                setup.actualBoundaries.transform.position = transform.position;
            }
        }
        else if (msg.Contains("Go to room"))
        {
            VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();
            if (setup != null)
            {
                setup.actualBoundaries.transform.position = Vector3.zero;
            }
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