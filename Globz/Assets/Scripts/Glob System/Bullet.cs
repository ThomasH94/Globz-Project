using UnityEngine;
using System.Collections;
using TMPro;


public class Bullet : MonoBehaviour 
{

	public GameObject globGO;
	private Glob glob;
	private GlobData bulletGlobData;
	public GlobData[] allGlobData;

	[SerializeField] private SpriteRenderer globBody;
	public int globValue;
	public TMP_Text globValueText;

	private void Start()
	{
		SetType();
	}

	public void SetType ()
	{
		int randomGlob = UnityEngine.Random.Range(0, 7);    // We don't want to start at 2048 so let's use weights instead of excluding
		globBody.color = allGlobData[randomGlob].globColor;
		globValue = allGlobData[randomGlob].globValue;
		globValueText.text = globValue.ToString();

		//colorsGO [(int)type].SetActive (true);
	}

}
