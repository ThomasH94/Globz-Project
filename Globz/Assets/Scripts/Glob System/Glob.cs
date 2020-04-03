using System;
using UnityEngine;
using System.Collections;
using TMPro;
using Random = System.Random;

/*
 * The purpose of this class is to hold the information about the Glob and use it to move the Glob,
 * check for matches, and set our Glob when created
 */
public class Glob : MonoBehaviour
{
    public GlobData[] allGlobData;
    public GlobData currentGlobData;

    public Color globColor;

    [HideInInspector]
    public int row;

    [HideInInspector]
    public int column;
    
    public int globValue;

    private int globIndexer = 0;

    [HideInInspector]
    public bool visited;

    [HideInInspector]
    public bool connected;

    private Vector3 globPosition;

    private GridCreator grid;

    [SerializeField] private SpriteRenderer globBody;
    [SerializeField] private TMP_Text globValueText;
    public Animator anim;

    private void Start()
    {
        SetRandomGlobType();
        float waitTime = UnityEngine.Random.Range(0,3.0f);
        float randomWaitTime = UnityEngine.Random.Range(0, 10);
        InvokeRepeating("Blink",waitTime, randomWaitTime);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            SetRandomGlobType();
        }
    }


    public void SetGlobPosition (GridCreator grid, int column, int row)
    {
        this.grid = grid;
        this.column = column;
        this.row = row;

        globPosition = new Vector3 
        ( 
            (column * grid.tileSize) - grid.gridOffsetX , 
            (row * grid.tileSize),
            0);

        if (column % 2 == 0) 
        {
            globPosition.y -= grid.tileSize * 0.5f;
        }

        transform.localPosition = globPosition;
        
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("bullet")) 
        {
            var b = other.gameObject.GetComponent<Bullet> ();
            grid.AddGlob(this, b);
        }

    }

    public void SetGlobType (int globIndex)
    {
        currentGlobData = allGlobData[globIndex];
        globValue = currentGlobData.globValue;
        globBody.color = currentGlobData.globColor;
        globColor = currentGlobData.globColor;
        Debug.Log("My Glob color is " + currentGlobData.globColor + " and my glob value is " + currentGlobData.globValue);
    }

    public void SetRandomGlobType()
    {
        int randomGlob = UnityEngine.Random.Range(0, 7);    // We don't want to start at 2048 so let's use weights instead of excluding
        globBody.color = allGlobData[randomGlob].globColor;
        globValue = allGlobData[randomGlob].globValue;
        globValueText.text = globValue.ToString();
    }

    private void Blink()
    {
        anim.SetTrigger("Blink");
    }
    
}