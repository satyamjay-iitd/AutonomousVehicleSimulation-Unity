using UnityEngine;
using System.Collections;

public class TrafficSystemVehiclePlayer : TrafficSystemVehicle 
{
	public delegate void HasEnteredTrafficLightTrigger( TrafficSystemTrafficLight a_trafficLight );
	public HasEnteredTrafficLightTrigger hasEnteredTrafficLightTrigger;

	/// <summary>
	/// To use the HasEnteredTrafficLightTrigger all you need to do is add this to your script or put code in the function below
	/// 
	/// in void Start -
	///     [TrafficSystemVehiclePlayer].hasEnteredTrafficLightTrigger += ProcessHasEnteredTrafficLightTrigger;
	/// 
	/// in void Destroy -
	///     [TrafficSystemVehiclePlayer].hasEnteredTrafficLightTrigger -= ProcessHasEnteredTrafficLightTrigger;
	///
	/// Then define your own function
	///    void ProcessHasEnteredTrafficLightTrigger( TrafficSystemTrafficLight a_trafficLight )
	///    {
	///   	  do something in here...
	///    }
	/// 
	/// </summary>

	public override void Start () 
	{
		hasEnteredTrafficLightTrigger += ProcessHasEnteredTrafficLightTrigger;

		// no need to do anyting, we just need to override TrafficSystemVehicle since this is the player
	}

	public override void Update () 
	{
		// no need to do anyting, we just need to override TrafficSystemVehicle since this is the player
	}

	public void ProcessHasEnteredTrafficLightTrigger( TrafficSystemTrafficLight a_trafficLight )
	{
		// Debug.Log("Hit " + a_trafficLight + " and the light was " + a_trafficLight.m_status);
		// put code here...
	}
}
