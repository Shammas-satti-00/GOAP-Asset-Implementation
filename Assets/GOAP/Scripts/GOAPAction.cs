//using System.Collections.Generic;
//using UnityEngine;

//[System.Serializable]
//public class GOAPAction
//{
//    public string actionName;
//    public Dictionary<string, bool> preconditions = new Dictionary<string, bool>();
//    public Dictionary<string, bool> effects = new Dictionary<string, bool>();
//    public float cost = 1f;

//    // Check if action can run in current world state
//    public bool ArePreconditionsMet(WorldState world)
//    {
//        foreach (var pre in preconditions)
//        {
//            if (!world.states.ContainsKey(pre.Key) || world.states[pre.Key] != pre.Value)
//                return false;
//        }
//        return true;
//    }

//    // These methods are now virtual, so derived classes like MoveToFood can override
//    public virtual bool PrePerform() { return true; }
//    public virtual bool Perform() { return true; }
//    public virtual bool PostPerform() { return true; }
//}




using System.Collections.Generic;

[System.Serializable]
public class GOAPAction
{
    public string actionName;
    public Dictionary<string, bool> preconditions = new Dictionary<string, bool>();
    public Dictionary<string, bool> effects = new Dictionary<string, bool>();
    public float cost = 1f;

    public bool ArePreconditionsMet(WorldState world)
    {
        foreach (var pre in preconditions)
        {
            if (!world.states.ContainsKey(pre.Key) || world.states[pre.Key] != pre.Value)
                return false;
        }
        return true;
    }
}
