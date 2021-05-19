using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HMS;
using TMPro;
public class HintLogUIScript : MonoBehaviour
{
    private Object HintMessageEntryObject;
    private Transform MessageLog;
    // Start is called before the first frame update
    private void Awake()
    {
        MessageLog = GameObject.Find("MessageLog").transform;
        HintMessageEntryObject = Resources.Load("UI/HintMessageEntry");
        HintMessageSystem.onHintSent += ReceiveHint;
    }

    private void OnDisable()
    {
        HintMessageSystem.onHintSent -= ReceiveHint;
    }
    private void OnDestroy()
    {
        HintMessageSystem.onHintSent -= ReceiveHint;
    }


    void ReceiveHint(string hint)
    {
        var scroll = this.gameObject.transform.GetComponentInChildren<ScrollRect>();
        float backup = scroll.verticalNormalizedPosition;

        var newHint = Instantiate(HintMessageEntryObject) as GameObject;
        newHint.GetComponentInChildren<TextMeshProUGUI>().text = hint;
        newHint.transform.SetParent(MessageLog, false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(MessageLog.GetComponent<VerticalLayoutGroup>().GetComponent<RectTransform>());

        StartCoroutine(RebuildAfterOneFrame(scroll, backup));


        // newHint.transform.parent = MessageLog;
    }
    private IEnumerator RebuildAfterOneFrame(ScrollRect scrollRect, float verticalPos)
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0;
        // LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)scrollRect.transform);

    }

    void Start()
    {

    }
    int c = 0;
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {

            HintMessageSystem.SendHint($"Test Hint {c}");
            c++;
        }
    }
}
