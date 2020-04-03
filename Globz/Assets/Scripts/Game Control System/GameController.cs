using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour 
{

	public RaycastSpawner[] spawners;

	private RaycastSpawner activeSpawner;

	private bool mouseDown = false;

	[SerializeField] private bool inGameplay = false;
	public AudioController audioController;
	public AudioClip gameplayMusic;

	// Update is called once per frame
	void Update () 
	{
		CheckInputs();
	}

	private void CheckInputs()
	{

		// Check our touches and touch phase to determine where to shoot 
		if (Input.touches.Length > 0)
		{
			Touch touch = Input.touches [0];

			if (touch.phase == TouchPhase.Began)
			{
				TouchDown (touch.position);
			} 
			else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) 
			{
				TouchUp (Input.mousePosition);
			} 
			else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
			{
				TouchMove (touch.position);
			}
			TouchMove (touch.position);	
			return;
		} 
		else if (Input.GetMouseButtonDown (0)) 
		{
			mouseDown = true;
			TouchDown (Input.mousePosition);
		} 
		else if (Input.GetMouseButtonUp (0))
		{
			mouseDown = false;
			TouchUp (Input.mousePosition);
		} 
		else if (mouseDown) 
		{
			TouchMove (Input.mousePosition);
		}
	}

	private RaycastSpawner GetAvailableSpawners()
	{
		// Send an event and get all of the available spawners based on the listening spawners
		// if none are available, then return nothing
		// btw we should return a gameobject or something
		RaycastSpawner raycastSpawner = null;
		return raycastSpawner;
		
	}
	
	void TouchDown (Vector2 touch) 
	{
		if (!inGameplay)
		{
			return;
		}
		
		activeSpawner = null;
		Vector2 point = Camera.main.ScreenToWorldPoint (touch);

		if (point.y < -1f)
		{
			var minDistance = 100000.0f;
			RaycastSpawner shooter = null;

			//look for closest shooter
			foreach (var s in spawners)
			{
				var d = Vector2.Distance (point, s.transform.position);
				if (d < minDistance) 
				{
					minDistance = d;
					shooter = s;
				}
			}
			activeSpawner = shooter;

		}
	}

	void TouchUp (Vector2 touch) 
	{
		if (!inGameplay)
		{
			return;
		}
		
		if (activeSpawner == null)
			return;
		
		Vector2 point = Camera.main.ScreenToWorldPoint (touch);
		if (Vector2.Distance (point, activeSpawner.transform.position) < 0.2f) 
		{
			activeSpawner.ClearShotPath ();
		} 
		else 
		{
			activeSpawner.HandleTouchUp (touch);
		}
	}

	void TouchMove (Vector2 touch) 
	{
		if (!inGameplay)
		{
			return;
		}
		if (activeSpawner == null)
			return;
		activeSpawner.HandleMoveOnTouch (touch);
	}

	public void StartGame()
	{
		StartCoroutine(StartGameRoutine());
	}

	private IEnumerator StartGameRoutine()
	{
		yield return new WaitForSeconds(0.5f);
		inGameplay = true;
		audioController.PlayMusic(gameplayMusic);
	}


}
