using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.Controllables.PhysicsBased;
using VRTK.Controllables;
using NT;
public class FaucetScript : MonoBehaviour
{
    public ParticleSystem waterStream;
    private VRTK_PhysicsRotator handle;
    private VRTK_SDKSetup setup;
    // Start is called before the first frame update
    void Start()
    {
        setup = VRTK_SDKManager.GetLoadedSDKSetup();
        waterStream.Stop();
        handle = GetComponentInChildren<VRTK_PhysicsRotator>();
        handle.ValueChanged += UpdateFlow;
        controllers = new List<GameObject>();

    }




    private void UpdateFlow(object sender, ControllableEventArgs e)
    {
        float angle = handle.GetValue();
        if (angle != handle.angleLimits.maximum)
        {
            waterStream.Play();
            var emission = waterStream.emission;
            var t = Mathf.Lerp(0, 100, 1 - (angle / handle.angleLimits.maximum));
            // Debug.Log(t);
            emission.rateOverTime = t;
        }
        else
        {
            waterStream.Stop();
        }

    }

    private GameObject getController(GameObject o)
    {
        if (o.transform.IsChildOf(setup.actualLeftController.transform))
        {
            return setup.actualLeftController;
        }
        else if (o.transform.IsChildOf(setup.actualRightController.transform))
        {
            return setup.actualRightController;
        }
        else
        {
            return null;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        setup = VRTK_SDKManager.GetLoadedSDKSetup();
        if (setup == null) return;
        var controller = getController(other.gameObject);
        // if (controller != null && !controllers.Contains(controller)) controllers.Add(controller);
        ProcessController(controller);



    }

    private void ProcessController(GameObject controller)
    {
        if (controller != null)
        {
            var mat = controller.GetComponentInChildren<Renderer>().materials[0];
            if (mat.GetFloat("_Metallic") >= 0.5f && mat.GetFloat("_Glossiness") >= 0.7f)
                MessageSystem.SendMessage(controller.name + " wet");
            else if (!controllers.Contains(controller))
            {
                controllers.Add(controller);
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        setup = VRTK_SDKManager.GetLoadedSDKSetup();
        if (setup == null) return;
        var controller = getController(other.gameObject);
        if (controller != null && controllers.Contains(controller)) controllers.Remove(controller);
    }

    private List<GameObject> controllers;

    void Update()
    {
        if (handle.GetValue() <= handle.angleLimits.maximum / 2)
        {
            foreach (var c in controllers)
            {
                var mat = c.GetComponentInChildren<Renderer>().materials[0];
                Debug.Log(c.name + $" Metallic={mat.GetFloat("_Metallic")} Glossiness={mat.GetFloat("_Glossiness")}");

                if (mat.GetFloat("_Metallic") <= 0.5f)
                    mat.SetFloat("_Metallic", mat.GetFloat("_Metallic") + 0.1f * Time.deltaTime);
                if (mat.GetFloat("_Glossiness") <= 0.7f)
                    mat.SetFloat("_Glossiness", mat.GetFloat("_Glossiness") + 0.1f * Time.deltaTime);
                if (mat.GetFloat("_Metallic") >= 0.5f && mat.GetFloat("_Glossiness") >= 0.7f)
                    MessageSystem.SendMessage(c.name + " wet");
            }
        }

    }
}
