using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreMultiplier : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreMultiplierTMP;

    private Vector3 defaultScale;

    private int currentScoreMultiplier = 1;
    // Start is called before the first frame update
    void Start()
    {
        defaultScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(ShowMultiplier());
        }
    }

    private IEnumerator ShowMultiplier()
    {
        scoreMultiplierTMP.text = currentScoreMultiplier.ToString() + "x";
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, defaultScale * 2, 0.3f).setEaseInSine();
        yield return new WaitForSeconds(.3f);
        LeanTween.scale(gameObject, defaultScale, 0.3f).setEaseInSine();
        LeanTween.alphaText(scoreMultiplierTMP.rectTransform, 0, 0.5f);

    }
}
