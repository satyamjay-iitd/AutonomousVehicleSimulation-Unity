using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class ScreenREcorder : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera cam;
    private int count = 0;
    void Start()
    {
        cam =  GameObject.Find("DeterministicLaneDetectionCamera").GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        var camTargetTexture = cam.targetTexture;
        var targetTexture = camTargetTexture;
        RenderTexture.active = camTargetTexture;
        var texture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0, false);
        texture.Apply();
        var leftImage = texture.EncodeToPNG();
        File.WriteAllBytes(  "/home/janib/Downloads/images/"+count+".png" , leftImage );
        count++;
        DestroyImmediate(texture);
    }
}
