using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExerciseFileLogger : Singleton<ExerciseFileLogger>
{
    public ExerciseManager exerciseManager;
    private string currentDate;

    private void Start()
    {
        currentDate = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        // Initial write
        string filePath = GetLogFilename();
        WriteHeaderIfNotExists(filePath);
    }

    #region Log Main Functions
    private void WriteHeaderIfNotExists(string path)
    {
        if (!File.Exists(path))
        {
            string text = string.Format("/// Nombre sesión: {0}      Fecha de ejecución: {1} ///",
                SessionManager.Instance.SessionData.displayName,
                System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            string header = new string('/', text.Length);

            using (StreamWriter sw = new StreamWriter(path, append: true))
            {
                sw.WriteLine(header);
                sw.WriteLine(text);
                sw.WriteLine(header);
                sw.WriteLine();
            }
        }
    }

    public void LogMessage(string message, bool timestamp)
    {
        string filePath = GetLogFilename();
        WriteMessageInFile(filePath, message, timestamp);
    }


    public void LogMessage(string[] message, bool timestamp)
    {
        string filePath = GetLogFilename();
        WriteMessageInFile(filePath, message, timestamp);
    }
    public void LogResult(string title, string[] message)
    {
        string filePath = GetLogFilename();
        WriteResultInFile(filePath, title, message);
    }

    public void LogMessage(List<string> message, bool timestamp) => LogMessage(message.ToArray(), timestamp);
    public void LogResult(string title, List<string> message) => LogResult(title, message.ToArray());

    private void WriteMessageInFile(string path, string message, bool timestamp)
    {
        message = timestamp ? exerciseManager.GetExerciseTimeFormatted() + " - " + message : message;

        using (StreamWriter sw = new StreamWriter(path, append: true))
        {
            sw.WriteLine(message);
        }
    }
    private void WriteMessageInFile(string path, string[] message, bool timestamp)
    {
        string currentTime = exerciseManager.GetExerciseTimeFormatted();

        using (StreamWriter sw = new StreamWriter(path, append: true))
        {
            foreach (var line in message)
            {
                var row = timestamp ? currentTime + " - " + line : line;
                sw.WriteLine(row);
            }
        }
    }
    private void WriteResultInFile(string path, string title, string[] message)
    {
        string header = new string('*', title.Length + 16);
        using (StreamWriter sw = new StreamWriter(path, append: true))
        {
            sw.WriteLine();
            sw.WriteLine(header);
            sw.WriteLine("\t\t" + title);
            sw.WriteLine("\t\tTime: " + exerciseManager.GetExerciseTimeFormatted());
            sw.WriteLine(header);

            foreach (var line in message)
            {
                sw.WriteLine(line);
            }

            sw.WriteLine(header);
            sw.WriteLine("\t* FIN * " + title);
            sw.WriteLine(header);
            sw.WriteLine();
        }
    }
    #endregion

    #region Log Auxiliary Functions
    private string GetLogFilename()
    {
        string folder = GetLogPath();
        return string.Format("{0}/sessionLog_{1}_{2}.{3}",
            folder,
            currentDate,
            exerciseManager.GetInstanceID(),
            "txt");
    }

    private string GetLogPath()
    {
        // Use Application.persistentDataPath
        string path = Application.streamingAssetsPath + "/Results";
        FolderExistsOrCreate(path);

        path += "/SessionLog";
        FolderExistsOrCreate(path);

        return path;
    }

    private void FolderExistsOrCreate(string folder)
    {
        // if (!Directory.Exists(folder))
        // {
        //     Directory.CreateDirectory(folder);
        // }


        // Don't use this when reenabling loggin, use commented one on top
        if (!BetterStreamingAssets.DirectoryExists(folder))
        {
            Directory.CreateDirectory(folder);
        }
    }
    #endregion

}
