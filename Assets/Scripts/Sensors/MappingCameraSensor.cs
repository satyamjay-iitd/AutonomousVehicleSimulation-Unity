using System;
using UnityEngine;

namespace Sensors
{
    public class MappingCameraSensor : MonoBehaviour, ISensor<(byte[], byte[])>
    {
        private Camera _cameraLeft;
        private Camera _cameraRight;
        public string cameraName;

        private void Awake()
        {
            _cameraLeft = GameObject.Find("OrbSlamCameraLeft").GetComponent<Camera>();
            _cameraRight = GameObject.Find("OrbSlamCameraRight").GetComponent<Camera>();
        }
        
        public (byte[], byte[]) ReadData()
        {
            var camTargetTexture = _cameraLeft.targetTexture;
            var targetTexture = camTargetTexture;
            RenderTexture.active = camTargetTexture;
            var texture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0, false);
            texture.Apply();
            var leftImage = texture.EncodeToPNG();
            
            camTargetTexture = _cameraRight.targetTexture;
            targetTexture = camTargetTexture;
            RenderTexture.active = camTargetTexture;
            texture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0, false);
            texture.Apply();
            var rightImage = texture.EncodeToPNG();
            return (leftImage, rightImage);
        }
    }
}