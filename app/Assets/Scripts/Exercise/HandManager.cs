using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using NT;

public class HandManager : MonoBehaviour
{

    private enum HandState
    {
        Dirty,
        Wetting,
        Wet,
        Soapy,
        CleanWet,
        Clean

    }

    private class HandInfo
    {
        public HandInfo(GameObject handReference)
        {
            this.handObject = handReference;
            this.state = HandState.Dirty;
            this.wetness = 0;
            if (handReference == VRTK_SDKManager.GetLoadedSDKSetup().actualLeftController)
            {
                this.name = "Left Hand";
            }
            else if (handReference == VRTK_SDKManager.GetLoadedSDKSetup().actualRightController)
            {
                this.name = "Right Hand";
            }

        }

        private string _name;
        public string name { get { return _name; } set { this._name = value; } }
        private float _wetness;
        public float wetness { get { return _wetness; } set { this._wetness = Mathf.Clamp(value, 0, 1); } }
        public GameObject handObject;
        public HandState state;

    }
    // private GameObject leftHand;
    // private GameObject rightHand;

    // private HandState leftHandState;
    // private HandState rightHandState;

    [Range(0, 1)]
    public float wetMetallic = 0.5f;
    [Range(0, 1)]
    public float wetSmoothness = 0.7f;

    private HandInfo leftHand;
    private HandInfo rightHand;

    private VRTK_SDKSetup setup;
    public ParticleSystem soapEffect;
    public ParticleSystem drippingEffect;

    void Awake()
    {
        MessageSystem.onMessageSent += RecieveMessage;
    }
    private void OnDisable()
    {
        MessageSystem.onMessageSent -= RecieveMessage;
    }
    private void OnDestroy()
    {
        MessageSystem.onMessageSent -= RecieveMessage;
    }
    private void RecieveMessage(string msg)
    {
        if (msg.Contains("Application Start")) return;
        else if (msg.Contains("Exercise Started"))
        {
            setup = VRTK_SDKManager.GetLoadedSDKSetup();
            leftHand = new HandInfo(setup.actualLeftController);
            rightHand = new HandInfo(setup.actualRightController);
        }

        else
        {
            if (msg.Contains(leftHand.name))
            {
                ProcessStatus(leftHand, msg);
            }
            else
            if (msg.Contains(rightHand.name))
            {
                ProcessStatus(rightHand, msg);
            }
        }
    }

    private void ProcessStatus(HandInfo hand, string msg)
    {
        switch (hand.state)
        {
            case HandState.Dirty:
                {
                    if (msg.Contains("in water"))
                        hand.state = HandState.Wetting;
                    else if (msg.Contains("out of water"))
                        hand.state = HandState.Dirty;
                    break;
                }
            case HandState.Wetting:
                {
                    if (msg.Contains("in water"))
                        hand.state = HandState.Wetting;
                    else if (msg.Contains("out of water"))
                        hand.state = HandState.Dirty;
                    else if (msg.Contains("wet"))
                        hand.state = HandState.Wet;
                    break;
                }
            case HandState.Wet:
                {
                    if (msg.Contains("soap"))
                    {
                        var soapParticles = Instantiate(soapEffect);
                        soapParticles.transform.position = hand.handObject.transform.position;
                        hand.state = HandState.Soapy;
                    }
                    break;
                }
            case HandState.Soapy:
                {
                    if (msg.Contains("in water"))
                        hand.state = HandState.CleanWet;
                    break;
                }
            case HandState.CleanWet:
                {
                    if (msg.Contains("dry"))
                    {
                        hand.state = HandState.Clean;
                    }
                    break;
                }
            case HandState.Clean:
                {
                    hand.wetness = 0;
                    // ExecuteState(hand);
                    CheckHands();
                    break;
                }
            default:
                break;
        }

    }

    private void CheckHands()
    {
        if (leftHand.state == HandState.Clean && rightHand.state == HandState.Clean)
            MessageSystem.SendMessage("Hands Cleaned");
    }

    private void ExecuteState(HandInfo hand)
    {
        var material = hand.handObject.GetComponentInChildren<Renderer>().materials[0];
        switch (hand.state)
        {
            case HandState.Wetting:
                {
                    Debug.Log(hand.name + " getting wet");
                    hand.wetness += 0.5f * Time.deltaTime;
                    if (material)
                    {
                        // Debug.Log($" Metallic={material.GetFloat("_Metallic")} Glossiness={material.GetFloat("_Glossiness")}");
                        if (material.GetFloat("_Metallic") <= wetMetallic)
                            material.SetFloat("_Metallic", hand.wetness * wetMetallic);
                        if (material.GetFloat("_Glossiness") <= wetSmoothness)
                            material.SetFloat("_Glossiness", hand.wetness * wetSmoothness);
                        if (material.GetFloat("_Metallic") >= wetMetallic && material.GetFloat("_Glossiness") >= wetSmoothness)
                            MessageSystem.SendMessage(hand.name + " wet");
                    }
                    break;
                }
            case HandState.Clean:
                {
                    if (hand.wetness == 0) return;
                    Debug.Log(hand.name + " drying");

                    material.SetFloat("_Metallic", 0);
                    material.SetFloat("_Glossiness", 0.5f);
                    hand.wetness = 0;
                    break;
                }
            default:
                return;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (leftHand != null && rightHand != null)
        {
            ExecuteState(leftHand);
            ExecuteState(rightHand);
        }
    }
}
