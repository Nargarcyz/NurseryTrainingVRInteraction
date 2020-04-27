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

    private List<string> toolResults = new List<string>();
    private float timeToolAsked;

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
            // TODO: CHANGE TO DETECT SESSION BY NODE
            var currentSession = SessionManager.Instance.SessionData;
            string[] names = { "Delivery", "ToolDelivery", "Tool Delivery" };
            if (names.Any( s => currentSession.displayName.ToLower().Contains( s.ToLower() )))
            {
                ExerciseStart();
            }
        }
    }

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
            surgeonAnimator.SetTrigger("CorrectTool");
            showMessage.transform.localScale = Vector3.zero;
            AddCorrectResultRegister(toolsEnumerator.Current);
            
            float seconds = Random.Range(5f, 15f);
            StartCoroutine(WaitSecondsAndReturnTool(seconds));
        }
        else
        {
            surgeonAnimator.SetTrigger("IncorrectTool");
            AddWrongResultRegister(e.snappedObject);
        }
    }
    private void HandleObjectUnsnapped(object sender, SnapDropZoneEventArgs e)
    {
        surgeonAnimator.SetBool("RightHandHasTool", false);

        if (returningTool)
        {
            returningTool = false;
            showMessage.transform.localScale = Vector3.zero;

            float seconds = Random.Range(5f, 10f);
            StartCoroutine(WaitSecondsAndAskTool(seconds));
        }
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
            surgeonAnimator.SetTrigger("AskNewTool");

            showMessage.transform.localScale = Vector3.one;
            messageText.text = string.Format("Necesito la herramienta \"{0}\"", toolsEnumerator.Current.ToString());
            timeToolAsked = GetExerciseTime();
        }
        else
        {
            surgeonAnimator.SetTrigger("SessionEnd");
            LogResultToolGiven();
        }
    }
    #endregion

    #region ReturnTool
    private IEnumerator WaitSecondsAndReturnTool(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        ReturnTool();
    }

    private bool returningTool = false;
    private void ReturnTool()
    {
        surgeonAnimator.SetTrigger("ReturnTool");
        returningTool = true;

        showMessage.transform.localScale = Vector3.one;
        messageText.text = "Recoge la herramienta, por favor";
    }
    #endregion

    #region Log Results Functions
    private void LogResultToolGiven()
    {
        ExerciseFileLogger.Instance.LogResult("Entrega de material a cirujano", toolResults);
    }

    private float GetExerciseTime()
    {
        return ExerciseFileLogger.Instance.exerciseManager.GetExerciseTime();
    }

    private void AddCorrectResultRegister(Tools current)
    {
        string currentTime = ExerciseFileLogger.Instance.exerciseManager.GetExerciseTimeFormatted();
        float elapsedTime = GetExerciseTime() - timeToolAsked;

        toolResults.Add(string.Format("{0} - Instrumental {1} entregado en {2} segundos",
            currentTime,
            current.ToString(),
            elapsedTime.ToString()));

    }
    private void AddWrongResultRegister(GameObject current)
    {
        string currentTime = ExerciseFileLogger.Instance.exerciseManager.GetExerciseTimeFormatted();
        var tool = current.GetComponentInChildren<ToolSceneGameObject>();

        if (tool == null)
        {
            toolResults.Add(string.Format("\t{0} - Instrumental INCORRECTO", currentTime));

        }
        else
        {
            toolResults.Add(string.Format("\t{0} - Instrumental INCORRECTO {1}", currentTime, tool.toolType.ToString()));
        }
    }
    #endregion
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