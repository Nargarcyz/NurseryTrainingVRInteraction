using System.IO;
using UnityEngine;

public class ExerciseFileLogger : MonoBehaviour
{
    public static void LogMessageToFile(string folder, string filename, string message)
    {
        CheckFolderExists(folder);
        string filePath = folder + filename;
        WriteMessageInFile(filePath, message);
    }

    private static void CheckFolderExists(string folder)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
    }

    private static void WriteMessageInFile(string path, string message)
    {
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.WriteLine(message);
        }
    }
}
