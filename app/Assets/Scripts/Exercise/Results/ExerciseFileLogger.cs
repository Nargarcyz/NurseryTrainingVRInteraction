using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExerciseFileLogger : Singleton<ExerciseFileLogger>
{
    public ExerciseManager exerciseManager;

    #region Log Main Functions
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
        string header = new string('*', 40);
        using (StreamWriter sw = new StreamWriter(path, append: true))
        {
            sw.WriteLine(header);
            sw.WriteLine(title);
            sw.WriteLine("Time: " + exerciseManager.GetExerciseTimeFormatted());
            sw.WriteLine(header);

            foreach (var line in message)
            {
                sw.WriteLine(line);
            }
        }
    }
    #endregion

    #region Log Auxiliary Functions
    private string GetLogFilename()
    {
        string folder = GetLogPath();
        return string.Format("{0}/sessionLog_{1}_{2}.{3}",
            folder,
            System.DateTime.Now.ToString("yyyy-MM-dd"),
            exerciseManager.GetInstanceID(),
            "txt");
    }

    private string GetLogPath()
    {
        string path = Application.dataPath + "/Results";
        FolderExistsOrCreate(path);

        path += "/SessionLog";
        FolderExistsOrCreate(path);

        return path;
    }

    private void FolderExistsOrCreate(string folder)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
    }
    #endregion

}
