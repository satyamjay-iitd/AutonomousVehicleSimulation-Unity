using System;
using System.Collections.Generic;
using Localization;
using Perception.LaneDetection;
using Perception.ObstacleDetection;
using Perception.ProximityDetection;
using Perception.SignalDetection;
using UnityEngine;

namespace Controller
{
    public static class SensorStore
    {
        public static ILdOutput LdOutput                                 { get; set; } = new DeterministicLdOutput();
        public static ProximityDetectionOutput ProximityDetectionOutput  { get; set; }
    
        private static IOdOutput _odOutput;
        public static IOdOutput OdOutput
        {
            get => _odOutput;
            set
            {
                _odOutput = value;
                AssignLaneToObstacles();
            }
        }

        public static YoloSdOutput SdOutput                              { get; set; } = new YoloSdOutput(TrafficLightColor.Green);
        public static LocalizationOutput LocalizationOutput              { get; set; }
        public static float PathPlanningTrajectory                       { get; set; }
        private static float ThresholdDistanceFromFrontObstacle          { get; } = 100.0f;
        private static float ThresholdDistanceFromSideObstacle           { get; } = 120.0f;
        
        public static int ShouldChangeLane()
        {
            if (!ObstacleInFront()) return 0;
            return WhichSideToMove();
        }

        public static bool IsLaneChangeComplete(CarState.CarMotionStateEnum state)
        {
            switch (state)
            {
                case CarState.CarMotionStateEnum.ChangeLaneLeft:
                    return LdOutput.SteeringAngle >= 0;
                case CarState.CarMotionStateEnum.ChangeLaneRight:
                    return LdOutput.SteeringAngle <= 0;
                default:
                    return true;
            }
        }
        public static bool ShouldStopImmediately()
        {
            return WillCollide();
        }
        public static bool FrontCarTooClose()
        {
            return ObstacleInFront();
        }
        public static bool ReachedDestination()
        {
            return LocalizationOutput.HasReachedDestination;
        }
        public static bool IsRedLight()
        {
            return SdOutput.TrafficLightColor == TrafficLightColor.Red;
        }
        public static bool IsAtSpeedLimit()
        {
            return false;
        }
        public static bool IsTurningLeft()
        {
            return false;
        }
        public static bool IsTurningRight()
        {
            return false;
        }
        private static bool ObstacleInFront()
        {
            var (carPosX, carPosY) = (LocalizationOutput.CarPosX, LocalizationOutput.CarPosY);
            List<Obstacle> obstacles;
            
            try
            {
                obstacles = OdOutput.Obstacles;
            }
            catch(NullReferenceException)
            {
                return false;
            }

            foreach (var obstacle in obstacles)
            {
                if (obstacle.Lane == ObstacleRelativeToCarEnum.InFront)
                {
                    var dist = Math.Pow(obstacle.Center.x - carPosX, 2)
                               +Math.Pow(obstacle.Center.y - carPosY, 2);
                    if (dist < ThresholdDistanceFromFrontObstacle)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private static int WhichSideToMove()
        {
            var carPos = (LocalizationOutput.CarPosX, LocalizationOutput.CarPosY);
            Debug.Log(LdOutput.Lane);
            double distanceToNearestObstacleLeft = float.MaxValue;
            foreach (var obstacle in OdOutput.Obstacles)
            {
                if (obstacle.Lane == ObstacleRelativeToCarEnum.InLeft)
                {
                    var dist = Math.Pow(obstacle.Center.x - carPos.CarPosX, 2)
                               + Math.Pow(obstacle.Center.y - carPos.CarPosY, 2);
                    distanceToNearestObstacleLeft = Math.Min(distanceToNearestObstacleLeft, dist);
                }
            }
            if (distanceToNearestObstacleLeft > ThresholdDistanceFromSideObstacle &&
                LdOutput.Lane == LanePosEnum.RightLane) return -1;   // Move to Left Lane
            
            double distanceToNearestObstacleRight = float.MaxValue;
            foreach (var obstacle in OdOutput.Obstacles)
            {
                if (obstacle.Lane == ObstacleRelativeToCarEnum.InRight)
                {
                    var dist = Math.Pow(obstacle.Center.x - carPos.CarPosX, 2)
                               + Math.Pow(obstacle.Center.y - carPos.CarPosY, 2);
                    distanceToNearestObstacleRight = Math.Min(distanceToNearestObstacleRight, dist);
                }
            }
            if (distanceToNearestObstacleRight > ThresholdDistanceFromSideObstacle &&
                LdOutput.Lane == LanePosEnum.LeftLane) return 1;   // Move to right Lane
            return 0;
        }
        private static bool WillCollide()
        {
            return ProximityDetectionOutput.AreObstaclesInProximity;
        }
        
        private static void AssignLaneToObstacles()
        {
            var carPos = new Vector2(LocalizationOutput.CarPosX, LocalizationOutput.CarPosY);

            foreach (var obstacle in _odOutput.Obstacles)
            {
                var obstacleRelativeToCar = obstacle.Center-carPos;
                // Check if the obstacle is behind
                if (obstacleRelativeToCar.y <= 0)
                {
                    obstacle.Lane = ObstacleRelativeToCarEnum.UnKnown;
                }
                // Check if the obstacle is in same lane
                else if (obstacleRelativeToCar.x <=2.9f && obstacleRelativeToCar.x > -2.9f)
                {
                    obstacle.Lane = ObstacleRelativeToCarEnum.InFront;
                }
                // Check if in Right
                else if (obstacleRelativeToCar.x <= 8.7f && obstacleRelativeToCar.x >= 2.9f)
                {
                    obstacle.Lane = ObstacleRelativeToCarEnum.InRight;
                }
                // Check if in Left
                else if (obstacleRelativeToCar.x <= -2.9f && obstacleRelativeToCar.x > -8.7f)
                {
                    obstacle.Lane = ObstacleRelativeToCarEnum.InLeft;
                }
                else if (obstacleRelativeToCar.x > 0)
                {
                    obstacle.Lane = ObstacleRelativeToCarEnum.InFarRight;
                }
                else
                {
                    obstacle.Lane = ObstacleRelativeToCarEnum.InFarLeft;
                }
                Debug.Log(obstacle.Lane);
            }
        }

    }

    public static class CarState
    {
        public enum CarControlStateEnum
        {
            Stop, Accelerate, Decelerate, MaintainSpeed 
        }
        public enum CarMotionStateEnum
        {
            StayInLane, ChangeLaneLeft, ChangeLaneRight
        }
        public static CarControlStateEnum ControlState;
        public static CarMotionStateEnum MotionState;
        static CarState()
        {
            ControlState = CarControlStateEnum.Stop;
            MotionState = CarMotionStateEnum.StayInLane;
        }
        
    }
}