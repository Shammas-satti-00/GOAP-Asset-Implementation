//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GOAPAgent : MonoBehaviour
//{
//    public WorldState worldState;
//    public List<GOAPAction> availableActions = new List<GOAPAction>();
//    public List<GOAPGoal> goals = new List<GOAPGoal>();
//    private GOAPPlanner planner;

//    private void Start()
//    {
//        planner = new GOAPPlanner();
//        StartCoroutine(AgentLoop());
//    }

//    // Main loop for the agent
//    public IEnumerator AgentLoop()
//    {
//        while (true)
//        {
//            GOAPGoal goalToPerform = ChooseGoal();

//            if (goalToPerform == null)
//            {
//                Debug.Log("[GOAPAgent] No valid goal available!");
//                yield return new WaitForSeconds(1f); // Wait a bit before trying again
//                continue;
//            }

//            var plan = planner.Plan(worldState, goalToPerform, availableActions);

//            if (plan != null)
//            {
//                Debug.Log("[GOAPAgent] Plan found!");
//                yield return StartCoroutine(ExecutePlan(plan));
//            }
//            else
//            {
//                Debug.Log("[GOAPAgent] No plan could be found for goal: " + goalToPerform.goalName);
//                yield return new WaitForSeconds(1f); // Wait a bit before trying again
//            }
//        }
//    }

//    // Choose a valid goal based on conditions
//    private GOAPGoal ChooseGoal()
//    {
//        HungerSystem hunger = GetComponent<HungerSystem>();

//        foreach (var g in goals)
//        {
//            if (g.goalName == "EatFood" && !hunger.IsHungry) continue;
//            return g;
//        }

//        return null;
//    }

//    // Execute a plan safely
//    private IEnumerator ExecutePlan(List<GOAPAction> plan)
//    {
//        foreach (var action in plan)
//        {
//            while (!action.Perform())
//                yield return null;

//            action.PostPerform();
//        }

//        Debug.Log("[GOAPAgent] Goal Completed!");
//        yield return null; // Give Unity one frame before looping back
//    }
//}






























using System.Collections.Generic;
using UnityEngine;

public class GOAPAgent : MonoBehaviour
{
    public WorldState worldState;

    [Header("Assign a GOAP Goal ScriptableObject")]
    public GOAPGoalSO goalSO;   // Drag your goal here in Inspector

    private GOAPGoal goal;
    private List<GOAPAction> actions;
    private Queue<GOAPAction> actionQueue;
    private GOAPPlanner planner;

    private Action currentAction;

    void Start()
    {
        // WORLD STATE
        worldState = new WorldState();
        worldState.SetState("HasFood", false);
        worldState.SetState("IsHungry", true);
        worldState.SetState("isAtTarget", false);

        // ACTIONS (same as your original implementation)
        actions = new List<GOAPAction>();

        // MOVE TO FOOD
        GOAPAction move = new GOAPAction();
        move.actionName = "MoveToFood";
        move.preconditions["isAtTarget"] = false;
        move.effects["isAtTarget"] = true;
        actions.Add(move);

        // PICK FOOD
        GOAPAction pickFood = new GOAPAction();
        pickFood.actionName = "PickFood";
        pickFood.preconditions["isAtTarget"] = true;
        pickFood.preconditions["HasFood"] = false;
        pickFood.effects["HasFood"] = true;
        actions.Add(pickFood);

        // EAT FOOD
        GOAPAction eat = new GOAPAction();
        eat.actionName = "Eat";
        eat.preconditions["HasFood"] = true;
        eat.effects["IsHungry"] = false;
        actions.Add(eat);

        // ---------------------------
        // USE SCRIPTABLEOBJECT GOAL
        // ---------------------------
        goal = ConvertSOToGoal(goalSO);


        // PLAN
        planner = new GOAPPlanner();
        List<GOAPAction> plan = planner.Plan(worldState, goal, actions);

        if (plan == null)
        {
            Debug.LogError("No valid GOAP plan found!");
            return;
        }

        actionQueue = new Queue<GOAPAction>(plan);

        Debug.Log("Plan found:");
        foreach (var a in actionQueue)
            Debug.Log(a.actionName);

        PrepareNextAction();
    }

    void Update()
    {
        if (currentAction == null) return;

        if (currentAction.Perform())
        {
            currentAction.PostPerform();
            PrepareNextAction();
        }
    }

    // -----------------------------------------
    // Convert ScriptableObject → GOAPGoal
    // -----------------------------------------
    private GOAPGoal ConvertSOToGoal(GOAPGoalSO so)
    {
        GOAPGoal newGoal = new GOAPGoal();
        newGoal.goalName = so.goalName;

        foreach (var entry in so.desiredStates)
        {
            newGoal.desiredState[entry.key] = entry.value;
        }

        return newGoal;
    }

    // -----------------------------------------
    // Begin next action in the plan
    // -----------------------------------------
    private void PrepareNextAction()
    {
        if (actionQueue == null || actionQueue.Count == 0)
        {
            currentAction = null;
            Debug.Log("GOAL COMPLETED!");
            return;
        }

        GOAPAction next = actionQueue.Dequeue();

        currentAction = GetComponent(next.actionName) as Action;

        if (currentAction == null)
        {
            Debug.LogError("Missing MonoBehaviour for GOAP Action: " + next.actionName);
            PrepareNextAction();
            return;
        }

        if (!currentAction.PrePerform())
        {
            Debug.LogError("PrePerform failed for: " + next.actionName);
            PrepareNextAction();
        }
    }
}








































//using System.Collections.Generic;
//using UnityEngine;

//public class GOAPAgent : MonoBehaviour
//{
//    public WorldState worldState;

//    private List<GOAPAction> actions;
//    private Queue<GOAPAction> actionQueue;
//    private GOAPGoal goal;
//    private GOAPPlanner planner;

//    private Action currentAction;

//    void Start()
//    {
//        // WORLD STATE
//        worldState = new WorldState();
//        worldState.SetState("HasFood", false);
//        worldState.SetState("IsHungry", true);
//        worldState.SetState("isAtTarget", false);

//        // ACTIONS
//        actions = new List<GOAPAction>();

//        // MOVE TO FOOD
//        GOAPAction move = new GOAPAction();
//        move.actionName = "MoveToFood";
//        move.preconditions["isAtTarget"] = false;
//        move.effects["isAtTarget"] = true;
//        actions.Add(move);

//        // PICK FOOD
//        GOAPAction pickFood = new GOAPAction();
//        pickFood.actionName = "PickFood";
//        pickFood.preconditions["isAtTarget"] = true;
//        pickFood.preconditions["HasFood"] = false;
//        pickFood.effects["HasFood"] = true;
//        actions.Add(pickFood);

//        // EAT FOOD
//        GOAPAction eat = new GOAPAction();
//        eat.actionName = "Eat";
//        eat.preconditions["HasFood"] = true;
//        eat.effects["IsHungry"] = false;
//        actions.Add(eat);

//        // GOAL
//        goal = new GOAPGoal();
//        goal.goalName = "NotHungry";
//        goal.desiredState["IsHungry"] = false;

//        // PLAN
//        planner = new GOAPPlanner();
//        List<GOAPAction> plan = planner.Plan(worldState, goal, actions);

//        if (plan == null)
//        {
//            Debug.Log("No valid plan found!");
//            return;
//        }

//        // Convert to queue so we can run actions one by one
//        actionQueue = new Queue<GOAPAction>(plan);

//        Debug.Log("Plan found:");
//        foreach (var a in actionQueue)
//            Debug.Log(a.actionName);

//        PrepareNextAction();
//    }

//    void Update()
//    {
//        if (currentAction == null) return;

//        // Execute the current Action
//        if (currentAction.Perform())
//        {
//            // Action completed
//            currentAction.PostPerform();
//            PrepareNextAction();
//        }
//    }
//    private void PrepareNextAction()
//    {
//        if (actionQueue == null || actionQueue.Count == 0)
//        {
//            currentAction = null;
//            Debug.Log("GOAL COMPLETED!");
//            return;
//        }

//        GOAPAction next = actionQueue.Dequeue();

//        // Find the Action component matching this action name
//        currentAction = GetComponent(next.actionName) as Action;

//        if (currentAction == null)
//        {
//            Debug.LogError("No Action script found for: " + next.actionName);
//            return;
//        }

//        if (!currentAction.PrePerform())
//        {
//            Debug.LogError("PrePerform failed for: " + next.actionName);
//            currentAction = null;
//            return;
//        }
//    }
//}
