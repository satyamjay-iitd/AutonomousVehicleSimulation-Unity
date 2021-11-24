using System.Collections.Generic;
using UnityEngine;

namespace Perception.LaneDetection
{
    public class DeterministicLdOutput: ILdOutput
    {
        
        private IReadOnlyList<float> B1  { get; }
        private IReadOnlyList<float> B2  { get; }
        public float Offset              { get; set; }
        public bool IsLaneDetected       { get; set; }
        public int NumLeftLanes          { get; set; }
        public int NumRightLanes         { get; set; }
        public float SteeringAngle       { get; set; }
        
        public float LaneWidth => GetRightLaneXIntercept() - GetLeftLaneXIntercept();

        private float _prevLane = 0;
        
        public LanePosEnum Lane
        {
            get
            {
                // Debug.Log(_prevLane);
                float currLane;
                if (NumLeftLanes == 3 && NumRightLanes == 0)
                {
                    currLane = 0.9f * _prevLane + 0.9f;
                }
                else if (NumLeftLanes == 2 && NumRightLanes == 1)
                {
                    currLane = 0.9f * _prevLane - 0.9f;
                }
                else
                {
                    currLane = 0.9f * _prevLane;
                }

                _prevLane = currLane;
                if (currLane >= 1) return LanePosEnum.RightLane;
                if (currLane <= -1) return LanePosEnum.LeftLane;
                return LanePosEnum.DontKnow;
            }
        }

        public DeterministicLdOutput(IReadOnlyList<float> b1 = null, IReadOnlyList<float> b2 = null, float offset = default, bool isDetLaneDetected = default, 
            int numLeftLane = default, int numRightLane = default, float steeringAngle = default)
        {
            B1 = b1;
            B2 = b2;
            Offset = offset;
            IsLaneDetected = isDetLaneDetected;
            SteeringAngle = steeringAngle;
            NumLeftLanes = numLeftLane;
            NumRightLanes = numRightLane;
        }
        public float GetLeftLaneXIntercept() {
            return Mathf.Pow(B1[0] * 255,  2) + B1[1] * 255 + B1[2];   //Bottom intercept
        }
        public float GetRightLaneXIntercept() {
            return Mathf.Pow(B2[0] * 255,  2) + B2[1] * 255 + B2[2];   //Bottom intercept
        }
        public IReadOnlyList<float> GetLeftLaneEquation() {
            return B1;
        }
        public IReadOnlyList<float> GetRightLaneEquation() {
            return B2;
        }
    }
}