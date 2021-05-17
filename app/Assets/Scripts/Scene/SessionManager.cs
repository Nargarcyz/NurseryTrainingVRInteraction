using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NT;
using NT.Graph;
using NT.SceneObjects;
using NT.Variables;

using OdinSerializer;
using SimpleJSON;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using XNode;
using Better;
using Better.StreamingAssets;


public class SessionManager : Singleton<SessionManager>, IVariableDelegate
{

    public static SessionData sessionToLoad;

    public static string GetSavePath()
    {
        if (Application.platform == RuntimePlatform.Android)
            return "/Saves/Sessions/";
        else
        {
            if (!Directory.Exists(Application.streamingAssetsPath + "/Saves"))
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Saves");

            if (!Directory.Exists(Application.streamingAssetsPath + "/Saves/Sessions"))
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Saves/Sessions");

            return "/Saves/Sessions/";
        }

    }

    public static string GetTemplatePath()
    {

        if (Application.platform == RuntimePlatform.Android)
            return "/Saves/Templates/";
        else
        {
            if (!Directory.Exists(Application.streamingAssetsPath + "/Saves"))
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Saves");

            if (!Directory.Exists(Application.streamingAssetsPath + "/Saves/Templates"))
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Saves/Templates");

            return "/Saves/Templates/";
        }
    }

    public static string GetNodeGroupPath()
    {

        if (Application.platform == RuntimePlatform.Android)
            return "/Saves/NodeGroups/";
        else
        {
            if (!Directory.Exists(Application.streamingAssetsPath + "/Saves"))
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Saves");

            if (!Directory.Exists(Application.streamingAssetsPath + "/Saves/NodeGroups"))
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Saves/NodeGroups");

            return "/Saves/NodeGroups/";
        }
    }

    public static void CreateSessionFromTemplate(SessionData sd, SessionData template)
    {
        string templatePath = Path.Combine(Application.streamingAssetsPath + GetTemplatePath(), template.sessionID);
        string destPath = Path.Combine(Application.streamingAssetsPath + GetSavePath(), sd.sessionID);


        DirectoryCopy(templatePath, destPath, true);
        // Assure config.nt has the updated data
        string sessionJson = JsonUtility.ToJson(sd);
        // File.WriteAllText(destPath + "/" + "config.nt", sessionJson);
        File.WriteAllText(destPath + "/" + "config.nt", sessionJson);

    }

    private static void DirectoryCopy(string sourceDir, string destDir, bool copySubDirs)
    {
        DirectoryInfo source = new DirectoryInfo(sourceDir);
        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        FileInfo[] files = source.GetFiles();
        foreach (var file in files)
        {
            string destFile = Path.Combine(destDir, file.Name);
            file.CopyTo(destFile, false);
        }


        if (copySubDirs)
        {
            var dirs = source.GetDirectories();
            foreach (var subdir in dirs)
            {
                string destSubPath = Path.Combine(destDir, subdir.Name);
                DirectoryCopy(subdir.FullName, destSubPath, copySubDirs);
            }
        }
    }

    public static SessionData GetSession(string sessionID)
    {
        // string configJSON = File.ReadAllText(GetSavePath() + sessionID + "/" + "config.nt");
        string configJSON = BetterStreamingAssets.ReadAllText(GetSavePath() + sessionID + "/" + "config.nt");
        return JsonUtility.FromJson<SessionData>(configJSON);
    }
    public static SessionData GetTemplateSession(string sessionID)
    {
        // string configJSON = File.ReadAllText(GetTemplatePath() + sessionID + "/" + "config.nt");
        string configJSON = BetterStreamingAssets.ReadAllText(GetTemplatePath() + sessionID + "/" + "config.nt");
        return JsonUtility.FromJson<SessionData>(configJSON);
    }

    public static void DeleteSession(string sessionID)
    {
        string sessionPath = Application.streamingAssetsPath + GetSavePath() + sessionID;
        if (Directory.Exists(sessionPath))
        {
            Directory.Delete(sessionPath, true);

        }
    }

    [Header("Session Data")]
    public SessionData SessionData;
    public bool loadOnAwake = true;
    public bool autoSave = true;
    public float autoSaveInterval = 60f;


    //public static string exportPath = Application.dataPath + "/Saves/Sessions/";


    [Header("References")]
    public SceneGraph sceneGraph;
    public SceneObjects sceneObjects;
    public IMapLoader mapLoader;


    [Space(20)]
    [Header("Debug")]
    public Dictionary<string, SceneGameObject> sceneGameObjects;
    public Dictionary<string, UserVariable> userVariables = new Dictionary<string, UserVariable>();

    [System.Serializable]
    public struct UserVariable
    {
        public object value;
        public object defaultValue;

        public void Reset()
        {
            this.value = defaultValue;
        }
    }

    private SceneGameObject _selectedObjectSceneObject;

    public SceneGameObject selectedSceneObject
    {

        get
        {
            return _selectedObjectSceneObject;
        }

        private set
        {
            if (_selectedObjectSceneObject != value)
            {
                _selectedObjectSceneObject = value;
                OnCurrentChanged.Invoke();
            }
        }
    }

    private NodeGraph _showingGraph;
    public NodeGraph showingGraph
    {
        get
        {
            return _showingGraph;
        }

        set
        {
            if (_showingGraph != value)
            {
                _showingGraph = value;
                OnShowingGraphChanged.Invoke();
            }
        }
    }


    [HideInInspector] public UnityEvent OnUserVariablesModified = new UnityEvent();
    [HideInInspector] public UnityEvent OnCurrentChanged = new UnityEvent();
    [HideInInspector] public UnityEvent OnShowingGraphChanged = new UnityEvent();
    [HideInInspector] public UnityEvent OnGraphListChanged = new UnityEvent();
    [HideInInspector] public UnityEvent OnSessionLoaded = new UnityEvent();
    [HideInInspector] public UnityEvent OnSceneGameObjectsChanged = new UnityEvent();

    private void Awake()
    {

        BetterStreamingAssets.Initialize();
        if (sceneGraph == null)
        {
            sceneGraph = new SceneGraph();
            sceneGraph.variableDelegate = this;
        }

        sceneGameObjects = new Dictionary<string, SceneGameObject>();

        sceneObjects.LoadPrefabs();

        var sceneRoot = new GameObject("SceneRoot");
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(sessionToLoad.sessionID))
        {
            SessionData = sessionToLoad;
        }
        Debug.Log("<color=red>" + sessionToLoad.sessionID + "</color>");

        if (loadOnAwake)
        {
            LoadSession(SessionData.sessionID);
        }
    }


    float timer = 0;
    private void Update()
    {
        if (autoSave)
        {
            timer += Time.deltaTime;

            if (timer > autoSaveInterval)
            {
                Debug.Log("<color=green> AUTOSave </color>");
                timer = 0;
                SaveSession();
            }
        }
    }


    [ContextMenu("Start execution")]
    public void StartExecution()
    {
        //MessageSystem.onMessageSent = null;

        SetUpForExecution();

        MessageSystem.SendMessage("Application Start");
        MessageSystem.SendMessage("Exercise Started");
    }

    public void EndExercise()
    {
        MessageSystem.SendMessage("Application End");
        MessageSystem.SendMessage("Exercise End");
    }

    public void SetUpForExecution()
    {
        Debug.Log("SETTING UP FOR EXECUTION");
        //Reset all variables to default values!
        foreach (var so in sceneGameObjects)
        {
            SceneGameObject scgo = so.Value;

            scgo.data.data.Reset();

            if (scgo.data.graph != null && (scgo.data.graph.nodes.Count > 0 || scgo.data.graph.packedNodes.Count > 0))
            {
                scgo.data.graph.StartExecution();
            }
        }


        var userVars = userVariables.Keys.ToArray();

        foreach (var uv in userVars)
        {
            var v = userVariables[uv];
            v.Reset();
            userVariables[uv] = v;
        }


        sceneGraph.StartExecution();
    }

    public void StartExecutionWithMessage(string message)
    {
        SetUpForExecution();
        MessageSystem.SendMessage(message);

    }

    #region Export/Import

    public void SaveScene(string path)
    {

        byte[] ojson = SerializationUtility.SerializeValue(sceneGameObjects, DataFormat.JSON);

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllBytes(path, ojson);
        }
    }

    [ContextMenu("Save")]
    public void SaveSession()
    {
        if (string.IsNullOrEmpty(SessionData.sessionID))
        {
            SessionData.sessionID = DateTime.Now.ToString();
        }

        string saveFolder = Application.streamingAssetsPath + GetSavePath() + SessionData.sessionID;
        Debug.Log($" Saving session to: {saveFolder} ");

        byte[] sceneGraphData = SerializationUtility.SerializeValue(sceneGraph, DataFormat.JSON);
        byte[] userVariablesData = SerializationUtility.SerializeValue(userVariables, DataFormat.JSON);
        if (sceneGraphData == null && userVariablesData == null && SessionData.sceneFile == null) return;

        if (Directory.Exists(saveFolder))
        {
            Directory.Delete(saveFolder, true);
        }

        Directory.CreateDirectory(saveFolder);


        SessionData.lastModified = DateTime.Now.ToString();



        SessionData.sceneGraphFile = "sceneGraph.nt";
        File.WriteAllBytes(saveFolder + "/" + SessionData.sceneGraphFile, sceneGraphData);

        SessionData.userVariables = "userVariables.nt";
        File.WriteAllBytes(saveFolder + "/" + SessionData.userVariables, userVariablesData);

        SessionData.sceneFile = "scene.nt";
        SaveScene(saveFolder + "/" + SessionData.sceneFile);

        string sessionJSON = JsonUtility.ToJson(SessionData);
        File.WriteAllText(saveFolder + "/" + "config.nt", sessionJSON);

    }

    public void LoadScene(string path)
    {
        // byte[] ojson = File.ReadAllBytes(path);
        byte[] ojson = BetterStreamingAssets.ReadAllBytes(path);

        var newRoot = SerializationUtility.DeserializeValue<Dictionary<string, SceneGameObject>>(ojson, DataFormat.JSON);

        mapLoader.LoadMap(newRoot);
    }

    public void LoadSession(string sessionID)
    {

        string saveFolder = GetSavePath() + sessionID;

        if (string.IsNullOrEmpty(sessionID)) return;

        if (!BetterStreamingAssets.DirectoryExists(saveFolder)) return;
        // if (!Directory.Exists(saveFolder))
        // {
        //     return;
        // }

        sceneGameObjects = new Dictionary<string, SceneGameObject>();


        // string configJSON = File.ReadAllText(saveFolder + "/" + "config.nt");
        string configJSON = BetterStreamingAssets.ReadAllText(saveFolder + "/" + "config.nt");
        SessionData = JsonUtility.FromJson<SessionData>(configJSON);

        if (BetterStreamingAssets.FileExists(saveFolder + "/" + SessionData.sceneGraphFile))
        // if (File.Exists(saveFolder + "/" + SessionData.sceneGraphFile))
        {
            // byte[] sceneGraphData = File.ReadAllBytes(saveFolder + "/" + SessionData.sceneGraphFile);
            byte[] sceneGraphData = BetterStreamingAssets.ReadAllBytes(saveFolder + "/" + SessionData.sceneGraphFile);
            sceneGraph = SerializationUtility.DeserializeValue<SceneGraph>(sceneGraphData, DataFormat.JSON);
            sceneGraph.variableDelegate = this;
        }

        // if (File.Exists(saveFolder + "/" + SessionData.userVariables))
        if (BetterStreamingAssets.FileExists(saveFolder + "/" + SessionData.userVariables))
        {
            // byte[] userVariablesData = File.ReadAllBytes(saveFolder + "/" + SessionData.userVariables);
            byte[] userVariablesData = BetterStreamingAssets.ReadAllBytes(saveFolder + "/" + SessionData.userVariables);
            userVariables = SerializationUtility.DeserializeValue<Dictionary<string, UserVariable>>(userVariablesData, DataFormat.JSON);
        }

        LoadScene(saveFolder + "/" + SessionData.sceneFile);

        OnSessionLoaded.Invoke();

        showingGraph = sceneGraph;
    }
    #endregion

    #region Scene GameObjcts
    public void AddSceneGameObject(SceneGameObject so)
    {
        sceneGameObjects.Add(so.data.id, so);

        if (so.data.graph != null)
        {
            so.data.graph.variableDelegate = this;
        }

        OnSceneGameObjectsChanged.Invoke();
        OnGraphListChanged.Invoke();
    }

    public void RemoveSceneGameObject(string key)
    {

        if (sceneGameObjects.ContainsKey(key))
        {
            SceneGameObject toRemove = sceneGameObjects[key];
            sceneGameObjects.Remove(key);

            if (toRemove.data.graph != null)
            {
                showingGraph = sceneGraph;
            }

            if (!string.IsNullOrEmpty(toRemove.data.parent))
            {
                SceneGameObject toRemoveParent = sceneGameObjects[toRemove.data.parent];
                toRemoveParent.data.childs.Remove(key);

                Debug.Log("Removed from parent!");
            }

            List<SceneGameObject> childsToRemove = new List<SceneGameObject>(toRemove.GetComponentsInChildren<SceneGameObject>());

            foreach (SceneGameObject childToRemove in childsToRemove)
            {
                string childKey = childToRemove.data.id;
                sceneGameObjects.Remove(childKey);
            }

            Destroy(toRemove.gameObject);
            OnSceneGameObjectsChanged.Invoke();
            OnGraphListChanged.Invoke();
        }
    }

    public void SetSelected(string key)
    {

        if (selectedSceneObject != null) selectedSceneObject.isSelected = false;

        if (!string.IsNullOrEmpty(key) && sceneGameObjects.ContainsKey(key))
        {
            selectedSceneObject = sceneGameObjects[key];
            selectedSceneObject.isSelected = true;
        }
        else
        {
            selectedSceneObject = null;
        }
    }

    public SceneGameObject GetSceneGameObject(string key)
    {
        if (sceneGameObjects.ContainsKey(key))
        {
            return sceneGameObjects[key];
        }
        else
        {
            return null;
        }
    }

    public List<SceneGameObject> GetSceneGameObjectsWithTag(string tag)
    {
        var result = sceneGameObjects.Where(g => g.Value.CompareTag(tag)).Select(g => g.Value).ToList();
        return result;
    }
    #endregion

    #region  Graph functions
    public void OpenGraphFor(string key)
    {
        SceneGameObject sobj = GetSceneGameObject(key);

        if (sobj == null) return;

        if (sobj.data.graph == null)
        {
            SceneObjectGraph soc = new SceneObjectGraph();
            soc.linkedNTVariable = sobj.data.id;
            soc.variableDelegate = this;

            sobj.data.graph = soc;
        }

        showingGraph = sobj.data.graph;
        SetSelected(key);

        OnGraphListChanged.Invoke();

    }

    public void OpenSceneGraph()
    {
        showingGraph = sceneGraph;
    }

    public List<SceneObjectGraph> GetAllGraphs()
    {
        List<SceneObjectGraph> graphs = new List<SceneObjectGraph>();

        if (sceneGameObjects == null) return graphs;

        foreach (var sceneGameObject in sceneGameObjects)
        {
            if (sceneGameObject.Value.data.graph != null &&
                (sceneGameObject.Value.data.graph.nodes.Count > 0 || sceneGameObject.Value.data.graph == showingGraph))
            {
                SceneObjectGraph sog = sceneGameObject.Value.data.graph;

                sog.linkedNTVariable = sceneGameObject.Value.data.id;
                sog.displayName = sceneGameObject.Value.sceneObject.GetDisplayName();

                if (sog.variableDelegate == null) sog.variableDelegate = this;

                graphs.Add(sceneGameObject.Value.data.graph);
            }
        }

        return graphs;
    }

    public object GetValue(string key)
    {
        SceneGameObject scgo = GetSceneGameObject(key);
        if (scgo != null)
        {
            return scgo.data.data.GetValue();
        }

        return null;
    }

    public void SetValue(string key, object value)
    {
        SceneGameObject scgo = GetSceneGameObject(key);
        if (scgo != null)
        {
            scgo.data.data.SetValue(value);
        }
    }


    public void SetDefaultUserVariable(string key, object value)
    {
        if (userVariables.ContainsKey(key))
        {

            userVariables[key] = new UserVariable { value = value, defaultValue = value };
        }
        else
        {
            userVariables.Add(key, new UserVariable { value = value, defaultValue = value });
            OnUserVariablesModified.Invoke();
        }
    }

    public void RemoveUserVariable(string key)
    {
        if (userVariables.ContainsKey(key))
        {
            userVariables.Remove(key);
        }

        OnUserVariablesModified.Invoke();
    }



    public object GetUserVariable(string key)
    {
        if (userVariables.ContainsKey(key))
        {
            return userVariables[key].value;
        }

        return null;
    }

    public void SetUserVariable(string key, object value)
    {
        if (userVariables.ContainsKey(key))
        {

            UserVariable uv = userVariables[key];
            uv.value = value;
            userVariables[key] = uv;
        }
    }

    public void Quit()
    {
        SaveSession();
        Application.Quit();
    }

    public void Back()
    {
        SaveSession();
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }


    #endregion


}

[System.Serializable]
public struct SessionData
{
    public string displayName;
    public string sessionID;
    public string lastModified;
    public string sceneGraphFile;
    public string sceneFile;
    public string userVariables;
}

