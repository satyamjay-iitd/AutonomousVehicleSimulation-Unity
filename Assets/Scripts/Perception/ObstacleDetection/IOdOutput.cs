using System;
using System.Collections.Generic;
using UnityEngine;

namespace Perception.ObstacleDetection
{
    
    public interface IOdOutput
    {
        public List<Obstacle> Obstacles { get; }
    }
    
    public class Obstacle
    {
        public Obstacle()
        {
            
        }
        public Obstacle(Collider collider)
        {
            var bound = collider.bounds;
            Center = new Vector2(bound.center.x, bound.center.z);
            Bbox = new Tuple<Vector2, Vector2, Vector2, Vector2>(
                Center + new Vector2(-bound.extents.x, bound.extents.z),
                Center + new Vector2(bound.extents.x, bound.extents.z),
                Center + new Vector2(-bound.extents.x, -bound.extents.z),
                Center + new Vector2(bound.extents.x, bound.extents.z));
            Lane = ObstacleRelativeToCarEnum.UnKnown;
        }
        public Vector2                                   Center { get; set; }
        public Tuple<Vector2, Vector2, Vector2, Vector2> Bbox   { get; set; }
        public float                                     YMax   { get; set; }
        public ObstacleRelativeToCarEnum                 Lane   { get; set; }
        // public int AssignLane(List<float> leftLanes, List<float> rightLanes, IReadOnlyList<float> b1, IReadOnlyList<float> b2)
        // {
        //     var fLx = Brain.CalculateY(b1, Center.Y);
        //     var fRx = Brain.CalculateY(b2, Center.Y);
        //
        //     if (leftLanes.Count > 0
        //         && fLx + leftLanes[0] < Center.X
        //         && rightLanes.Count > 0
        //         && fRx + rightLanes[0] < Center.X)
        //     {
        //         Lane = ObstacleRelativeToCarEnum.InFront;
        //         return 0;
        //     }
        //     if (leftLanes.Count > 1 && fLx + leftLanes[1] < Center.X && fLx + leftLanes[0] > Center.X)
        //     {
        //         Lane = ObstacleRelativeToCarEnum.InLeft;
        //         return -1;
        //     }
        //     for (var i = 2; i < leftLanes.Count; i++)   // check for object in other left lane
        //     {
        //         if(fLx + leftLanes[i] < Center.X && Center.X < fLx + leftLanes[i - 1]){
        //             Lane = ObstacleRelativeToCarEnum.InFarLeft;
        //             return -i;
        //         }
        //     }
        //     if (rightLanes.Count > 1 && fRx + rightLanes[1] > Center.X && fRx + rightLanes[0] > Center.X)
        //     {
        //         Lane = ObstacleRelativeToCarEnum.InRight;
        //         return 1;
        //     }
        //     for (var i = 2; i < rightLanes.Count; i++)   // check for object in other right lane
        //     {
        //         if(fRx + rightLanes[i] > Center.X && Center.X > fRx + rightLanes[i - 1]){
        //             Lane = ObstacleRelativeToCarEnum.InFarRight;
        //             return i;
        //         }
        //     }
        //     // Represents "Cannot assign lane"
        //     Lane = ObstacleRelativeToCarEnum.UnKnown;
        //     return -1000;
        // }
    }
    
    public enum ObstacleRelativeToCarEnum
    {
        InFront,
        InLeft,
        InRight,
        InFarLeft,
        InFarRight,
        UnKnown
    }

}