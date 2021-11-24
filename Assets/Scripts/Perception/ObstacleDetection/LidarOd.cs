using System;
using System.Collections.Generic;
using System.Diagnostics;
using Controller;
using IPC;
using Newtonsoft.Json;
using Sensors;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Newtonsoft.Json.Linq;

namespace Perception.ObstacleDetection
{ 
    [RequireComponent(typeof(LidarSensor))]
    public class LidarOd : ObstacleDetection
    {
        private Process _process;
        private LidarSensor _lidarSensor;
        private void Start()
        {
            var context = new ProcessStartInfo
            {
                FileName = "/home/janib/anaconda3/envs/spconv/bin/python",
                Arguments = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server/ObstacleDetectionProcess.py",
                WorkingDirectory = "/home/janib/Downloads/Editor/AutonomousDriving-Refactored/Inference_Server",
                UseShellExecute = true,
            };
            _process = Process.Start(context);
            _lidarSensor = gameObject.GetComponent<LidarSensor>();
            Debug.Log("Waiting for the Lidar process to start");
            while (Ipc.IsLidarOutputReady() == false) {}
            // WorldState.IsObstacleDetectionAvailable = true;
        }

        private void FixedUpdate()
        {
            if (Ipc.IsLidarOutputReady())
            {
                SensorStore.OdOutput = new LidarOdOutput(JsonConvert.DeserializeObject<List<Obstacle>>(Ipc.ReadLidarOutput(), new ObstacleConverter()));
                var sensorData = _lidarSensor.ReadData();
                if (sensorData == null) return;
                var s = Convert.ToBase64String(sensorData);
                Ipc.WriteLidarImg(s);
                Ipc.UnsetLidarOutputReady();
            }
        }
        
        private void OnApplicationQuit()
        {
            _process.Kill();
        }
    }
    
    public class LidarOdOutput: IOdOutput
    {
        public LidarOdOutput(List<Obstacle> obstacles)
        {
            Obstacles = obstacles;
        }
        public List<Obstacle> Obstacles { get; }
    }

    public class ObstacleConverter : JsonConverter<Obstacle>
    {
        public override bool CanRead  => true;
        public override bool CanWrite => false;
        
        public override Obstacle ReadJson(JsonReader reader, Type objectType, Obstacle existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var obstacle = new Obstacle();
            var jsonObject = JObject.Load(reader);
            var centerCord = jsonObject.GetValue("Center") as JArray;
            obstacle.Center = new Vector2((float)centerCord[0], (float)centerCord[1]);
            var bbox = jsonObject.GetValue("Bbox") as JArray;
            var bbox0 = bbox[0] as JArray;
            var bbox1 = bbox[1] as JArray;
            var bbox2 = bbox[2] as JArray;
            var bbox3 = bbox[3] as JArray;
            obstacle.Bbox = new Tuple<Vector2, Vector2, Vector2, Vector2>(
                new Vector2((float)bbox0[0], (float)bbox0[1]),
                new Vector2((float)bbox1[0], (float)bbox1[1]),
                new Vector2((float)bbox2[0], (float)bbox2[1]),
                new Vector2((float)bbox3[0], (float)bbox3[1]));
            var yMax = (float)jsonObject.GetValue("YMax");
            obstacle.YMax = yMax;
            return obstacle;
        }
        
        public override void WriteJson(JsonWriter writer, Obstacle value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

