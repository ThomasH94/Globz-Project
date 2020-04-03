using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// This script will manage leveling up the player
public class UserLevelManagement : MonoBehaviour
{
    public int currentLevel;
    public TMP_Text currentLevelText;

    private int experienceToNextLevel;
    // Start is called before the first frame update
    void Start()
    {
        currentLevel = 1;
        //currentLevelText.text = currentLevel.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LevelUp()
    {
        currentLevel++;
        int nextLevelFormula = (currentLevel * 5 * 2048) / 2;
        experienceToNextLevel = nextLevelFormula;
    }
}
