using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.Controllables.PhysicsBased;
using VRTK.Controllables;
public class FaucetScript : MonoBehaviour
{
    public ParticleSystem waterStream;
    private VRTK_PhysicsRotator handle;
    // Start is called before the first frame update
    void Start()
    {
        waterStream.Stop();
        handle = GetComponentInChildren<VRTK_PhysicsRotator>();
        handle.ValueChanged += UpdateFlow;
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

    // Update is called once per frame
    // void Update()
    // {
    //     float angle = handle.GetValue();
    //     if (angle != handle.angleLimits.maximum)
    //     {
    //         waterStream.Play();
    //         var emission = waterStream.emission;
    //         var t = Mathf.Lerp(0, 100, 1 - (angle / handle.angleLimits.maximum));
    //         // Debug.Log(t);
    //         emission.rateOverTime = t;
    //     }
    //     else
    //     {
    //         waterStream.Stop();
    //     }


    // }
}
