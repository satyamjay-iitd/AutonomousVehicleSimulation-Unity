using System;
using System.Text;
using Controller;
using TMPro;
using UnityEngine;

namespace MainMenu
{
    public class InfoRenderer : MonoBehaviour
    {
        [Header("Sensor Store")]
        public TMP_Text ldOutput;
        public TMP_Text odOutput;
        public TMP_Text localizationOutput;
        public TMP_Text pathPlanningOutput;
        public TMP_Text proximityOutput;

        [Header("Car State")]
        public TMP_Text infoInferred;
        public TMP_Text motionOutput;
        public TMP_Text controlOutput;
        
        private void Update()
        {
            ldOutput.text           = PrepareLdOutput();
            odOutput.text           = PrepareOdOutput();
            localizationOutput.text = PrepareLocalizationOutput();
            pathPlanningOutput.text = PreparePathPlanningOutput();
            proximityOutput.text    = PrepareProximityOutput();

            motionOutput.text       = PrepareMotionOutput();
            controlOutput.text      = PrepareControlOutput();
            infoInferred.text       = PrepareInfoInferredOutput();
        }
        
        private static string PrepareLdOutput()
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("Left Lanes:"+ SensorStore.LdOutput.NumLeftLanes+"\n");
                sb.Append("Right Lanes:"+ SensorStore.LdOutput.NumRightLanes+"\n");
                sb.Append("Steering Angle:"+ SensorStore.LdOutput.SteeringAngle+"\n");
                return sb.ToString();
            }
            catch (NullReferenceException)
            {
                return "NA";
            }
        }
        private static string PrepareOdOutput()
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("Obstacles Detected:"+ SensorStore.OdOutput.Obstacles.Count+"\n");
                return sb.ToString();
            }
            catch (NullReferenceException)
            {
                return "NA";
            }
        }
        private static string PrepareLocalizationOutput()
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("NextNodeL:"+ SensorStore.LocalizationOutput.NextNodeLx+", "+SensorStore.LocalizationOutput.NextNodeLy+"\n");
                sb.Append("NextNodeR:"+ SensorStore.LocalizationOutput.NextNodeRx+", "+SensorStore.LocalizationOutput.NextNodeRy+"\n");
                return sb.ToString();
            }
            catch (NullReferenceException)
            {
                return "NA";
            }
        }
        private static string PreparePathPlanningOutput()
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("Steering Angle "+ SensorStore.PathPlanningTrajectory);
                return sb.ToString();
            }
            catch (NullReferenceException)
            {
                return "NA";
            }
        }
        private static string PrepareProximityOutput()
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("Obstacles in proximity "+ SensorStore.ProximityDetectionOutput.AreObstaclesInProximity+"\n");
                return sb.ToString();
            }
            catch (NullReferenceException)
            {
                return "NA";
            }
        }

        private static string PrepareInfoInferredOutput()
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("Has Reached Destination: "+Brain.ReachedDestination + "\n");
                sb.Append("Is Red Light           : "+Brain.IsRedLight + "\n");
                sb.Append("Is Turning Left        : "+Brain.IsTurningLeft + "\n");
                sb.Append("Is Turning Right       : "+Brain.IsTurningRight + "\n");
                sb.Append("Should Change Lane     : "+Brain.ShouldChangeLane + "\n");
                sb.Append("Should Stop Immediately: "+Brain.ShouldStopImmediately + "\n");
                sb.Append("Front Car Too Close    : "+Brain.FrontCarTooClose + "\n");
                sb.Append("At Speed Limit         : "+Brain.IsAtSpeedLimit + "\n");
                return sb.ToString();
            }
            catch (NullReferenceException)
            {
                return "NA";
            }
        }
        private static string PrepareMotionOutput()
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append(CarState.ControlState);
                return sb.ToString();
            }
            catch (NullReferenceException)
            {
                return "NA";
            }
        }
        private static string PrepareControlOutput()
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append(CarState.MotionState);
                return sb.ToString();
            }
            catch (NullReferenceException)
            {
                return "NA";
            }
        }
        
    }

}
