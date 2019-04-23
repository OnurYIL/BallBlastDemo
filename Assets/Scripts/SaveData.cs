using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData {

    public static void SaveCoins(int value)
    {
        PlayerPrefs.SetInt("coins", value);
    }
    public static int LoadCoins()
    {
        return PlayerPrefs.GetInt("coins");
    }
    public static void SaveFireRate(int value)
    {
        PlayerPrefs.SetInt("fireRate", value);
    }
    public static int LoadFireRate()
    {
        return PlayerPrefs.GetInt("fireRate");
    }
}
