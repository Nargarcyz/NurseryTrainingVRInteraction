using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScreenshot : MonoBehaviour
{
    public Camera screenshotCamera;

    public int resWidth = 1920;
    public int resHeight = 1080;

    public void TakeScreenshot()
    {
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        screenshotCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        screenshotCamera.Render();
        RenderTexture.active = rt;

        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenshotCamera.targetTexture = null;
        RenderTexture.active = null; // added to avoid errors
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName(resWidth, resHeight);
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }

    private string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshots/screen_{1}_{2}x{3}_{4}.png",
            Application.dataPath,
            SessionManager.Instance.GetInstanceID(),
            width, height,
            System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }
}
