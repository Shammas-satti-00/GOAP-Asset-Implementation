using System.Collections.Generic;

[System.Serializable]
public class GOAPGoal
{
    public string goalName;
    public Dictionary<string, bool> desiredState = new Dictionary<string, bool>();
}
