using System;
using UnityEngine;


namespace HMS
{
    public class HintMessageSystem
    {
        public static Action<string> onHintSent;

        public static void SendHint(string message)
        {
            Debug.Log("Sending hint " + message);
            onHintSent?.Invoke(message);
        }
    }
}

