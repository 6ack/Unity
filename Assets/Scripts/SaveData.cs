using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveData
{
    public static void UpdateBestScore(int value)
    {
        if (value > GetBestScore())
        {
            PlayerPrefs.SetInt("BestScore", value);
        }
    }

    public static int GetBestScore()
    {
        return PlayerPrefs.GetInt("BestScore", 0);
    }

}
