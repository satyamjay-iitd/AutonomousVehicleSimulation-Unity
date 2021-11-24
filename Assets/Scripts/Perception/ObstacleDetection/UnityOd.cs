using System.Collections.Generic;
using Controller;
using Sensors;
using UnityEngine;

namespace Perception.ObstacleDetection
{
    [RequireComponent (typeof(UnityObstacleSensor))]
    public class UnityOd: ObstacleDetection
    {
        private UnityObstacleSensor _sensor;

        private void Start()
        {
            _sensor = GetComponent<UnityObstacleSensor>();
            _sensor.range = new Vector3(20, 2, 25);
            _sensor.layerMask = 1 << 3;
        }

        private void Update()
        {
            var colliders = _sensor.ReadData();
            var obstacles = new List<Obstacle>();
            foreach (var c in colliders)
            {
                obstacles.Add(new Obstacle(c));
            }
            SensorStore.OdOutput = new UnityOdOutput(obstacles);
        }
    }
    
    public class UnityOdOutput: IOdOutput
    {
        public UnityOdOutput()
        {
            Obstacles = new List<Obstacle>();
        }
        public UnityOdOutput(List<Obstacle> obstacles)
        {
            Obstacles = obstacles;
        }
        public List<Obstacle> Obstacles { get; }
    }
    
}