using UnityEngine;

namespace Sensors
{
    public class DeterministicLdCameraSensor : MonoBehaviour, ISensor<byte[]>
    {
        private Camera _capture;
        private int _imgName = 0;
        
        private void Start()
        {
            _capture = GameObject.Find("DeterministicLaneDetectionCamera").GetComponent<Camera>();
        }
        
        // void OnPostRender()
        // {
        //     if (tick == fps)
        //     {
        //         captureCamera.targetTexture = RenderTexture.GetTemporary(800, 600, 16);
        //         RenderTexture renderTexture = captureCamera.targetTexture;
        //         
        //         Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        //         texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0, false);
        //         
        //         byte[] image = texture.EncodeToPNG();
        //         File.WriteAllBytes("calibration/" + _imgName + ".png", image);
        //         _imgName++;
        //         tick = 0;
        //         
        //         RenderTexture.ReleaseTemporary(renderTexture);
        //         captureCamera.targetTexture = null;
        //         Debug.Log("Screenshot taken");
        //     }
        //     tick++;
        // }
    
        public byte[] ReadData()
        {
            var camTargetTexture = _capture.targetTexture;
            RenderTexture targetTexture = camTargetTexture;
            RenderTexture.active = camTargetTexture;
            Texture2D texture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0, false);
            texture.Apply();
            var image = texture.EncodeToPNG();
            //File.WriteAllBytes("/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server/DeterministicLaneDetection/Pics_LD/" + _imgName + ".png", image);
            _imgName++;
            //Debug.Log("Snapped");
            return image;
        }
    }
}
