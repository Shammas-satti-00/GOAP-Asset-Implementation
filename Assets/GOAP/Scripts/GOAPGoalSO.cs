using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGOAPGoal", menuName = "GOAP/Goal")]
public class GOAPGoalSO : ScriptableObject
{
    public string goalName;

    [Tooltip("Desired world states for this goal.")]
    public List<WorldStateEntry> desiredStates = new List<WorldStateEntry>();
}

[System.Serializable]
public class WorldStateEntry
{
    public string key;
    public bool value;
}
