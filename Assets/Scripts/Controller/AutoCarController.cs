using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;


// Car Default Position X = 1507, Y = 0, Z = 375, Rotation Y = 180
namespace Controller
{
    [RequireComponent(typeof(CarMover))]
    public class AutoCarController : MonoBehaviour, IController{
        private CarMover _mCar;                                          // The car controller we want to use
        private Rigidbody _rigidCar;                                     // Rigid body of object of the car
        public float speed;                                              // Speed of car in kmph is stored here for output
        public bool startMoving;                                         // Boolean that decides whether car should move forward or not
        public double steerAngle;                                        // Steering angle of the car
        
        private bool _takeTurn;                                          // Map sets this true if there is a turn ahead 
        
        public float accVal=0.5f;                                        // The value to accelerate forward: range is from 0 to 1
        [Header("Sensors")]
        public int nodeIndex;                                            // Points to the current gps node
        public Vector3 frontSensorPos = new Vector3(0f, 0f, 3f);    // Straight sensor position in front of car
        public Vector3 sideSensorPos = new Vector3(1f, 0f, 3f);     // Sensor position for side straight and side-angled sensors
        public Vector3 sideWallSensorPos;                                // Sensor position for sensors on walls
        public float longSensorLength = 200f;                            // Utility variable for calculating safe braking distance
        public float shortSensorLength = 20f;                            // Utility variable for calculating safe braking distance
        public float sideLaneThresh = 5f;                                // Safe distance that the side sensors need to maintain while changing lane
        
        private readonly float[] _prevLidar = new float[5];              // Stores the lidar readings of previous frame for all 5 sensors at the front 
        
        public TrafficSystemNode sourceNode;                             // Next node that car needs to go.
        public TrafficSystemNode destinationNode;                        // Final destination node  
        public float topSpeed = 20f;                                     // Maximum speed of the car
        public float sideSensorAngle = 25f;                              // Angle at which side sensor is inclined
        
        private readonly List<TrafficSystemNode> _mNodeToPathToRoute =
            new List<TrafficSystemNode>();                               // This is the nodes that it will traverse to get to the "m_nodeToPathTo" destination

        private int _turnStart, _turnEnd;                                // Denotes the start and end nodes for lane change
        
        private bool _mDoneCalculatingPath, _mCalculatingPathToTake;     // Utility variable for path planning
        public bool isTurning;                                           // True when car is taking turn
        
        private void PathPlanning()
        {
            _mNodeToPathToRoute.Clear();

            if (sourceNode == destinationNode) return;

            // Implements A star algorithm
            // f, g, h function are standard notations for A star 
            List<Tuple<TrafficSystemNode, float, float>> candidateList = new List<Tuple<TrafficSystemNode, float, float>>();
            Dictionary<int, TrafficSystemNode> idToNodeMap = new Dictionary<int, TrafficSystemNode>();
            Dictionary<int, int> bestParent = new Dictionary<int, int>();
            Dictionary<int, float> distanceTillNode = new Dictionary<int, float>();
            float fBestYet = float.MaxValue;

            candidateList.Add(new Tuple<TrafficSystemNode, float, float>(sourceNode, 0.0f, 0.0f));
            int startNodeId = sourceNode.GetInstanceID();
            idToNodeMap.Add(startNodeId, sourceNode);
            distanceTillNode.Add(startNodeId, 0.0f);

            // BFS like loop
            while (candidateList.Count > 0)
            {
                Tuple<TrafficSystemNode, float, float> candidate = PopMin(ref candidateList);
                int candidateId = candidate.Item1.GetInstanceID();

                if (!idToNodeMap.ContainsKey(candidateId))
                {
                    idToNodeMap.Add(candidateId, candidate.Item1);
                }

                List<TrafficSystemNode> children = candidate.Item1.GetChildren();

                foreach (TrafficSystemNode child in children)
                {
                    int childId = child.GetInstanceID();
                    float g = candidate.Item3 + displacementMagnitude(child, candidate.Item1);
                    // f = g + h
                    float f = g + displacementMagnitude(child, destinationNode);

                    if (distanceTillNode.ContainsKey(childId))
                    {
                        if (distanceTillNode[childId] > f)
                        {
                            distanceTillNode[childId] = f;
                            bestParent[childId] = candidateId;
                            candidateList.Add(new Tuple<TrafficSystemNode, float, float>(child, f, g));
                        }
                    }
                    else
                    {
                        distanceTillNode.Add(childId, f);
                        bestParent.Add(childId, candidateId);
                        candidateList.Add(new Tuple<TrafficSystemNode, float, float>(child, f, g));
                    }

                    // Reached destination, but is it the best path?
                    if (child == destinationNode)
                    {
                        if (f < fBestYet)
                        {
                            fBestYet = f;
                        }
                    }
                }
                // Condition to ensure best path is selected, break only when g is more than best f achieved till yet
                if (fBestYet < candidate.Item2) break;
            }
            // Backtrack to get path
            int finishNodeId = destinationNode.GetInstanceID();
            int currentNodeId = finishNodeId;

            while (true)
            {
                _mNodeToPathToRoute.Add(idToNodeMap[currentNodeId]);
                if (currentNodeId == startNodeId) break;
                currentNodeId = bestParent[currentNodeId];
            }
            // backtracking gives the reverse path
            _mNodeToPathToRoute.Reverse();
            _mDoneCalculatingPath = true;
            _mCalculatingPathToTake = false;
        }

        private static Tuple<TrafficSystemNode, float, float> PopMin(
            ref List<Tuple<TrafficSystemNode, float, float>> candidateList)
        {
            float minVal = float.MaxValue;
            int index = -1;
            for (int i = 0; i < candidateList.Count; i++)
            {
                if (candidateList[i].Item2 < minVal)
                {
                    minVal = candidateList[i].Item2;
                    index = i;
                }
            }

            Tuple<TrafficSystemNode, float, float> ret = candidateList[index];
            candidateList.RemoveAt(index);
            return ret;
        }

        private void CalculatePathToTake(TrafficSystemNode aNodeToPathTo)
        {
            if (!sourceNode)
                return; // we are not connected to the road network

            if (_mCalculatingPathToTake) // we are already calculating a path to find
                return;

            destinationNode = aNodeToPathTo;

            if (destinationNode && !_mDoneCalculatingPath)
            {
                _mCalculatingPathToTake = true;
                PathPlanning();
            }
        }
        
        private float displacementMagnitude(TrafficSystemNode a, TrafficSystemNode b)
        {
            Vector3 displacement = a.transform.position - b.transform.position;
            return displacement.magnitude;
        }
        
        // Called once at the start of script
        // private void Start()
        // {
        //     // Initialize the array
        //     for(int i=0; i<_prevLidar.Length; i++) {
        //         _prevLidar[i] = 0; 
        //     }
        //     WorldState.CurrentLane = sourceNode.m_lane;
        // }
        private void Awake()
        {
            _mCar = GetComponent<CarMover>();
            _rigidCar = GetComponent<Rigidbody>();
        }
        
        // Called once per each update frame
        // private void FixedUpdate()
        // {
        //     // Wait until path planning is completed
        //     if (!_mDoneCalculatingPath && !_mCalculatingPathToTake)
        //     {
        //         CalculatePathToTake(destinationNode);
        //         return;
        //     }
        //     if (!startMoving)
        //     {
        //         return;
        //     }
        //     // handbrake can be applied using space bar
        //     var handbrake = CrossPlatformInputManager.GetAxis("Jump");
        //
        //     Brain.UpdateWorldState();
        //     steerAngle = (int) (WorldState.SteeringAngle) != -100 ? WorldState.SteeringAngle : 0;
        //
        //     // If map says to take turn and there are no lane lines
        //     /*  if(!isTurning && _takeTurn || (WorldState.LdOutput.TotalPts < 40 ||
        //                                    WorldState.LdOutput.NumLeftLanes==0 ||
        //                                    WorldState.LdOutput.NumRightLanes==0))*/
        //     if(!isTurning && _takeTurn || WorldState.LdOutput.IsLaneDetected)
        //     {
        //         isTurning = true;
        //     }
        //     
        //     // This section is run when car is navigating on intersection, roundabouts.
        //     if (isTurning)
        //     {
        //         if (nodeIndex > _turnEnd)
        //         {
        //             isTurning = false;
        //             _takeTurn = false;
        //         }
        //         // In real scenarios these information will come from the inertial sensors. here we simply use coordinate system in unity
        //         else
        //         {
        //             Vector3 dir = _mNodeToPathToRoute[nodeIndex].transform.position - transform.position; //estimates the vector from the car to the next node in the path 
        //             steerAngle = -Vector3.SignedAngle(transform.forward, dir, transform.up);   //calculates steering angle
        //         }
        //     }
        //     
        //     // This section simulates GPS   
        //     if (nodeIndex < _mNodeToPathToRoute.Count)
        //     {
        //         accVal=1f;
        //         Vector3 dir = _mNodeToPathToRoute[nodeIndex].transform.position - transform.position;
        //         if (!isTurning && dir.magnitude < 7)
        //         {
        //             nodeIndex++;
        //             if (nodeIndex < _mNodeToPathToRoute.Count)
        //             {
        //                 if (_mNodeToPathToRoute[nodeIndex].isTurn)
        //                 {
        //                     _takeTurn = true;
        //                     _turnStart = nodeIndex;
        //                     _turnEnd = _turnStart;
        //                     while (_mNodeToPathToRoute[_turnEnd].isTurn)
        //                     {
        //                         _turnEnd++;
        //                         if (_turnEnd >= _mNodeToPathToRoute.Count)
        //                         {
        //                             break;
        //                         }
        //                     }
        //                     WorldState.CurrentLane = _mNodeToPathToRoute[_turnEnd - 1].m_lane;
        //                 }
        //                 else if (_mNodeToPathToRoute[nodeIndex - 1].m_lane != _mNodeToPathToRoute[nodeIndex].m_lane)
        //                 {
        //                     isTurning = true;
        //                     _takeTurn = true;
        //                     _turnEnd = nodeIndex + 2;
        //                 }
        //             }
        //         }
        //         else if (dir.magnitude < 3)
        //         {
        //             nodeIndex++;
        //         }
        //     }
        //     else{
        //           accVal=0; // Set acceleration to zero if reached at destination
        //     }
        //     if (steerAngle >= 10 || steerAngle <= -10) // if car is at turn or the steering angle is greater than the threshold steering angle then limit the velocity at turn
        //     {
        //         _mCar.topSpeed = 10; // v_max_turn=10
        //     }
        //     else
        //     {
        //         _mCar.topSpeed = topSpeed; // Normal speed limits of the car
        //     }
        //     // Sensor(-(float)steerAngle / 25, _accVal, handbrake); // Steering angle sent to the sensors which control the velocity, 25 in first argument is actually maximum steer angle
        //     handbrake = WorldState.FootBrake ? 1.0f : 0f;
        //     _mCar.Move(-(float)steerAngle / 25, accVal, 0, handbrake, false);
        // }
    }
}
