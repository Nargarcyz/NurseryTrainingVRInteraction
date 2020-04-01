using NT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using TMPro;
using UnityEngine.SceneManagement;

public class ExerciseManager : MonoBehaviour
{

	public VRTK_ControllerEvents leftController;
	public VRTK_ControllerEvents rightController;

    public GameObject menu;
    public GameObject endMenu;

    #region Session Timer
    public float _startTime { get; private set; }

    public float GetExerciseTime()
    {
        return Time.time - _startTime;
    }

    public string GetExerciseTimeFormatted()
    {
        var time = GetExerciseTime();
        TimeSpan t = TimeSpan.FromSeconds(time);
        return t.ToString("g");
    }
    #endregion
    private void Start()
    {
        _startTime = Time.time;
    }

    private void RecieveMessage(string msg)
    {
        if(msg.Contains("Fail Session /"))
        {
            endMenu.SetActive(true);
            menu.SetActive(true);

            float grade = float.Parse(msg.Split('/')[1]);

            endMenu.GetComponentInChildren<TextMeshProUGUI>().text = "The session has been failed with an accuracy of " + grade;
        }   
        
        if (msg.Contains("Pass Session /"))
        {

            endMenu.SetActive(true);
            menu.SetActive(true);

            float grade = float.Parse(msg.Split('/')[1]);

            endMenu.GetComponentInChildren<TextMeshProUGUI>().text = "The session has ended succesfully with an accuracy of " + grade; ;
        }
    }

    protected virtual void OnEnable()
	{
		if (leftController != null)
		{
			leftController.ButtonTwoPressed += TogglePause;
		}

		if (rightController != null)
		{
			rightController.ButtonTwoPressed += TogglePause;
		}
    }

	protected virtual void OnDisable()
	{
		if (leftController != null)
		{
			leftController.ButtonTwoPressed -= TogglePause;
		}

		if (rightController != null)
		{
			rightController.ButtonTwoPressed -= TogglePause;
		}
    }

    public void TogglePause(object sender, ControllerInteractionEventArgs e)
    {
        menu.gameObject.SetActive(!menu.activeInHierarchy);

        if (menu.gameObject.activeInHierarchy)
        {
            MessageSystem.SendMessage("Pause");
            ExerciseFileLogger.Instance.LogMessage("Se ha abierto el menú principal", true);
        }
        else
        {
            MessageSystem.SendMessage("Resume");
            ExerciseFileLogger.Instance.LogMessage("Se ha cerrado el menú principal", true);
        }
    }


	public void StartExercise(){
        SessionManager.Instance.StartExecution();
        MessageSystem.onMessageSent += RecieveMessage;
        TogglePause(this, new ControllerInteractionEventArgs());
        //rightController.buttonTwoPressed;
    }

    public void EndExercise()
    {
        SessionManager.Instance.EndExercise();
    }

    public void Exit(){
        MessageSystem.onMessageSent -= RecieveMessage;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }


}
