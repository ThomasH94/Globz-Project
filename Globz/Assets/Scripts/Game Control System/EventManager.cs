using UnityEngine;
using System.Collections;

public class EventManager {

	public delegate void Event_ShootBall ();

	public static event Event_ShootBall  OnShootBall;

	public static void ShootBall () 
	{
		if (OnShootBall != null)
			OnShootBall ();
	}
}
