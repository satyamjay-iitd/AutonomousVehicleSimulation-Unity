using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Controller;
using IPC;
using Newtonsoft.Json;
using UnityEngine;
using Sensors;
using Debug = UnityEngine.Debug;

namespace Localization.ORBSLAM
{
    [RequireComponent(typeof (RGBDSensor))]
    [RequireComponent(typeof(Gps))]
    public class OrbSlamLocalizer : MonoBehaviour, ILocalizer
    {
        private Process _process;
        private Gps _gps;
        private RGBDSensor _rgbdSensor;
        private DateTime _startTime; 
        
        // Start is called before the first frame update
        void Start()
        {
            var context = new ProcessStartInfo
            {
                FileName = "/home/janib/anaconda3/envs/spconv/bin/python",
                Arguments = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server/MappingProcess.py --localization-only true",
                WorkingDirectory = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server",
                UseShellExecute = true,
            };
            _process = Process.Start(context);
            _rgbdSensor = gameObject.GetComponent<RGBDSensor>();
            _gps = gameObject.GetComponent<Gps>();
            Debug.Log("Waiting for the ORB-SLAM Localization process to start");
            while (Ipc.IsMapperReadyToReceive() == false) {}

            var (leftImage, rightImage) = _rgbdSensor.ReadData();
            _startTime = DateTime.Now;
            Ipc.WriteMappingImgAndTime(Convert.ToBase64String(leftImage), 
                Convert.ToBase64String(rightImage), "0");
            Ipc.UnsetMapperReadyToReceive();
            
            SensorStore.LocalizationOutput = new LocalizationOutput();
        }

        private void LateUpdate()
        {
            var (_, dir) = _gps.ReadData();
            SensorStore.LocalizationOutput.CarDirX = dir.x;
            SensorStore.LocalizationOutput.CarDirY = dir.y;
            
            if (Ipc.IsMapperReadyToReceive())
            {
                var localizationOutput = new LocalizationOutput();    
                var output = Ipc.ReadMapperOutput();
                var deserializedObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(output, new LocalizationOutputDeserializer());
                localizationOutput.CarPosX = float.Parse(deserializedObject["CurrPosX"]);
                localizationOutput.CarPosY = float.Parse(deserializedObject["CurrPosY"]);
                localizationOutput.NextNodeRx = float.Parse(deserializedObject["NextNodeRX"]);
                localizationOutput.NextNodeRy = float.Parse(deserializedObject["NextNodeRY"]);
                localizationOutput.NextNodeLx = float.Parse(deserializedObject["NextNodeLX"]);
                localizationOutput.NextNodeLy = float.Parse(deserializedObject["NextNodeLY"]);
                localizationOutput.HasReachedDestination = bool.Parse(deserializedObject["HasReachedDestination"]);
                var (leftImage, rightImage) = _rgbdSensor.ReadData();
                Ipc.WriteMappingImgAndTime(Convert.ToBase64String(leftImage), 
                    Convert.ToBase64String(rightImage),
                    DateTime.Now.Subtract(_startTime).TotalMilliseconds.ToString("0.0000"));
                Ipc.UnsetMapperReadyToReceive();

                localizationOutput.CarDirX = dir.x;
                localizationOutput.CarDirY = dir.y;
                SensorStore.LocalizationOutput = localizationOutput;
            }
        }
        private void OnApplicationQuit()
        {
            _process.Close();
        }
    }
}