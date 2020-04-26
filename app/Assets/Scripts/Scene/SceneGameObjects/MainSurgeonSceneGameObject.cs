using NT;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using VRTK;

public class MainSurgeonSceneGameObject : SceneGameObject
{
    public Animator surgeonAnimator;
    public VRTK_SnapDropZone rightHandSnap;
    public GameObject showMessage;

    private List<Tools> sceneTools;
    private List<Tools>.Enumerator toolsEnumerator;
    private TextMeshPro messageText;

    void Start()
    {
        surgeonAnimator.SetBool("RightHandHasTool", false);
        surgeonAnimator.SetBool("CorrectTool", false);

        rightHandSnap.ObjectSnappedToDropZone += HandleObjectSnapped;
        rightHandSnap.ObjectUnsnappedFromDropZone += HandleObjectUnsnapped;

        // Change for node behavior
        UpdateOptionsList();
        sceneTools.Shuffle();
        toolsEnumerator = sceneTools.GetEnumerator();
        MessageSystem.onMessageSent += RecieveMessage;
        // Hide message box
        showMessage.transform.localScale = Vector3.zero;
        messageText = showMessage.GetComponentInChildren<TextMeshPro>();
    }

    private void UpdateOptionsList()
    {
        var tools = SessionManager.Instance.GetSceneGameObjectsWithTag("Tool");
        sceneTools = tools.Cast<ITool>().Select(t => t.GetToolType()).Distinct().ToList();
    }
    private void RecieveMessage(string msg)
    {
        if (msg.Contains("Exercise Started"))
        {
            ExerciseStart();
        }
    }

    //void Update()
    //{
    //    if (rightHandSnap.enabled && rightHandSnap.)
    //}

    public void ExerciseStart()
    {
        surgeonAnimator.SetTrigger("SessionStart");

        float seconds = Random.Range(5f, 10f);
        StartCoroutine(WaitSecondsAndAskTool(seconds));
    }

    private void HandleObjectSnapped(object sender, SnapDropZoneEventArgs e)
    {
        surgeonAnimator.SetBool("RightHandHasTool", true);

        var snappedTool = e.snappedObject
            .GetComponentInChildren<ToolSceneGameObject>()
            ?.GetToolType();
        bool toolCorrect = toolsEnumerator.Current == snappedTool;

        if (toolCorrect)
        {
            showMessage.transform.localScale = Vector3.zero;

            surgeonAnimator.SetTrigger("CorrectTool");
            float seconds = Random.Range(5f, 15f);
            StartCoroutine(WaitSecondsAndReturnTool(seconds));
        }
        else
        {
            surgeonAnimator.SetTrigger("IncorrectTool");
        }
    }
    private void HandleObjectUnsnapped(object sender, SnapDropZoneEventArgs e)
    {
        surgeonAnimator.SetBool("RightHandHasTool", false);
    }

    #region AskNewTool
    private IEnumerator WaitSecondsAndAskTool(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        AskTool();
    }
    private void AskTool()
    {
        if (toolsEnumerator.MoveNext()) {
            showMessage.transform.localScale = Vector3.one;
            messageText.text = string.Format("Need tool \"{0}\"", toolsEnumerator.Current.ToString());

            surgeonAnimator.SetTrigger("AskNewTool");
        }
        else
        {
            surgeonAnimator.SetTrigger("SessionEnd");
        }
    }
    #endregion

    #region ReturnTool
    private IEnumerator WaitSecondsAndReturnTool(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        ReturnTool();
    }

    private void ReturnTool()
    {
        surgeonAnimator.SetTrigger("ReturnTool");

        float seconds = Random.Range(5f, 10f);
        StartCoroutine(WaitSecondsAndAskTool(seconds));
    }
    #endregion

    private void LogResultToolGiven()
    {

    }
}

#region Shuffle List extension
public static class ThreadSafeRandom
{
    [System.ThreadStatic] private static System.Random Local;
    public static System.Random ThisThreadsRandom
    {
        get { return Local ?? (Local = new System.Random(unchecked(System.Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
    }
}

static class MyExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
#endregion