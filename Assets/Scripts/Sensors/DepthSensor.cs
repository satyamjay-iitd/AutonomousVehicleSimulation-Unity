using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Sensors
{
    public class DepthSensor : MonoBehaviour, ISensor<byte[]>
    {
        private Camera _depthCamera;
        void Start()
        {
            _depthCamera = GameObject.Find("DepthCamera").GetComponent<Camera>();
        }

        public byte[] ReadData()
        {
            var camTargetTexture = _depthCamera.targetTexture;
            var targetTexture = camTargetTexture;
            RenderTexture.active = camTargetTexture;
            var texture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0, false);
            texture.Apply();
            var image = texture.EncodeToPNG();
            DestroyImmediate(texture);
            return image;
        }
    }
}