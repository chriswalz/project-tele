using UnityEngine;
using System.Collections;

public abstract class MissionTemplate : MonoBehaviour
{
    //Used in distance missions
    public abstract void UpdateMission(int distance, int collectedCoins, bool powerupUsed);
    //Used in coin and crash missions
    public abstract void UpdateMission(int missionValue);
    //Used in shop and collision missions
    public abstract void UpdateMission(string missionValue);
    //Set mission completion
    public abstract void SetCompletition(bool toValue);
    //Returns true, if the mission is completed
    public abstract bool IsCompleted();
    //Update the stored value
    public abstract void SetStoredValue(int savedValue);
    //Return the stored value to be saved
    public abstract int MissionData();
    //Returns the mission description
    public abstract string MissionDescription();
    //Returns the mission status
    public abstract string MissionStatus();
}
