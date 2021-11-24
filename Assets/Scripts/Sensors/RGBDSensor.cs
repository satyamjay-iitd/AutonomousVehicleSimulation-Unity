using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Sensors
{
    public class RGBDSensor : MonoBehaviour, ISensor<(byte[], byte[])>
    {
        // private Camera _rgbCamera;
        // private Camera _depthCamera;
        private Camera _stereoCameraL;
        private Camera _stereoCameraR;
        
        private void Awake()
        {
            // _rgbCamera = GameObject.Find("RGB").GetComponent<Camera>();
            // _depthCamera = GameObject.Find("Depth").GetComponent<Camera>();
            _stereoCameraL = GameObject.Find("OrbSlamCameraLeft").GetComponent<Camera>();
            _stereoCameraR = GameObject.Find("OrbSlamCameraRight").GetComponent<Camera>();
        }

        public (byte[], byte[]) ReadData()
        {
            var camTargetTexture = _stereoCameraR.targetTexture;
            var targetTexture = camTargetTexture;
            RenderTexture.active = camTargetTexture;
            var texture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0, false);
            texture.Apply();
            var leftImage = texture.EncodeToPNG();
            DestroyImmediate(texture);

            camTargetTexture = _stereoCameraL.targetTexture;
            targetTexture = camTargetTexture;
            RenderTexture.active = camTargetTexture;
            texture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0, false);
            texture.Apply();
            var rightImage = texture.EncodeToPNG();
            DestroyImmediate(texture);
            
            return (leftImage, rightImage);
        }
    }
}

