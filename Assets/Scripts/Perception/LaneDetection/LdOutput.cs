using System.Collections.Generic;
namespace Perception.LaneDetection
{
    public enum LanePosEnum
    {
        LeftLane,
        RightLane,
        DontKnow,
    }
    public interface ILdOutput
    {
        LanePosEnum Lane { get; }
        int NumLeftLanes { get; set; }
        int NumRightLanes { get; set; }
        bool IsLaneDetected { get; set; }
        float Offset { get; set; }

        float LaneWidth { get; }

        float SteeringAngle { get; set; }

        float GetLeftLaneXIntercept();
        
        float GetRightLaneXIntercept();

        IReadOnlyList<float> GetLeftLaneEquation();
        
        IReadOnlyList<float> GetRightLaneEquation();

        // public int CurrLane();
    }
}

