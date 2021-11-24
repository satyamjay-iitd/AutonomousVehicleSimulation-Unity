using System;
using UnityEngine;

namespace Sensors
{
    public class Gps : MonoBehaviour, ISensor<(Vector2, Vector2)>
    {
        [Range(0.0f, 1.0f)]
        public float standardDeviationX = 0.01f;
        [Range(0.0f, 1.0f)]
        public float standardDeviationZ = 0.01f;

        private System.Random _random;
        private Vector2 _dir, _pos;
        private Transform _carTransform;
        private void Start()
        {
            _random = new System.Random();
            _carTransform = GetComponent<Transform>();
            
            var pos = _carTransform.position;
            var x = pos.x + SampleGaussian(_random, 0, standardDeviationX);
            var z = pos.z + SampleGaussian(_random, 0, standardDeviationZ);
            _pos = new Vector2((float)x, (float)z);
            
            var dir = _carTransform.forward; 
            dir = Quaternion.Euler(0, _carTransform.rotation.y, 0) * dir;
            var dirX = dir.x + SampleGaussian(_random, 0, standardDeviationX);
            var dirY = dir.z + SampleGaussian(_random, 0, standardDeviationZ);
            _dir = new Vector2((float)dirX, (float)dirY);
        }
        
        private void Update()
        {
            var pos = _carTransform.position;
            var x = pos.x + SampleGaussian(_random, 0, standardDeviationX);
            var z = pos.z + SampleGaussian(_random, 0, standardDeviationZ);
            _pos = new Vector2((float)x, (float)z);
            
            var dir = _carTransform.forward;
            dir = Quaternion.Euler(0, _carTransform.rotation.y, 0) * dir;

            var dirX = dir.x + SampleGaussian(_random, 0, standardDeviationX);
            var dirY = dir.z + SampleGaussian(_random, 0, standardDeviationZ);
            _dir = new Vector2((float)dirX, (float)dirY);
        }

        public (Vector2, Vector2) ReadData()
        {
            return (_pos, _dir);
        }

        private static float SampleGaussian(System.Random random, double mean, double stdDeviation)
        {
            // The method requires sampling from a uniform random of (0,1]
            // but Random.NextDouble() returns a sample of [0,1).
            var x1 = 1 - random.NextDouble();
            var x2 = 1 - random.NextDouble();
            var y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
            return (float) (y1 * stdDeviation + mean);
        }
    }
}

