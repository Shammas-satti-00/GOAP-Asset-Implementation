using System.Collections.Generic;
using System.Linq;

public class GOAPPlanner
{
    // Returns a simple plan (list of actions) to achieve the goal
    public List<GOAPAction> Plan(WorldState currentWorld, GOAPGoal goal, List<GOAPAction> availableActions)
    {
        List<GOAPAction> plan = new List<GOAPAction>();
        WorldState tempWorld = new WorldState { states = new Dictionary<string, bool>(currentWorld.states) };

        while (!GoalReached(goal, tempWorld))
        {
            // Find first action whose preconditions are met
            GOAPAction nextAction = availableActions
                .Where(a => a.ArePreconditionsMet(tempWorld))
                .FirstOrDefault();

            if (nextAction == null)
            {
                // No valid plan found
                return null;
            }

            plan.Add(nextAction);

            // Apply effects to temporary world state
            foreach (var effect in nextAction.effects)
            {
                tempWorld.SetState(effect.Key, effect.Value);
            }
        }

        return plan;
    }

    bool GoalReached(GOAPGoal goal, WorldState state)
    {
        foreach (var kv in goal.desiredState)
        {
            if (!state.states.ContainsKey(kv.Key) || state.states[kv.Key] != kv.Value)
                return false;
        }
        return true;
    }
}
