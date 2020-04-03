using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridCreator : MonoBehaviour 
{
	[Header("Grid Generation")]
	#region Grid Generation
	public int rowCount = 20;

	public int columnCount = 14;

	public float tileSize = 0.68f;

	public float gridFallRate = 0.1f;

	public float changeTypeRate = 0.5f;

	public int emptyLines = 16;

	public GameObject gridGlobGO;

	[HideInInspector]
	public float gridOffsetX = 0;
	
	[HideInInspector] 
	public float gridOffsetY = 0;	// Optional if you want spacing on the Y
	#endregion

	[Header("Grid Object Information")]
	#region Grid Object Info
	[HideInInspector]
	public List<List<Glob>> gridGlobs;

	private List<Glob> matchList;

	private List<GlobData> typePool;
	public GlobData[] allGlobData;

	private GlobData lastType;

	private int bullets = 0;
	#endregion
	
	[Header("Miscellaneous")]
	private static readonly System.Random GeneratedRandomNumber = new System.Random(); 

	void Start () 
	{
		matchList = new List<Glob> ();
		lastType = allGlobData[UnityEngine.Random.Range(0, 5)];
		typePool = new List<GlobData> ();

		var i = 0;
		var total = 100000;
		while (i < total)
		{
			typePool.Add (GetGlobType ());
			i++;
		}

		Shuffle(typePool);

		CreateGrid();

	}


	void BuildGrid ()	
	{
		gridGlobs = new List<List<Glob>> ();
		float gridOffsetMultiplier = 0.5f;

		gridOffsetX = (columnCount * tileSize) * gridOffsetMultiplier;
		gridOffsetX -= tileSize * gridOffsetMultiplier;


		for (int row = 0; row < rowCount; row++)
		{

			var rowGlobs = new List<Glob>();

			for (int column = 0; column < columnCount; column++) 
			{

				var item = Instantiate (gridGlobGO) as GameObject;
				var glob = item.GetComponent<Glob>();

				glob.SetGlobPosition(this, (column + (int)20), (row + (int)10));
				glob.SetGlobType (UnityEngine.Random.Range(0,4));
				typePool.RemoveAt (0);

				glob.transform.parent = gameObject.transform;
				rowGlobs.Add (glob);

				if (gridGlobs.Count < emptyLines ) 
				{
					glob.gameObject.SetActive (false);
				}
			}
				
			gridGlobs.Add(rowGlobs);
		}

		var p = transform.position;
		p.y -= 4.7f;
		transform.position = p;

	}

	private void CreateGrid()
	{
		//List<GameObject> globsInGrid = new List<GameObject>();
		gridGlobs = new List<List<Glob>> ();
		var rowGlobs = new List<Glob>();

		for (int rows = 0; rows < rowCount; rows++)
		{
			for (int columns = 0; columns < columnCount; columns++)
			{
				var item = Instantiate (gridGlobGO) as GameObject;
				var glob = item.GetComponent<Glob>();
				
				glob.gameObject.transform.parent = gameObject.transform;
				glob.SetGlobPosition(this, columns, rows);
				
				glob.SetGlobType (UnityEngine.Random.Range(0,4));

			}
			gridGlobs.Add(rowGlobs);
		}
	}


	// Create another line on the grid when the grid starts getting cleared
	void AddLine () 
	{
		rowCount++;

		var rowGlobs = new List<Glob>();

		for (int column = 0; column < columnCount; column++) 
		{

			var item = Instantiate (gridGlobGO) as GameObject;

			var glob = item.GetComponent<Glob>();
			glob.transform.parent = gameObject.transform;
			glob.SetGlobPosition(this, column, gridGlobs.Count-1);
			glob.SetGlobType (UnityEngine.Random.Range(0,4));
			glob.connected = true;

			typePool.RemoveAt (0);

			rowGlobs.Add (glob);
		}
		gridGlobs.Add(rowGlobs);

	}


	public void AddGlob (Glob collisionGlob, Bullet projectile) 
	{

		var neighbors = GlobEmptyNeighbors(collisionGlob);
		var minDistance = 10000.0f;
		Glob minGlob = null;
		foreach (var neighbor in neighbors) 
		{
			var distance = Vector2.Distance (neighbor.transform.position, projectile.transform.position);
			if (distance < minDistance) 
			{
				minDistance = distance;
				minGlob = neighbor;
			}
		}
		
		projectile.gameObject.SetActive (false);
		minGlob.SetGlobType (UnityEngine.Random.Range(0,4));
		minGlob.gameObject.SetActive (true);

		CheckForMatches (minGlob);

	}

	// Check for matches based on the Glob value
	public void CheckForMatches (Glob globChecker)
	{
		matchList.Clear ();
		//Compare the glob scriptable object against the one being hit
		// If they're the same, add their values
		// Otherwise, stick to it

		foreach (var r in gridGlobs) 
		{
			foreach (var glob in r) 
			{
				glob.visited = false;
			}
		}


		// Search for matches around the Glob
		var initialResult = GetMatches( globChecker );
		matchList.AddRange (initialResult);

		while (true) 
		{
			var allVisited = true;
			for (var i = matchList.Count - 1; i >= 0 ; i--) 
			{
				var glob = matchList [i];
				if (!glob.visited) 
				{
					AddMatches (GetMatches (glob));
					allVisited = false;
				}
			}

			if (allVisited) 
			{
				if (matchList.Count > 2) 
				{
					foreach (var b in matchList) 
					{
						b.gameObject.SetActive (false);
					}

					CheckForDisconnected ();

					// Remove disconnected globs
					var i = gridGlobs.Count - 1;
					while (i >= 0) 
					{
						foreach (var b in gridGlobs[i]) 
						{
							if (!b.connected) 
							{
								b.gameObject.SetActive (false);
							}
						}
						i--;
					}
				}
				return;
			}
		}
	}


	// Check if we need to drop any globs
	void CheckForDisconnected () 
	{
		// Set all globs as disconnected
		foreach (var r in gridGlobs) 
		{
			foreach (var b in r) 
			{
				b.connected = false;
			}
		}
		// Connect visible globs in last row 
		foreach (var b in gridGlobs[rowCount-1]) 
		{
			if (b.gameObject.activeSelf)
				b.connected = true;
		}
		
		// Now set connect property on the rest of the globs
		var i = rowCount-1;
		while (i >= 0) 
		{
			foreach (var glob in gridGlobs[i]) 
			{
				if (glob.gameObject.activeSelf) 
				{
					var neighbors = GlobActiveNeighbors (glob);
					var connected = false;

					foreach (var n in neighbors) 
					{
						if (n.connected) 
						{
							connected = true;
							break;
						}
					}

					if (connected) 
					{
						glob.connected = true;
						foreach (var n in neighbors) 
						{
							if (n.gameObject.activeSelf) 
							{
								n.connected = true;
							}
						}
					} 
				}
			}
			i--;
		}
	}


	List<Glob> GetMatches (Glob glob) 
	{
		glob.visited = true;
		var result = new List<Glob> () { glob };
		var matchingGlobs = GlobActiveNeighbors (glob);

		// Check each Glob in the list for it's Glob Type and add it to the list if they match
		foreach (var matchingGlob in matchingGlobs) 
		{
			if (matchingGlob.currentGlobData == glob.currentGlobData) 
			{
				result.Add (matchingGlob);
			}
		}

		return result;
	}

	void AddMatches (List<Glob> matches) 
	{
		foreach (var b in matches) 
		{
			if (!matchList.Contains (b))
				matchList.Add (b);
		}
	}
	
	GlobData GetGlobType () 
	{
		var random = Random.Range (0.0f, 1.0f);
		if (random > changeTypeRate)
		{
			lastType = allGlobData[UnityEngine.Random.Range(0,5)];
		}
		return lastType;
	}
	
	List<Glob> GlobEmptyNeighbors (Glob glob) 
	{
		var result = new List<Glob> ();
		if (glob.column + 1 < columnCount) 
		{
			if (!gridGlobs [glob.row] [glob.column + 1].gameObject.activeSelf)
				result.Add (gridGlobs [glob.row] [glob.column + 1]);
		}

		// Left
		if (glob.column - 1 >= 0) 
		{
			if (!gridGlobs [glob.row] [glob.column - 1].gameObject.activeSelf)
				result.Add (gridGlobs [glob.row] [glob.column - 1]);
		}
		// Top
		if (glob.row - 1 >= 0) 
		{
			if (!gridGlobs [glob.row - 1] [glob.column].gameObject.activeSelf)
				result.Add (gridGlobs [glob.row - 1] [glob.column]);
		}

		// Bottom
		if (glob.row + 1 < gridGlobs.Count) 
		{
			if (!gridGlobs [glob.row + 1] [glob.column].gameObject.activeSelf)
				result.Add (gridGlobs [glob.row + 1] [glob.column]);
		}

		if (glob.column % 2 == 0) 
		{

			// Top-Left
			if (glob.row - 1 >= 0 && glob.column - 1 >= 0) 
			{
				if (!gridGlobs [glob.row - 1] [glob.column - 1].gameObject.activeSelf)
					result.Add (gridGlobs [glob.row - 1] [glob.column - 1]);
			}

			// Top-Right
			if (glob.row - 1 >= 0 && glob.column + 1 < columnCount) 
			{
				if (!gridGlobs [glob.row - 1] [glob.column + 1].gameObject.activeSelf)
					result.Add (gridGlobs [glob.row - 1] [glob.column + 1]);
			}
		} 
		else 
		{
			// Bottom-Left
			if (glob.row + 1 < gridGlobs.Count && glob.column - 1 >= 0) 
			{
				if (!gridGlobs [glob.row + 1] [glob.column - 1].gameObject.activeSelf)
					result.Add (gridGlobs [glob.row + 1] [glob.column - 1]);
			}

			// Bottom-Right
			if (glob.row + 1 < gridGlobs.Count && glob.column + 1 < columnCount) 
			{
				if (!gridGlobs [glob.row + 1] [glob.column + 1].gameObject.activeSelf)
					result.Add (gridGlobs [glob.row + 1] [glob.column + 1]);
			}
		}
		
		return result;
	}

	List<Glob> GlobActiveNeighbors (Glob glob)
	{
		var result = new List<Glob> ();
		// Right
		if (glob.column + 1 < columnCount) 
		{
			if (gridGlobs [glob.row] [glob.column + 1].gameObject.activeSelf)
				result.Add (gridGlobs [glob.row] [glob.column + 1]);
		}

		// Left
		if (glob.column - 1 >= 0)
		{
			if (gridGlobs [glob.row] [glob.column - 1].gameObject.activeSelf)
				result.Add (gridGlobs [glob.row] [glob.column - 1]);
		}
		// Bottom
		if (glob.row - 1 >= 0) 
		{
			if (gridGlobs [glob.row - 1] [glob.column].gameObject.activeSelf)
				result.Add (gridGlobs [glob.row - 1] [glob.column]);
		}

		// Top
		if (glob.row + 1 < gridGlobs.Count)
		{
			if (gridGlobs [glob.row + 1] [glob.column].gameObject.activeSelf)
				result.Add (gridGlobs [glob.row + 1] [glob.column]);
		}


		if (glob.column % 2 == 0) 
		{

			// Top-Left
			if (glob.row - 1 >= 0 && glob.column - 1 >= 0) 
			{
				if (gridGlobs [glob.row - 1] [glob.column - 1].gameObject.activeSelf)
					result.Add (gridGlobs [glob.row - 1] [glob.column - 1]);
			}

			// Top-Right
			if (glob.row - 1 >= 0 && glob.column + 1 < columnCount) 
			{
				if (gridGlobs [glob.row - 1] [glob.column + 1].gameObject.activeSelf)
					result.Add (gridGlobs [glob.row - 1] [glob.column + 1]);
			}
		} 
		else 
		{
			// Bottom-Left
			if (glob.row + 1 < gridGlobs.Count && glob.column - 1 >= 0) 
			{
				if (gridGlobs [glob.row + 1] [glob.column - 1].gameObject.activeSelf)
					result.Add (gridGlobs [glob.row + 1] [glob.column - 1]);
			}

			// Bottom-Right
			if (glob.row + 1 < gridGlobs.Count && glob.column + 1 < columnCount)
			{
				if (gridGlobs [glob.row + 1] [glob.column + 1].gameObject.activeSelf)
					result.Add (gridGlobs [glob.row + 1] [glob.column + 1]);
			}

		}

		return result;
	}

	public Glob GlobCloseToPoint (Vector2 point)
	{
		point.y -= transform.position.y;

		int columnChecks = Mathf.FloorToInt ((point.x + gridOffsetX + ( tileSize * 0.5f )) / tileSize);
		if (columnChecks < 0)
			columnChecks = 0;
		if (columnChecks >= columnCount)
			columnChecks = columnCount - 1;

		int rowChecks =  Mathf.FloorToInt (( ( tileSize * 0.5f ) + point.y )/  tileSize);
		if (rowChecks < 0) rowChecks = 0;
		if (rowChecks >= gridGlobs.Count) rowChecks = gridGlobs.Count - 1;
		
		return gridGlobs [rowChecks] [columnChecks];
	}

	public static void Shuffle<T>(IList<T> list)  
	{  
		int n = list.Count;  
		while (n > 1)
		{  
			n--;  
			int k = GeneratedRandomNumber.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
	
}
