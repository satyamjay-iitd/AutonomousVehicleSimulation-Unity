using System.Collections.Generic;

namespace Perception.LaneDetection
{
    public class PinetLdOutput: ILdOutput
    {
        public float SteeringAngle       { get; set; }
        private int TotalPts             { get; set; }
        public int NumLeftLanes          { get; set; }
        public int NumRightLanes         { get; set; }
        private float cLyMax             { get; set; }
        private float cRyMax             { get; set; }
        private IReadOnlyList<float> B1  { get; }
        private IReadOnlyList<float> B2  { get; }  
        public float Offset              { get; set; }
        public bool IsLaneDetected       { get; set; }
        public float LaneWidth           { get; }

        public LanePosEnum Lane
        {
            get
            {
                if (NumLeftLanes == 3 && NumRightLanes == 0) return LanePosEnum.RightLane;
                if (NumLeftLanes == 2 && NumRightLanes == 1) return LanePosEnum.LeftLane;
                return LanePosEnum.DontKnow;
            }
        }

        public PinetLdOutput(float steeringAngle = default, int totalPts = default, int numLeftLanes = default, int numRightLanes = default, float cLyMax = default, float cRyMax = default, IReadOnlyList<float> b1 = null, IReadOnlyList<float> b2 = null, int offset = default)
        {
            SteeringAngle = steeringAngle;
            TotalPts = totalPts;
            NumLeftLanes = numLeftLanes;
            NumRightLanes = numRightLanes;
            this.cLyMax = cLyMax;
            this.cRyMax = cRyMax;
            B1 = b1;
            B2 = b2;
            Offset = offset;
        }

        public float GetLeftLaneXIntercept() {
            return B1[3];
        }

        public float GetRightLaneXIntercept() {
            return B2[3];
        }

        public IReadOnlyList<float> GetLeftLaneEquation() {
            return B1;
        }

        public IReadOnlyList<float> GetRightLaneEquation() {
            return B2;
        }
    }
}