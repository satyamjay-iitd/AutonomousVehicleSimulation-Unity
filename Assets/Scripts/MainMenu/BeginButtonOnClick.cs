using System;
using Controller;
using Perception.LaneDetection;
using Perception.ObstacleDetection;
using Perception.ProximityDetection;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

namespace MainMenu{
    public class BeginButtonOnClick : MonoBehaviour
    {
        public GameObject car;
        public TMP_Dropdown controllerDropdown;
        public TMP_Dropdown laneDetectionDropdown;
        public TMP_Dropdown obstacleDetectionDropdown;
        public TMP_Dropdown signalDetectionDropdown;
        public TMP_Dropdown mapperDropdown;
        public TMP_Dropdown pathPlannerDropdown;
        public TMP_Dropdown localizerDropdown;
        public Toggle proximitySensorToggle; 
        
        [SerializeField] private Canvas infoOverlay;
        public void OnClick()
        {
            var car = GameObject.Find("Car");
            // Select Controller
            var controllerType = GetSelectedType(controllerDropdown);
            if (controllerType == typeof(UserCarController)) {
                car.GetComponent<UserCarController>().enabled = true;
                car.GetComponent<AutoCarController>().enabled = false;
                car.GetComponent<AutoCarController2>().enabled = false;
            }
            else if (controllerType == typeof(AutoCarController)) {
                car.GetComponent<UserCarController>().enabled = false;
                car.GetComponent<AutoCarController2>().enabled = false;
                car.GetComponent<AutoCarController>().enabled = true;
                car.GetComponent<AutoCarController>().startMoving = true;
            }
            else if (controllerType == typeof(AutoCarController2)){
                car.GetComponent<UserCarController>().enabled = false;
                car.GetComponent<AutoCarController>().enabled = false;
                car.GetComponent<AutoCarController2>().enabled = true;
            }
            // Select Lane Detection
            var lDType = GetSelectedType(laneDetectionDropdown);
            if(lDType != null) car.AddComponent(lDType);
            // Select Obstacle Detection
            var oDType = GetSelectedType(obstacleDetectionDropdown);
            if(oDType != null) car.AddComponent(oDType);
            // Select Signal Detection
            var sDType = GetSelectedType(signalDetectionDropdown);
            if(sDType != null) car.AddComponent(sDType);
            // Select Mapper
            var mapperType = GetSelectedType(mapperDropdown);
            if(mapperType != null) car.AddComponent(mapperType);
            // Select Path Planner
            var plannerType = GetSelectedType(pathPlannerDropdown);
            if(plannerType != null) car.AddComponent(plannerType);
            // Select Localizer
            var localizer = GetSelectedType(localizerDropdown);
            if(localizer != null) car.AddComponent(localizer);
            // Toggle Proximity Sensor
            if (proximitySensorToggle.isOn)
                car.AddComponent(typeof(ProximityDetection));
            HideMenu();
            ShowOverlay();
        }

        private static Type GetSelectedType(TMP_Dropdown dropdown)
        {
            var cd = dropdown.options[dropdown.value] as CustomOptionData;
            if (cd.CustomData == "null")
                return null;
            else
            {
                return Type.GetType(cd.CustomData, true);
            }
            //return cd.CustomData == "null" ?  null : Type.GetType(cd.CustomData, true);
        }
        private static void HideMenu()
        {
            GameObject.Find("MainMenu").SetActive(false);
        }


        private void ShowOverlay()
        {
            infoOverlay.gameObject.SetActive(true);
        }
    }

}