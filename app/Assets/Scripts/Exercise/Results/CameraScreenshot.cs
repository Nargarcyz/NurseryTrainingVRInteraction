using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraScreenshot : MonoBehaviour
{
    public Camera screenshotCamera;
    public Collider triggerCollider;

    public int resWidth = 1920;
    public int resHeight = 1080;

    public float waitSeconds = 3;

    public void TakeScreenshotAfterSeconds()
    {
        StartCoroutine(WaitForSecondsAndScreenshot());
    }

    private IEnumerator WaitForSecondsAndScreenshot()
    {
        yield return new WaitForSeconds(waitSeconds);
        StartCoroutine(TakeScreenshot());
    }

    private IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        screenshotCamera.targetTexture = rt;
        screenshotCamera.Render();
        RenderTexture.active = rt;

        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenshotCamera.targetTexture = null;
        RenderTexture.active = null; // added to avoid errors
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToJPG(); // choose encoding format
        string filename = ScreenshotName(resWidth, resHeight, "jpg");
        System.IO.File.WriteAllBytes(filename, bytes);

        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }

    private string ScreenshotName(int width, int height, string format)
    {
        string folder = GetScreenshotPath();
        return string.Format("{0}/screen_{1}_{2}x{3}_{4}.{5}",
            folder,
            SessionManager.Instance.GetInstanceID(),
            width, height,
            System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
            format);
    }

    private string GetScreenshotPath()
    {
        string path = Application.dataPath + "/Results";
        FolderExistsOrCreate(path);

        path += "/Screenshots";
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
}
