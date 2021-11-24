using System;
using System.Collections.Generic;
using System.Diagnostics;
using Controller;
using IPC;
using Newtonsoft.Json;
using Perception.LaneDetection;
using Sensors;
using TMPro.Examples;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Perception.ObstacleDetection
{
    [RequireComponent (typeof(DepthSensor))]
    public class PCSegmentationOd : ObstacleDetection
    {
        private Process _process;
        private DepthSensor _sensor;
        private void Start()
        {
            var context = new ProcessStartInfo
            {
                FileName = "/home/janib/anaconda3/envs/spconv/bin/python",
                Arguments = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server/PointCloudSegmentation.py",
                WorkingDirectory = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server",
                UseShellExecute = true,
            };
            _process = Process.Start(context);
            _sensor = gameObject.GetComponent<DepthSensor>();
            Debug.Log("Waiting for the Point Cloud Segmentation process to start");
            while (Ipc.IsPCSegmentationOutputReady() == false) {}
        }
        private void LateUpdate()
        {
            if (Ipc.IsPCSegmentationOutputReady())
            {
                SensorStore.OdOutput = JsonConvert.DeserializeObject<PCSegmentationOutput>(Ipc.ReadPCSegmentationOutput());
                var s = Convert.ToBase64String(_sensor.ReadData());
                Ipc.WriteDepth(s);
                Ipc.UnsetPCSegmentationOutputReady();
            }
        }
        private void OnApplicationQuit()
        { 
            _process.Kill();
        }
        
        private class PCSegmentationOutput: IOdOutput
        {
            public PCSegmentationOutput()
            {
                Obstacles = new List<Obstacle>();
            }
            public PCSegmentationOutput(List<Obstacle> obstacles)
            {
                Obstacles = obstacles;
            }
            public List<Obstacle> Obstacles { get; }
        }
    }
}

