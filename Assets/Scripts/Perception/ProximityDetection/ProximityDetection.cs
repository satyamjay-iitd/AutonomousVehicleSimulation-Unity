using Controller;
using Sensors;
using UnityEngine;

namespace Perception.ProximityDetection
{
    [RequireComponent(typeof(ProximitySensor))]
    public class ProximityDetection : MonoBehaviour
    {
        private ProximitySensor _proximitySensor;

        private void Start()
        {
            _proximitySensor = GetComponent<ProximitySensor>();
            SensorStore.ProximityDetectionOutput = new ProximityDetectionOutput(false);
        }
        private void Update()
        {
           SensorStore.ProximityDetectionOutput = new ProximityDetectionOutput(_proximitySensor.ReadData());
        }
    }
    
    public class ProximityDetectionOutput
    {
        public readonly bool AreObstaclesInProximity;
        public ProximityDetectionOutput(bool areObstaclesInProximity)
        {
            AreObstaclesInProximity = areObstaclesInProximity;
        }
    }

}