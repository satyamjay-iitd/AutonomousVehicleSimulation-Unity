using System;
using System.Diagnostics;
using Controller;
using IPC;
using Newtonsoft.Json;
using UnityEngine;
using Sensors;
using Debug = UnityEngine.Debug;

namespace Perception.LaneDetection
{
    [RequireComponent(typeof (DeterministicLdCameraSensor))]
    public class DeterministicLd : LaneDetection
    {
        private Process _process;
        private DeterministicLdCameraSensor _ldCameraSensor;

        private void Start()
        {
            var context = new ProcessStartInfo
            {
                FileName = "/home/janib/anaconda3/envs/spconv/bin/python",
                Arguments = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server/DeterministicLaneDetectionProcess.py",
                WorkingDirectory = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server",
                UseShellExecute = true,
            };
            _process = Process.Start(context);
            _ldCameraSensor = gameObject.GetComponent<DeterministicLdCameraSensor>();
            Debug.Log("Waiting for the DeterministicLd process to start");
            while (Ipc.IsDeterministicLdOutputReady() == false) {}

            // SensorStore.IsLaneDetectionAvailable = true;
        }
        private void LateUpdate()
        {
            if (Ipc.IsDeterministicLdOutputReady()) {
                var output = Ipc.ReadDeterministicLdOutput();
                var deserializedObject = JsonConvert.DeserializeObject<DeterministicLdOutput>(output);
                SensorStore.LdOutput.IsLaneDetected = deserializedObject.IsLaneDetected;
                if (SensorStore.LdOutput.IsLaneDetected)
                {
                    SensorStore.LdOutput.Offset = deserializedObject.Offset;
                    SensorStore.LdOutput.SteeringAngle = deserializedObject.SteeringAngle;

                    SensorStore.LdOutput.NumLeftLanes = deserializedObject.NumLeftLanes;
                    SensorStore.LdOutput.NumRightLanes = deserializedObject.NumRightLanes;
                }

                var s = Convert.ToBase64String(_ldCameraSensor.ReadData());
                Ipc.WriteDeterministicLdImg(s);
                Ipc.UnsetDeterministicLdOutputReady();
            }
        }
        private void OnApplicationQuit()
        {
            _process.Kill();
        }
    }
    
}