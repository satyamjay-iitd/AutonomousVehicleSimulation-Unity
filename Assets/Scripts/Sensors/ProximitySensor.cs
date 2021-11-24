using Unity.Collections;
using UnityEngine;

namespace Sensors
{
    public class ProximitySensor : MonoBehaviour, ISensor<bool>
    {
        private Transform _carTransform;

        [Header("Sensors")]
        [SerializeField] private Transform frontMid;
        [SerializeField] private Transform frontLeft;
        [SerializeField] private Transform frontRight;
        [SerializeField] private float sensorRange;
        [SerializeField] private LayerMask layerMask;
        void Start()
        {
            _carTransform = GetComponent<Transform>();
        }

        public bool ReadData()
        {
            return Sensor();
        }

        // v is fractional motor torque, h is fractional steering angle
        private bool Sensor()
        {
            var forward = _carTransform.forward;
            
            var results = new NativeArray<RaycastHit>(3, Allocator.TempJob);

            var commands = new NativeArray<RaycastCommand>(3, Allocator.TempJob);
            commands[0] = new RaycastCommand(frontMid.position, forward, sensorRange, layerMask);
            commands[1] = new RaycastCommand(frontLeft.position, forward, sensorRange, layerMask);
            commands[2] = new RaycastCommand(frontRight.position, forward, sensorRange, layerMask);
            
            var handle = RaycastCommand.ScheduleBatch(commands, results, 1);
            handle.Complete();
            
            var frontMidHit = results[0];
            var frontLeftHit = results[1];
            var frontRightHit = results[2];

            commands.Dispose();
            results.Dispose();
            return !(frontMidHit.collider is null) || !(frontLeftHit.collider is null) || !(frontRightHit.collider is null);
        }
    }
}

