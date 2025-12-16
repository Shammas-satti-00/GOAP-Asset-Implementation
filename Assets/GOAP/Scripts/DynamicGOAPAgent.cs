using System.Collections.Generic;
using UnityEngine;

public class DynamicGOAPAgent : MonoBehaviour
{
    public WorldState worldState;

    [Header("Goals")]
    public GOAPGoalSO roamGoalSO;  // Goal for roaming
    public GOAPGoalSO eatGoalSO;   // Goal for eating

    [Header("Re-planning")]
    public float replanInterval = 2f; // Check if we need to replan every 2 seconds

    private List<GOAPAction> actions;
    private Queue<GOAPAction> actionQueue;
    private GOAPPlanner planner;
    private Action currentAction;
    private HungerSystem hungerSystem;

    private GOAPGoal currentGoal;
    private float timeSinceLastReplan;

    void Start()
    {
        hungerSystem = GetComponent<HungerSystem>();

        // WORLD STATE
        worldState = new WorldState();
        worldState.SetState("HasFood", false);
        worldState.SetState("IsHungry", false);
        worldState.SetState("IsRoaming", false);

        // DEFINE ACTIONS
        DefineActions();

        planner = new GOAPPlanner();

        // Start with initial plan
        Replan();
    }

    void Update()
    {
        // Update world state based on hunger
        if (hungerSystem != null)
        {
            worldState.SetState("IsHungry", hungerSystem.IsHungry);
        }

        // Check if we need to replan
        timeSinceLastReplan += Time.deltaTime;
        if (timeSinceLastReplan >= replanInterval)
        {
            timeSinceLastReplan = 0f;
            CheckForReplan();
        }

        // Execute current action
        if (currentAction != null)
        {
            if (currentAction.Perform())
            {
                currentAction.PostPerform();
                PrepareNextAction();
            }
        }
    }

    private void DefineActions()
    {
        actions = new List<GOAPAction>();

        // MOVE TO FOOD & EAT (combined into one action)
        GOAPAction moveAndEat = new GOAPAction();
        moveAndEat.actionName = "ImprovedMoveToFood";
        moveAndEat.preconditions["IsHungry"] = true;
        moveAndEat.effects["IsHungry"] = false;
        moveAndEat.cost = 1f;
        actions.Add(moveAndEat);

        // ROAM
        GOAPAction roam = new GOAPAction();
        roam.actionName = "RoamAction";
        roam.preconditions["IsHungry"] = false;
        roam.effects["IsRoaming"] = true;
        roam.cost = 1f;
        actions.Add(roam);
    }

    private void CheckForReplan()
    {
        // Determine which goal should be active
        GOAPGoal desiredGoal = DetermineGoal();

        // If goal changed or queue is empty, replan
        if (desiredGoal != currentGoal || actionQueue == null || actionQueue.Count == 0)
        {
            Debug.Log("[DynamicGOAPAgent] Replanning due to goal change or empty queue...");
            Replan();
        }
    }

    private GOAPGoal DetermineGoal()
    {
        // Priority: If hungry, eat. Otherwise, roam.
        if (worldState.GetState("IsHungry"))
        {
            return ConvertSOToGoal(eatGoalSO);
        }
        else
        {
            return ConvertSOToGoal(roamGoalSO);
        }
    }

    private void Replan()
    {
        currentGoal = DetermineGoal();

        Debug.Log("[DynamicGOAPAgent] Planning for goal: " + currentGoal.goalName);

        List<GOAPAction> plan = planner.Plan(worldState, currentGoal, actions);

        if (plan == null || plan.Count == 0)
        {
            Debug.LogWarning("[DynamicGOAPAgent] No valid plan found!");

            // Fallback: if no plan for eating, try roaming
            if (currentGoal.goalName == eatGoalSO.goalName)
            {
                Debug.Log("[DynamicGOAPAgent] Falling back to roaming...");
                currentGoal = ConvertSOToGoal(roamGoalSO);
                plan = planner.Plan(worldState, currentGoal, actions);
            }
        }

        if (plan != null && plan.Count > 0)
        {
            actionQueue = new Queue<GOAPAction>(plan);

            Debug.Log("[DynamicGOAPAgent] Plan created:");
            foreach (var a in plan)
            {
                Debug.Log("  - " + a.actionName);
            }

            PrepareNextAction();
        }
        else
        {
            Debug.LogError("[DynamicGOAPAgent] Still no valid plan after fallback!");
            currentAction = null;
            actionQueue = null;
        }
    }

    private void PrepareNextAction()
    {
        if (actionQueue == null || actionQueue.Count == 0)
        {
            currentAction = null;
            Debug.Log("[DynamicGOAPAgent] Plan completed!");
            return;
        }

        GOAPAction next = actionQueue.Dequeue();

        currentAction = GetComponent(next.actionName) as Action;

        if (currentAction == null)
        {
            Debug.LogError("[DynamicGOAPAgent] Missing MonoBehaviour: " + next.actionName);
            PrepareNextAction();
            return;
        }

        if (!currentAction.PrePerform())
        {
            Debug.LogWarning("[DynamicGOAPAgent] PrePerform failed for: " + next.actionName);
            PrepareNextAction();
        }
        else
        {
            Debug.Log("[DynamicGOAPAgent] Starting action: " + next.actionName);
        }
    }

    private GOAPGoal ConvertSOToGoal(GOAPGoalSO so)
    {
        if (so == null) return null;

        GOAPGoal newGoal = new GOAPGoal();
        newGoal.goalName = so.goalName;

        foreach (var entry in so.desiredStates)
        {
            newGoal.desiredState[entry.key] = entry.value;
        }

        return newGoal;
    }
}