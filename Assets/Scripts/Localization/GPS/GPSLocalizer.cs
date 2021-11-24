using System.Collections.Generic;
using System.Diagnostics;
using Controller;
using IPC;
using Newtonsoft.Json;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Localization.GPS
{
    [RequireComponent(typeof(Sensors.Gps))]
    public class GpsLocalizer : MonoBehaviour, ILocalizer
    {
        private Sensors.Gps _gps;
        private Process _process;

        void Start()
        {
            var context = new ProcessStartInfo
            {
                FileName = "/home/janib/anaconda3/envs/spconv/bin/python",
                Arguments = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server/GPSLocalizationProcess.py",
                WorkingDirectory = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server",
                UseShellExecute = true,
            }; 
            _process = Process.Start(context);
            _gps = gameObject.GetComponent<Sensors.Gps>();
            Debug.Log("Waiting for the Map to load");
            while (Ipc.IsMapProviderReady() == false)
            {
            }
            Debug.Log("Map Loaded");
            
        }

        private void LateUpdate()
        {
            var (pos, dir) = _gps.ReadData();
            SensorStore.LocalizationOutput = new LocalizationOutput();
            SensorStore.LocalizationOutput.CarDirX = dir.x;
            SensorStore.LocalizationOutput.CarDirY = dir.y;
            if (Ipc.IsMapProviderReady())
            {
                var output = Ipc.ReadMapOutput();
                var deserializedObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(output, new LocalizationOutputDeserializer());
                SensorStore.LocalizationOutput.CarPosX = float.Parse(deserializedObject["CurrPosX"]);
                SensorStore.LocalizationOutput.CarPosY = float.Parse(deserializedObject["CurrPosY"]);
                SensorStore.LocalizationOutput.NextNodeRx = float.Parse(deserializedObject["NextNodeRX"]);
                SensorStore.LocalizationOutput.NextNodeRy = float.Parse(deserializedObject["NextNodeRY"]);
                SensorStore.LocalizationOutput.NextNodeLx = float.Parse(deserializedObject["NextNodeLX"]);
                SensorStore.LocalizationOutput.NextNodeLy = float.Parse(deserializedObject["NextNodeLY"]);
                SensorStore.LocalizationOutput.HasReachedDestination = bool.Parse(deserializedObject["HasReachedDestination"]);
                var toSend = pos.x + " " + pos.y;
                Ipc.WriteUnityCoordinate(toSend);
                Ipc.UnsetMapOutputReady();
            }
        }

        private void OnApplicationQuit()
        {
            _process.Kill();
        }
    }
}

