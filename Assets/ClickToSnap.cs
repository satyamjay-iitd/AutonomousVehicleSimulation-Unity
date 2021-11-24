using System.IO;
using UnityEngine;

public class ClickToSnap : MonoBehaviour
{

    public Camera leftCamera;
    public Camera rightCamera;
    private int _id = 0;

    private void Capture(Camera _cam, int cameraSide)
    {
        var camTargetTexture = _cam.targetTexture;
        RenderTexture targetTexture = camTargetTexture;
        RenderTexture.active = camTargetTexture;
        Texture2D texture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0, false);
        texture.Apply();
        var image = texture.EncodeToPNG();
        
        File.WriteAllBytes("calibration/Stereo/"+cameraSide+"/" + _id + ".png", image);
        Debug.Log("Screenshot taken");
    }
    void Update()
    {
        if (Input.GetKey (KeyCode.Space))
        {
            Capture(leftCamera, 1);
            Capture(rightCamera, 2);
            _id++;
        }
    }
}
