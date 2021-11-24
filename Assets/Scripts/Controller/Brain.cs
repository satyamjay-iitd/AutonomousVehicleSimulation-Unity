using System;

namespace Controller
{
    public static class Brain
    {
        // public static void UpdateWorldState()
        // {
        //     var steeringAngle = WorldState.IsObstacleDetectionAvailable
        //         ? NavigationWithLaneChange()
        //         : NavigationWithoutLaneChange();
        //     WorldState.FootBrake = WorldState.LocalizationOutput.HasReachedDestination;
        //     if (WorldState.IsSignalDetectionAvailable)
        //         WorldState.FootBrake = (WorldState.SdOutput.TrafficLightColor != TrafficLightColor.Green) && WorldState.FootBrake;
        //     WorldState.SteeringAngle = steeringAngle;
        //     WorldState.PrevLdOutput = WorldState.LdOutput;
        //     WorldState.PrevOdOutput = WorldState.OdOutput;
        // }
        //
        // private static float NavigationWithoutLaneChange()
        // {
        //     if(WorldState.IsLaneDetectionAvailable &&WorldState.LdOutput.IsLaneDetected)
        //         return WorldState.LdOutput.SteeringAngle;
        //     else 
        //         return WorldState.PathPlanningTrajectory;
        // }
        //
        // private static float NavigationWithLaneChange()
        // {
        //     var prevOffset = WorldState.PrevLdOutput!=null ? WorldState.PrevLdOutput.Offset: 0;
        //     if (WorldState.LdOutput == null) return 0;
        //     var currOffset = WorldState.LdOutput.Offset;
        //     
        //     var steeringAngle = WorldState.LdOutput.SteeringAngle;
        //     // if move_right_lane flag is true then lane changing to right lane is happening
        //     if (WorldState.IsChangingLane == LaneChangeEnum.ToRight)
        //     {
        //         steeringAngle = -10;
        //         // check for change in sign and discontinuity of offset indicating lane changing has finished
        //         if (prevOffset * currOffset < 0
        //             && prevOffset - currOffset > 6000)
        //         {
        //             WorldState.IsChangingLane = LaneChangeEnum.NoChange;
        //             WorldState.CurrentLane -= 1;
        //             steeringAngle = 12;
        //         }
        //     }
        //     // if move_left_lane flag is true then lane changing to left lane is happening
        //     else if (WorldState.IsChangingLane == LaneChangeEnum.ToLeft)
        //     {
        //         steeringAngle = 10;
        //         // check for change in sign and discontinuity of offset indicating lane changing has finished
        //         if (prevOffset * currOffset < 0 && currOffset - prevOffset > 6000)
        //         {
        //             WorldState.IsChangingLane = LaneChangeEnum.NoChange;
        //             WorldState.CurrentLane += 1;
        //             steeringAngle = -12;
        //         }
        //     }
        //     // lanes are detected do usual task of lane centering
        //     // else if (WorldState.LdOutput.B1.Count == 4 || WorldState.LdOutput.B2.Count == 4)
        //     else if (WorldState.LdOutput.IsLaneDetected)
        //     {
        //         var obstaclesAndLane = MapObstacleToLane();
        //         // Lane change Mode part
        //         WorldState.IsChangingLane = CanChangeLane(obstaclesAndLane);
        //         if (prevOffset * currOffset < 0)
        //         {
        //             // if by accident car goes off lane bring it back
        //             if (prevOffset - currOffset > 6000)
        //             {
        //                 WorldState.IsChangingLane = LaneChangeEnum.ToLeft;
        //                 WorldState.CurrentLane--;
        //             }
        //             else if (currOffset - prevOffset > 6000)
        //             {
        //                 WorldState.IsChangingLane = LaneChangeEnum.ToRight;
        //                 WorldState.CurrentLane++;
        //             }
        //         }
        //     }
        //     // indicates lane are not detected. This means we are at intersection or roundabout
        //     else if (!WorldState.LdOutput.IsLaneDetected)
        //     {
        //         WorldState.CurrentLane = 4 - WorldState.CurrentLane;
        //     }
        //
        //     return steeringAngle;
        // }
        //
        // // Fuse Obstacle and Lane Detection
        // private static Tuple<List<Obstacle>, List<Obstacle>, List<Obstacle>> MapObstacleToLane()
        // {
        //     var numLeftLanes = WorldState.LdOutput.NumLeftLanes; // number of traffic lines to the left of car
        //     var numRightLanes = WorldState.LdOutput.NumRightLanes; // number of traffic lines to the right of car
        //     // bottom most coordinate of left traffic line of current lane
        //     
        //     //var yL = CalculateY(WorldState.LdOutput.B1, WorldState.LdOutput.cLyMax);
        //     // var yR = CalculateY(WorldState.LdOutput.B2, WorldState.LdOutput.cRyMax);
        //     //
        //     // var width = yR - yL; // width of current lane
        //     var width = WorldState.LdOutput.LaneWidth;
        //     var bl3 = new List<float>();
        //     var br3 = new List<float>();
        //     // Parallel lane assumption. finding other traffic lines under parallel lane assumption
        //     for (var j = 0; j < numLeftLanes; j++)
        //     {
        //         bl3.Add(WorldState.LdOutput.GetLeftLaneXIntercept() - j * width);
        //     }
        //
        //     for (var j = 0; j < numRightLanes; j++)
        //     {
        //         br3.Add(WorldState.LdOutput.GetRightLaneXIntercept() - j * width);
        //     }
        //
        //     var obstacleInLeft = new List<Obstacle>();
        //     var obstacleInCurr = new List<Obstacle>();
        //     var obstacleInRight = new List<Obstacle>();
        //
        //     // This assigns lanes to object detected
        //     foreach (var obstacle in WorldState.OdOutput.Obstacles)
        //     {
        //         var lane = obstacle.AssignLane(bl3, br3, WorldState.LdOutput.GetLeftLaneEquation(), WorldState.LdOutput.GetRightLaneEquation());
        //         // Debug.Log(lane);
        //         if (lane == -1000) continue;
        //         if (lane < 0) obstacleInLeft.Add(obstacle);
        //         if (lane == 0) obstacleInCurr.Add(obstacle);
        //         if (lane > 0) obstacleInRight.Add(obstacle);
        //     }
        //     return new Tuple<List<Obstacle>, List<Obstacle>, List<Obstacle>>(obstacleInLeft, obstacleInCurr,
        //         obstacleInRight);
        // }
        //
        // public static float CalculateY(IReadOnlyList<float> cubicLineEquation, float x)
        // {
        //     return cubicLineEquation[0] * x * x * x
        //            + cubicLineEquation[1] * x * x
        //            + cubicLineEquation[2] * x
        //            + cubicLineEquation[3];
        // }
        //
        // private static LaneChangeEnum CanChangeLane(
        //     Tuple<List<Obstacle>, List<Obstacle>, List<Obstacle>> obstacleInLane)
        // {
        //     var carPos = WorldState.CarPos;
        //     var leftLaneObstacle = obstacleInLane.Item1;
        //     var currLaneObstacle = obstacleInLane.Item2;
        //     var rightLaneObstacle = obstacleInLane.Item3;
        //
        //     var isObstacleAhead = false;
        //     foreach (var obstacle in currLaneObstacle) // check for object in front of self-driving car
        //     {
        //         if (obstacle.YMax < carPos[1].x
        //             && 300 > Math.Abs(obstacle.YMax - carPos[1].x)
        //             && 250 < Math.Abs(obstacle.YMax - carPos[1].x)
        //         )
        //         {
        //             isObstacleAhead = true;
        //             break;
        //         }
        //     }
        //
        //     if (!isObstacleAhead) return LaneChangeEnum.NoChange;
        //
        //     if (WorldState.CurrentLane != 4)
        //     {
        //         // if right lane detected and no objects in right lane
        //         if (rightLaneObstacle.Count == 0 && WorldState.LdOutput.NumRightLanes > 1)
        //         {
        //             return LaneChangeEnum.ToRight;
        //         }
        //
        //         foreach (var obstacle in rightLaneObstacle) // check for space in right lane
        //         {
        //             if (obstacle.Center.Y < WorldState.CarPos[1].x
        //                 && Math.Abs(obstacle.Center.Y - WorldState.CarPos[1].x) > 600)
        //             {
        //                 return LaneChangeEnum.ToRight;
        //             }
        //         }
        //     }
        //
        //     if (WorldState.CurrentLane != 3)
        //     {
        //         // if left lane detected and no objects in left lane
        //         if (leftLaneObstacle.Count == 0 && WorldState.LdOutput.NumLeftLanes > 1)
        //         {
        //             return LaneChangeEnum.ToLeft;
        //         }
        //
        //         foreach (var obstacle in leftLaneObstacle) // check for space in right lane
        //         {
        //             if (obstacle.Center.Y < WorldState.CarPos[1].x
        //                 && Math.Abs(obstacle.Center.Y - WorldState.CarPos[1].x) > 600)
        //             {
        //                 return LaneChangeEnum.ToLeft;
        //             }
        //         }
        //     }
        //     return LaneChangeEnum.NoChange;
        // }
        //
        private static bool LaneChangeComplete   { get; set; } = true;
        public static int ShouldChangeLane       {get; private set;}
        public static bool ShouldStopImmediately {get; private set;}
        public static bool FrontCarTooClose      {get; private set;}
        public static bool ReachedDestination    {get; private set;}
        public static bool IsRedLight            {get; private set;}
        public static bool IsAtSpeedLimit        {get; private set;}
        public static bool IsTurningLeft         {get; private set;}
        public static bool IsTurningRight        {get; private set;}

        public static float GetSteeringAngle()
        {
            try
            {
                return (-SensorStore.LdOutput.SteeringAngle + SensorStore.PathPlanningTrajectory) / 2;
            }
            catch (NullReferenceException ex)
            {
                return SensorStore.PathPlanningTrajectory;
            }
        } 
        public static void UpdateStates()
        {
            ShouldChangeLane = SensorStore.ShouldChangeLane();
            ShouldStopImmediately = SensorStore.ShouldStopImmediately();
            FrontCarTooClose = SensorStore.FrontCarTooClose();
            ReachedDestination = SensorStore.ReachedDestination();
            IsRedLight = SensorStore.IsRedLight();
            IsAtSpeedLimit = SensorStore.IsAtSpeedLimit();
            IsTurningLeft = SensorStore.IsTurningLeft();
            IsTurningRight = SensorStore.IsTurningRight();
        
            UpdateCarControlState();
            UpdateCarMotionState();
        }
        private static void UpdateCarControlState()
        {
            var currentState = CarState.ControlState;
            var newState = currentState;
            switch (currentState)
            {
                case CarState.CarControlStateEnum.Stop:
                    if (!ShouldStopImmediately && !ReachedDestination && !IsRedLight)
                        newState = CarState.CarControlStateEnum.Accelerate;
                    break;
                
                case CarState.CarControlStateEnum.Accelerate:
                    if (ShouldStopImmediately || ReachedDestination || IsRedLight)
                        newState = CarState.CarControlStateEnum.Stop;
                    else if (IsAtSpeedLimit || (ShouldChangeLane==0 && FrontCarTooClose))
                        newState = CarState.CarControlStateEnum.MaintainSpeed;
                    break;
                
                case CarState.CarControlStateEnum.Decelerate:
                    if (ShouldStopImmediately || ReachedDestination || IsRedLight)
                        newState = CarState.CarControlStateEnum.Stop;
                    else if (!(IsAtSpeedLimit || FrontCarTooClose))
                        newState = CarState.CarControlStateEnum.Accelerate;
                    break;
                
                case CarState.CarControlStateEnum.MaintainSpeed:
                    if (FrontCarTooClose)
                        newState = CarState.CarControlStateEnum.Decelerate;
                    else if (!(IsAtSpeedLimit || IsRedLight || ReachedDestination))
                        newState = CarState.CarControlStateEnum.Accelerate;
                    break;
            }
            CarState.ControlState = newState;
        }
        private static void UpdateCarMotionState()
        {
            var currentState = CarState.MotionState;
            var newState = currentState;
            switch (currentState)
            {
                case CarState.CarMotionStateEnum.StayInLane:
                    if (ShouldChangeLane != 0)
                    {
                        newState = ShouldChangeLane == -1
                            ? CarState.CarMotionStateEnum.ChangeLaneLeft
                            : CarState.CarMotionStateEnum.ChangeLaneRight;
                    }

                    break;

                case CarState.CarMotionStateEnum.ChangeLaneLeft:
                    if (!LaneChangeComplete)
                    {
                        newState = CarState.CarMotionStateEnum.ChangeLaneLeft;
                        LaneChangeComplete = SensorStore.IsLaneChangeComplete(CarState.CarMotionStateEnum.ChangeLaneLeft);
                    }
                    else if (ShouldChangeLane == 0)
                    {
                        newState = CarState.CarMotionStateEnum.StayInLane;
                        LaneChangeComplete = false;
                    }

                    break;
                
                case CarState.CarMotionStateEnum.ChangeLaneRight:
                    if (ShouldChangeLane == 0)
                        newState = CarState.CarMotionStateEnum.StayInLane;
                    break;
                
            }
            CarState.MotionState = newState;
        }

    }
}