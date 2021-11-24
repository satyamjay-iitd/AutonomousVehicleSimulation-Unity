using UnityEngine;

namespace Sensors
{
    public class UnityObstacleSensor: MonoBehaviour, ISensor<Collider[]>
    {
        public LayerMask layerMask;
        public Vector3 range;

        private Transform _transform;
        private void Start()
        {
            _transform = GetComponent<Transform>();
        }

        public  Collider[] ReadData()
        {
            var colliders = Physics.OverlapBox(_transform.position, range, Quaternion.identity, layerMask);
            return colliders;
        }

        private void OnDrawGizmos()
        {
            foreach (var c in ReadData())
            {
                Gizmos.DrawWireCube(c.bounds.center, c.bounds.extents*2);
            }
        }
    }
}