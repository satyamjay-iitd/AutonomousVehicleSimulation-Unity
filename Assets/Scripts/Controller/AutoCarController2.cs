using UnityEngine;
using UnityEngine.Video;
using UnityStandardAssets.Vehicles.Car;

namespace Controller
{
    [RequireComponent(typeof(CarMover))]
    public class AutoCarController2 : MonoBehaviour, IController
    {

        private CarMover _mCar;
        private void Start()
        {
            _mCar = GetComponent<CarMover>();
        }
        
        // private void Update()
        // {
        //     Brain.UpdateWorldState();
        //     var handbrake = WorldState.FootBrake ? 1.0f : 0f;
        //     var steerAngle = WorldState.SteeringAngle;
        //     _mCar.Move(-(float)steerAngle / 25, 1.0f, 0, handbrake, false);
        // }

        // TODO
        private void Update()
        {
            Brain.UpdateStates();
            var (steeringAngle, acceleration, handbrake) = ExecuteState();
            _mCar.Move(steeringAngle / 25, acceleration, 0, handbrake, false);
        }
        
        private static (float steeringAngle, float acceleration, float handbrake) ExecuteState()
        {
            var controlState = CarState.ControlState;
            var motionState = CarState.MotionState;
            
            var steeringAngle = motionState switch
            { 
                CarState.CarMotionStateEnum.StayInLane => Brain.GetSteeringAngle(),
                CarState.CarMotionStateEnum.ChangeLaneLeft => -15,
                CarState.CarMotionStateEnum.ChangeLaneRight => 15,
                _ => 0
            };
                
            
            switch (controlState)
            {
                case CarState.CarControlStateEnum.Stop:
                    return (0, 0, 1);
                
                case CarState.CarControlStateEnum.Accelerate:
                    switch (motionState)
                    {
                        case CarState.CarMotionStateEnum.StayInLane:
                            return (steeringAngle, 1, 0);
                        case CarState.CarMotionStateEnum.ChangeLaneLeft:
                            return (steeringAngle, 0.5f, 0);
                        case CarState.CarMotionStateEnum.ChangeLaneRight:
                            return (steeringAngle, 0.5f, 0);
                    }
                    break;
                
                case CarState.CarControlStateEnum.Decelerate:
                    return (steeringAngle, 0, 0);

                case CarState.CarControlStateEnum.MaintainSpeed:
                    return (steeringAngle, 0.2f, 0);
                
                default:
                    return (0, 0, 0);
            }
            return (0, 0, 0);
        }
    }
}

