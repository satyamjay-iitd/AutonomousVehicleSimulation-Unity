using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Controller;
using Mapping;
using Localization;
using PathPlanning;
using Perception.LaneDetection;
using Perception.ObstacleDetection;
using Perception.SignalDetection;
using TMPro;
using UnityEngine;

namespace MainMenu
{
    public class MainMenuRenderer : MonoBehaviour
    {
        private void Start()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            ////////////////////////////////////// Setup Controller Selector \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            var controllerDropdown = gameObject.GetComponent<Transform>().Find("ControllerDropdown").GetComponent<TMP_Dropdown>();
            var controllers = GetTypesWithInterface(assemblies, typeof(IController));
            PopulateDropdown(controllerDropdown, controllers, "");

            //////////////////////////////////// Setup Lane Detection Selector \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            var laneDetectorDropdown = gameObject.GetComponent<Transform>().Find("Perception").Find("LaneDetectionDropdown").GetComponent<TMP_Dropdown>();
            var laneDetectors = GetTypesWithInterface(assemblies, typeof(LaneDetection));
            PopulateDropdown(laneDetectorDropdown, laneDetectors, "Off");

            ////////////////////////////////// Setup Obstacle Detection Selector \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            var obstacleDropdown = gameObject.GetComponent<Transform>().Find("Perception").Find("ObstacleDetectionDropdown").GetComponent<TMP_Dropdown>();
            var obstacleDetectors = GetTypesWithInterface(assemblies, typeof(ObstacleDetection));
            PopulateDropdown(obstacleDropdown, obstacleDetectors, "Off");

            ///////////////////////////////////// Setup Signal Detection Selector \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            var signalDetectorDropdown = gameObject.GetComponent<Transform>().Find("Perception").Find("SignalDetectionDropdown").GetComponent<TMP_Dropdown>();
            var signalDetectors = GetTypesWithInterface(assemblies, typeof(SignalDetection));
            PopulateDropdown(signalDetectorDropdown, signalDetectors, "Off");
            
            //////////////////////////////////////// Setup Path Planner Selector \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            var plannerDropdown = gameObject.GetComponent<Transform>().Find("Planning").Find("PathPlanningDropdown").GetComponent<TMP_Dropdown>();
            var planners = GetTypesWithInterface(assemblies, typeof(IPlanner));
            PopulateDropdown(plannerDropdown, planners, "Beacon");
            
            //////////////////////////////////////// Setup Mapping Selector \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            var mapperDropdown = gameObject.GetComponent<Transform>().Find("Planning").Find("MappingDropdown").GetComponent<TMP_Dropdown>();
            var mappers = GetTypesWithInterface(assemblies, typeof(IMapper));
            PopulateDropdown(mapperDropdown, mappers, "Manual");
            
            //////////////////////////////////////// Setup Localization Selector \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            var localizationDropdown = gameObject.GetComponent<Transform>().Find("Planning").Find("LocalizationDropdown").GetComponent<TMP_Dropdown>();
            var localizers = GetTypesWithInterface(assemblies, typeof(ILocalizer));
            PopulateDropdown(localizationDropdown, localizers, "Off");
        }
        
        private static void PopulateDropdown(TMP_Dropdown dropdown, IEnumerable<Type> classes, string nullString)
        {
            if (nullString != "")
            {
                TMP_Dropdown.OptionData optionData = new CustomOptionData(nullString, "null");
                dropdown.options.Add(optionData);
            }
            foreach (var cClass in classes)
            {
                TMP_Dropdown.OptionData optionData = new CustomOptionData(cClass.Name, cClass.FullName);
                dropdown.options.Add(optionData);
            }
            dropdown.RefreshShownValue();
        }
        
        private IEnumerable<Type> GetTypesWithInterface(Assembly[] assemblies, Type type)
        {
            var types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                types = types.Concat(GetLoadableTypes(assembly).
                        Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract).
                        ToList()).ToList();    
            }
            return types;
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }


    class CustomOptionData : TMP_Dropdown.OptionData
    {
        private string _customData;

        public string CustomData
        {
            get => _customData;
            set => _customData = value;
        }

        public CustomOptionData(string text, string customData) : base(text)
        {
            this._customData = customData;
        }
    } 
}

        // private void InitSelector(string title, IEnumerable<Type> classes, int index)
        // { 
        //     var selector = Instantiate(selectorCanvasTemplate, _mainCanvas.transform);
        //     selector.name = title + "Selector";
        //     selector.GetComponent<Transform>()
        //         .Find("Title").GetComponent<TextMeshProUGUI>()
        //         .SetText(title);
        //     // Setting Location of selector
        //     selector.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
        //     selector.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
        //     selector.transform.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        //     selector.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(200*index+20, 0);
        //     selector.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 195);
        //     
        //     var i = 0;
        //     foreach (var cClass in classes) {
        //         Toggle toggle = Toggle.Instantiate(toggleTemplate, selector.transform);
        //         toggle.GetComponent<Transform>().Find("Label").GetComponent<Text>().text = cClass.Name;
        //         toggle.group = selector.GetComponent<Transform>().Find("ControllerToggleGroup").GetComponent<ToggleGroup>();
        //         
        //         toggle.isOn = false;
        //         // Setting location of toggle
        //         toggle.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        //         toggle.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
        //         toggle.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
        //         toggle.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(70 + i * 30));
        //         toggle.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 20);
        //         
        //         toggle.gameObject.SetActive(true);
        //         i++;
        //     }
        //     selector.gameObject.SetActive(true);
        // }