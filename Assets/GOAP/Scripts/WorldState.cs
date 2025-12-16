using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldState
{
    public Dictionary<string, bool> states = new Dictionary<string, bool>();

    public bool GetState(string key)
    {
        if (states.ContainsKey(key))
            return states[key];
        return false;
    }

    public void SetState(string key, bool value)
    {
        states[key] = value;
    }
}
