using UnityEngine;
using System.Collections;

public static class SaveManager : object 
{
	public static int coinAmmount = 1500;                   //The ammount of coins the player has
    public static int bestDistance = 0;                     //The best distance the player has reached

    public static int extraSpeed = 0;                       //The ammount of extra speed power ups the player has
    public static int shield = 0;                           //The ammount of shield power ups the player has
    public static int sonicWave = 0;                        //The ammount of sonic wave power ups the player has
    public static int revive = 0;                           //The ammount of revive power ups the player has

	public static int currentSkinID = 0;                    //The current submarine skin ID (0 is the default skin)
	public static int skin2Unlocked = 0;                    //Hold the skin 2 owned state
	public static int skin3Unlocked = 0;					//Holds the skin 3 owned state

    public static int audioEnabled = 1;

    public static int[] missionID;                          //Mission 1, 2 and 3 ID
    public static int[] missionData;                        //Mission 1, 2 and 3 Data

    public static string missionDataString = "";            //Saved mission data string

    //Loads the player data
    public static void LoadData()
    {
        //If found the coin ammount data, load the datas
        if (!PlayerPrefs.HasKey("Coin ammount"))
            SaveData();
        else
        {
            coinAmmount = PlayerPrefs.GetInt("Coin ammount");
            bestDistance = PlayerPrefs.GetInt("Best Distance");

            extraSpeed = PlayerPrefs.GetInt("Extra Speed");
            shield = PlayerPrefs.GetInt("Shield");
            sonicWave = PlayerPrefs.GetInt("Sonic Wave");
            revive = PlayerPrefs.GetInt("Revive");

            audioEnabled = PlayerPrefs.GetInt("AudioEnabled");
        }

		if (!PlayerPrefs.HasKey("Skin ID"))
		{
			PlayerPrefs.SetInt("Skin ID", currentSkinID);
			PlayerPrefs.SetInt("Skin 2 Unlocked", skin2Unlocked);
		}
		else
		{
			currentSkinID = PlayerPrefs.GetInt("Skin ID");
			skin2Unlocked = PlayerPrefs.GetInt("Skin 2 Unlocked");
		}

		if (!PlayerPrefs.HasKey("Skin 3 Unlocked"))
			PlayerPrefs.SetInt("Skin 3 Unlocked", skin3Unlocked);
		else
			skin3Unlocked = PlayerPrefs.GetInt("Skin 3 Unlocked");

		PlayerPrefs.Save();
    }
    //Saves the player data
    public static void SaveData()
    {
        PlayerPrefs.SetInt("Coin ammount", coinAmmount);
        PlayerPrefs.SetInt("Best Distance", bestDistance);

        PlayerPrefs.SetInt("Extra Speed", extraSpeed);
        PlayerPrefs.SetInt("Shield", shield);
        PlayerPrefs.SetInt("Sonic Wave", sonicWave);
        PlayerPrefs.SetInt("Revive", revive);

        PlayerPrefs.SetInt("AudioEnabled", audioEnabled);

		PlayerPrefs.SetInt("Skin ID", currentSkinID);
		PlayerPrefs.SetInt("Skin 2 Unlocked", skin2Unlocked);
		PlayerPrefs.SetInt("Skin 3 Unlocked", skin3Unlocked);

        PlayerPrefs.Save();
    }
    //Loads the mission data
    public static void LoadMissionData()
    {
        missionID = new int[3] { -1, -1, -1 };
        missionData = new int[3] { 0, 0, 0 };

        if (!PlayerPrefs.HasKey("Missions"))
        {
            SaveMissionData();
        }
        else
        {
            missionDataString = PlayerPrefs.GetString("Missions");

            missionID[0] = PlayerPrefs.GetInt("Mission1");
            missionID[1] = PlayerPrefs.GetInt("Mission2");
            missionID[2] = PlayerPrefs.GetInt("Mission3");

            missionData[0] = PlayerPrefs.GetInt("Mission1Data");
            missionData[1] = PlayerPrefs.GetInt("Mission2Data");
            missionData[2] = PlayerPrefs.GetInt("Mission3Data");
        }
    }
    //Saves the mission data
    public static void SaveMissionData()
    {
        PlayerPrefs.SetInt("Mission1", missionID[0]);
        PlayerPrefs.SetInt("Mission2", missionID[1]);
        PlayerPrefs.SetInt("Mission3", missionID[2]);

        PlayerPrefs.SetInt("Mission1Data", missionData[0]);
        PlayerPrefs.SetInt("Mission2Data", missionData[1]);
        PlayerPrefs.SetInt("Mission3Data", missionData[2]);

        PlayerPrefs.SetString("Missions", missionDataString);
    }
    //Reset mission data
    public static void ResetMissions()
    {
        missionID = new int[3] { -1, -1, -1 };
        missionData = new int[3] { 0, 0, 0 };

        missionDataString = "";

        SaveMissionData();
    }
}