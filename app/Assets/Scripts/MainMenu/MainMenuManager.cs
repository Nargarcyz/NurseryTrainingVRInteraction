using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    public Transform content;
    public GameObject emptyList;
    public GameObject sessionPrototype;
    public GameObject confirmDelete;

    public TMP_InputField sessionID;

    private Dictionary<string, GameObject> sessions = new Dictionary<string, GameObject>();

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Awake() {
        DirectoryInfo sessionsDir = new DirectoryInfo(SessionManager.GetSavePath());

        if(!sessionsDir.Exists) {
            emptyList.SetActive(true);
            return;
        }

        FileInfo[] files = sessionsDir.GetFiles("config.nt", SearchOption.AllDirectories);
        
        if(files.Length == 0) {
            emptyList.SetActive(true);
        }
        else
        {
            emptyList.SetActive(false);

            var sessionsData = files
                .Select(x => SessionManager.GetSession(x.Directory.Name))
                .OrderBy(x => x.displayName);

            foreach(SessionData sd in sessionsData)
            {
                GameObject itemCard = Instantiate(sessionPrototype, content);
                itemCard.SetActive(true);
                itemCard.GetComponent<MenuCard>().SetSessionData(sd, this);
                sessions.Add(sd.sessionID, itemCard);
            }
        }

    }

    public void PlayeSession(SessionData sessionData)
    {
         SessionManager.sessionToLoad = sessionData;
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    public void EditSession(SessionData sessionData)
    {
        SessionManager.sessionToLoad = sessionData;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void RemoveSession(SessionData sessionData)
    {
        confirmDelete.SetActive(false);

        if(sessions.ContainsKey(sessionData.sessionID)){
            GameObject go = sessions[sessionData.sessionID];
            Destroy(go);
            sessions.Remove(sessionData.sessionID);
        }

        SessionManager.DeleteSession(sessionData.sessionID);
    }

    public void CreateFromWindow(){
        SessionData data = new SessionData();

        data.sessionID = Guid.NewGuid().ToString();
        data.displayName = sessionID.text;

        EditSession(data);
    }

    public void ConfirmRemoveSession(SessionData sessionData)
    {
        confirmDelete.SetActive(true);

        var textBody = confirmDelete.transform.Find("Text Body").GetComponentInChildren<TextMeshProUGUI>();
        textBody.text = $"Are you sure you want to delete the session \"{sessionData.displayName}\"?";

        var button = confirmDelete.transform
            .Find("ConfirmDeleteButton")
            .GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => RemoveSession(sessionData));
    }
}