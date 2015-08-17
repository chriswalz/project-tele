using UnityEngine;
using System.Collections;

public class CoinMission : MissionTemplate
{
    public string description;
    public int requiredCoins;

    public bool inOneRune;

    private int storedValue;
    private int receivedValue;

    private bool isCompleted;

    //Updates the mission
    public override void UpdateMission(int missionValue)
    {
        receivedValue = missionValue;

        if (inOneRune && requiredCoins < missionValue)
            isCompleted = true;
        else if (!inOneRune && requiredCoins < storedValue + missionValue)
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
        if (!inOneRune)
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
            return (storedValue + receivedValue) + "/" + requiredCoins;
        else
            return requiredCoins + "/" + requiredCoins;
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
