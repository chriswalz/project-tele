using UnityEngine;
using System.Collections;

public class DistanceMission : MissionTemplate 
{
    public enum DistanceMissionType { InOneRun, InMultipleRun, NoCoin, NoPowerup }

    public string description;
    public int requiredDistance;

    public DistanceMissionType missionType;

    private int storedValue;
    private int receivedValue;

    private bool isCompleted;

    //Updates the mission
    public override void UpdateMission(int distance, int collectedCoins, bool powerupUsed)
    {
        receivedValue = distance;

        if (missionType == DistanceMissionType.InOneRun && requiredDistance < distance)
            isCompleted = true;
        else if (missionType == DistanceMissionType.InMultipleRun && requiredDistance < storedValue + distance)
            isCompleted = true;
        else if (missionType == DistanceMissionType.NoCoin && requiredDistance < distance && collectedCoins == 0)
            isCompleted = true;
        else if (missionType == DistanceMissionType.NoPowerup && requiredDistance < distance && !powerupUsed)
            isCompleted = true;
    }
    //Set mission completion
    public override void SetCompletition(bool toValue)
    {
        isCompleted = toValue;
    }
    //Returns true, if the mission is completed
    public override bool IsCompleted()
    {
        return isCompleted;
    }
    //Update the stored value
    public override void SetStoredValue(int savedValue)
    {
        storedValue = savedValue;
    }
    //Return the mission data to be saved
    public override int MissionData()
    {
        if (missionType == DistanceMissionType.InMultipleRun)
            return storedValue + receivedValue;
        else
            return 0;
    }
    //Returns the mission description
    public override string MissionDescription()
    {
        return description;
    }
    //Returns the mission status
    public override string MissionStatus()
    {
        if (!isCompleted)
            return (storedValue + receivedValue) + "/" + requiredDistance;
        else
            return requiredDistance + "/" + requiredDistance;
    }

    //Not implemented for this mission type
    public override void UpdateMission(int missionValue)
    {
        throw new System.NotImplementedException();
    }
    //Not implemented for this mission type
    public override void UpdateMission(string missionValue)
    {
        throw new System.NotImplementedException();
    }
}
