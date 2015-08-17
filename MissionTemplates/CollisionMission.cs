using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionMission : MissionTemplate
{
    public string description;
    public List<string> requiredCollision;

    private bool isCompleted;

    //Updates the mission
    public override void UpdateMission(string missionValue)
    {
        if (requiredCollision.Contains(missionValue))
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
    public override void SetStoredValue(int savedValue) { }
    //Return the mission data to be saved
    public override int MissionData()
    {
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
        return "";
    }

    //Not implemented for this mission type
    public override void UpdateMission(int missionValue)
    {
        throw new System.NotImplementedException();
    }
    //Not implemented for this mission type
    public override void UpdateMission(int distance, int collectedCoins, bool powerupUsed)
    {
        throw new System.NotImplementedException();
    }
}
