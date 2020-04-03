using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUpdater : MonoBehaviour
{
    public int score;
    public TMP_Text scoreText;
    [SerializeField] private Vector3 scoredTextScale = Vector3.one;
    private Vector3 currentScale;

    private void Start()
    {
        scoreText.text = "Score: 0";
        currentScale = transform.localScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            UpdateScore(1);
        }
    }

    public void UpdateScore(int scoreAmount)
    {
        StartCoroutine(ScoreUpdateSequence(scoreAmount));
    }

    private IEnumerator ScoreUpdateSequence(int scoreAmount)
    {
        float scoreTimer = 0.1f;
        score += scoreAmount;
        LeanTween.scale(gameObject, scoredTextScale, scoreTimer);
        yield return new WaitForSeconds(0.1f);
        scoreText.text = "Score: " + score.ToString();
        LeanTween.scale(gameObject, currentScale, scoreTimer);
    }
}
