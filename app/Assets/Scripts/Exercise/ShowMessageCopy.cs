using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowMessageCopy : MonoBehaviour
{
    public TextMeshPro thisText;
    public TextMeshPro copyText;

    void Update()
    {
        thisText.text = copyText.text;
    }
}
