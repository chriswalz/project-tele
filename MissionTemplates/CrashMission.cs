using UnityEngine;
using System.Collections;

public class CrashMission : MissionTemplate
{
    public enum CrashMissionTypes { CrashBefore, CrashBetween, CrashAfter };

    public string description;

    public CrashMissionTypes missionType;

    public int minDistance;
    public int maxDistance;

    private bool isCompleted;

    //Updates the mission
    public override void UpdateMission(int missionValue)
    {
        if (missionType == CrashMissionTypes.CrashBefore && missionValue < minDistance)
            isCompleted = true;
        else if (missionType == CrashMissionTypes.CrashBetween && minDistance <= missionValue && maxDistance >= missionValue)
            isCompleted = true;
        else if (missionType == CrashMissionTypes.CrashAfter && missionValue > maxDistance)
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
    public override void UpdateMission(string missionValue)
    {
        throw new System.NotImplementedException();
    }
    //Not implemented for this mission type
    public override void UpdateMission(int distance, int collectedCoins, bool powerupUsed)
    {
        throw new System.NotImplementedException();
    }
}
