using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour 
{
    public GUIManager guiManager;                                   //A link to the GUI Manager
    public LevelManager levelManager;                               //A link to the Level Manager
    public PowerupManager powerupManager;                           //A link to the Powerup Manager
    
	public MissionTemplate[] missions;						        //List of missions

	private int[] missionID;					                    //The ID of the active missions
    private int[] missionData;				                        //Stores the saved data for the active missions
    private string data = "";										//The data string containing the saved status

    //Load the saved data
    public void LoadData()
    {
        missionID = SaveManager.missionID;
        missionData = SaveManager.missionData;

        data = SaveManager.missionDataString;

        //If a mission is removed, reset the data string
        if (data.Length > missions.Length)
            ResetMissions();

        //If a mission was added, update the data string
        else if (data.Length < missions.Length)
            AddNewMissions();

        //Set completition status for the missions
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == '1')
                missions[i].SetCompletition(true);
        }

        //If a mission slot is empty, or contains a completed mission, look for a new mission, if possible
        for (int i = 0; i < 3; i++)
        {
            if (missionID[i] == -1 || missions[missionID[i]].IsCompleted())
                GetNewMission(i);
        }

        //Load saves mission data
        for (int i = 0; i < 3; i++)
        {
			if (missionID[i] != -1 && !missions[missionID[i]].IsCompleted())
                missions[missionID[i]].SetStoredValue(missionData[i]);
        }

        SaveManager.SaveMissionData();

        //Update mission GUI texts
        //UpdateGUITexts();
    }
    //Saves the mission data
    public void SaveData()
    {
        missionData = new int[3];

        for (int i = 0; i < missionID.Length; i++)
        {
            if (missionID[i] == -1)
                missionData[i] = 0;
            else
                missionData[i] = missions[missionID[i]].MissionData();
        }

        SaveManager.missionData = missionData;
        SaveManager.missionID = missionID;

        SaveManager.missionDataString = data;

        SaveManager.SaveMissionData();
    }

    public string[] GetMissionTexts()
    {
        string[] missionTexts = new string[missionID.Length];

        for (int i = 0; i < missionID.Length; i++)
            if (missionID[i] > -1)
                missionTexts[i] = missions[missionID[i]].MissionDescription();
            else
                missionTexts[i] = "Completed";

        return missionTexts;
    }
    public string[] GetMissionStats()
    {
        string[] missionStatus = new string[missionID.Length];

        for (int i = 0; i < missionID.Length; i++)
            if (missionID[i] > -1)
                missionStatus[i] = missions[missionID[i]].MissionStatus();
            else
                missionStatus[i] = "";

        return missionStatus;
    }

    //Distance event
    public void DistanceEvent(int distance)
    {
        for (int i = 0; i < missionID.Length; i++)
        {
            if (missionID[i] != -1 && missions[missionID[i]] is DistanceMission && !missions[missionID[i]].IsCompleted())
            {
                missions[missionID[i]].UpdateMission(distance, levelManager.CollectedCoins(), powerupManager.PowerupUsed());

                if (missions[missionID[i]].IsCompleted())
                    MissionCompleted(i);
                //else
                //    UpdateGUITexts();
            }
        }
    }
    //Coin event
    public void CoinEvent(int collectedCoins)
    {
        for (int i = 0; i < missionID.Length; i++)
        {
            if (missionID[i] != -1 && missions[missionID[i]] is CoinMission && !missions[missionID[i]].IsCompleted())
            {
                missions[missionID[i]].UpdateMission(collectedCoins);

                if (missions[missionID[i]].IsCompleted())
                    MissionCompleted(i);
                //else
                //    UpdateGUITexts();
            }
        }
    }
    //Crash event
    public void CrashEvent(int distance)
    {
        for (int i = 0; i < missionID.Length; i++)
        {
            if (missionID[i] != -1 && missions[missionID[i]] is CrashMission && !missions[missionID[i]].IsCompleted())
            {
                missions[missionID[i]].UpdateMission(distance);

                if (missions[missionID[i]].IsCompleted())
                    MissionCompleted(i);
            }
        }
    }
    //Crash event
    public void ShopEvent(string bought)
    {
        print(bought);
        for (int i = 0; i < missionID.Length; i++)
        {
            if (missionID[i] != -1 && missions[missionID[i]] is ShopMission && !missions[missionID[i]].IsCompleted())
            {
                missions[missionID[i]].UpdateMission(bought);

                if (missions[missionID[i]].IsCompleted())
                    MissionCompleted(i);
            }
        }
    }
    //Crash event
    public void CollisionEvent(string collidedWith)
    {
        for (int i = 0; i < missionID.Length; i++)
        {
            if (missionID[i] != -1 && missions[missionID[i]] is CollisionMission && !missions[missionID[i]].IsCompleted())
            {
                missions[missionID[i]].UpdateMission(collidedWith);

                if (missions[missionID[i]].IsCompleted())
                    MissionCompleted(i);
            }
        }
    }

    //Resets the mision data
    private void ResetMissions()
    {
        data = "";

        for (int i = 0; i < missions.Length; i++)
            data += "0";

        for (int i = 0; i < missionID.Length; i++)
            missionID[i] = -1;

        for (int i = 0; i < missionData.Length; i++)
            missionData[i] = 0;

        SaveManager.missionDataString = data;

        SaveManager.missionID = missionID;
        SaveManager.missionData = missionData;

    }
    //Adds new elements to the mission data string
    private void AddNewMissions()
    {
        for (int i = data.Length; i < missions.Length; i++)
            data += "0";

        SaveManager.missionDataString = data;
    }
    //Called when a mission is completed, notifies the player, and updates the mission data string
    private void MissionCompleted(int id)
    {
        guiManager.ShowMissionComplete(missions[missionID[id]].MissionDescription());

        char[] dataChar = data.ToCharArray();

        dataChar[missionID[id]] = '1';
        data = new string(dataChar);

        SaveData();

        if (!guiManager.InPlayMode())
        {
            for (int i = 0; i < 3; i++)
            {
                if (missionID[i] == -1 || missions[missionID[i]].IsCompleted())
                    GetNewMission(i);
            }
        }
    }
    //Gets a new mission to the given slot
    private void GetNewMission(int id)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == '0' && missionID[0] != i && missionID[1] != i && missionID[2] != i)
            {
                missionID[id] = i;
                missionData[id] = 0;
                return;
            }
        }

        missionID[id] = -1;
        missionData[id] = 0;
    }
}
