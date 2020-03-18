using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSceneObject : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Tool"))
        {
            ExerciseFileLogger.Instance.LogMessage($"La herramienta {other.gameObject.name} ha entrado en contacto con el suelo.", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Tool"))
        {
            ExerciseFileLogger.Instance.LogMessage($"La herramienta {other.gameObject.name} ha dejado de tocar el suelo.", true);
        }
    }

}
