using System;
using System.IO;
using System.Runtime.InteropServices;
using IPC;
using Newtonsoft.Json;
using UnityEngine;
using Sensors;
using Debug = UnityEngine.Debug;

namespace Mapping.OrbSlam
{
    [RequireComponent(typeof (RGBDSensor))]
    public class OrbSlamMapper : MonoBehaviour, IMapper
    {
        private RGBDSensor _rgbdSensor;
        private DateTime _startTime; 


        private void Start()
        {
            _rgbdSensor = gameObject.GetComponent<RGBDSensor>();
            Debug.Log("Waiting for the Mapping process to start");
            Debug.Log(gameObject.transform.Find("StereoCamera").gameObject.transform.Find("OrbSlamCameraLeft").gameObject.GetComponent<Transform>().position.ToString());
            Debug.Log(gameObject.transform.Find("StereoCamera").gameObject.transform.Find("OrbSlamCameraRight").gameObject.GetComponent<Transform>().position.ToString());
            while (Ipc.IsMapperReadyToReceive() == false) {}
            
            var (leftImage, rightImage) = _rgbdSensor.ReadData();
            _startTime = DateTime.Now;
            
            Ipc.WriteMappingImgAndTime(Convert.ToBase64String(leftImage), 
                                       Convert.ToBase64String(rightImage), "0");
            Ipc.UnsetMapperReadyToReceive();
        }
        private void LateUpdate()
        {
            
            
            if (Ipc.IsMapperReadyToReceive())
            {
                
                var output = Ipc.ReadMapperOutput();
                var deserializedObject = JsonConvert.DeserializeObject<OrbSlamOutput>(output);
               
                var (leftImage, rightImage) = _rgbdSensor.ReadData();
                Ipc.WriteMappingImgAndTime(Convert.ToBase64String(leftImage), 
                                           Convert.ToBase64String(rightImage),
                                           DateTime.Now.Subtract(_startTime).TotalMilliseconds.ToString("0.0000"));
                Ipc.UnsetMapperReadyToReceive();
            }
        }
        
        private void OnApplicationQuit()
        {
            
        }
    }
}

