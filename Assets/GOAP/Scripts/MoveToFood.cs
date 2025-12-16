//using UnityEngine;
//using System.Collections;

//public class MoveToFood : Action
//{
//    public float speed = 3f;
//    public float eatAmount = 20f;
//    public float eatInterval = 0.5f;

//    private GameObject targetFood;
//    private CharacterController controller;
//    private GOAPAgent agent;
//    private HungerSystem hungerSystem;

//    private void Start()
//    {
//        controller = GetComponent<CharacterController>();
//        agent = GetComponent<GOAPAgent>();
//        hungerSystem = GetComponent<HungerSystem>();
//    }

//    public override bool PrePerform()
//    {
//        Debug.Log("[MoveToFood] Checking hunger...");
//        if (!hungerSystem.IsHungry)
//        {
//            Debug.Log("[MoveToFood] Not hungry, skipping food action.");
//            return false;
//        }

//        // Find nearest unclaimed and reachable food
//        Food[] allFoods = GameObject.FindObjectsByType<Food>(FindObjectsSortMode.None);
//        float closestDistance = Mathf.Infinity;
//        Food chosenFood = null;

//        foreach (Food f in allFoods)
//        {
//            if (f.isClaimed) continue;

//            float dist = Vector3.Distance(transform.position, f.transform.position);

//            // Optional: add obstacle check here (raycast)
//            if (dist < closestDistance)
//            {
//                closestDistance = dist;
//                chosenFood = f;
//            }
//        }

//        if (chosenFood != null)
//        {
//            targetFood = chosenFood.gameObject;
//            chosenFood.isClaimed = true;
//            Debug.Log($"[MoveToFood] Chose food: {targetFood.name}");
//            return true;
//        }

//        Debug.Log("[MoveToFood] No available food found!");
//        return false;
//    }


//    public override bool Perform()
//    {
//        if (targetFood == null) return false;

//        // Move toward food
//        Vector3 direction = (targetFood.transform.position - transform.position).normalized;
//        controller.Move(direction * speed * Time.deltaTime);

//        // Rotate smoothly
//        if (direction != Vector3.zero)
//            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 8f * Time.deltaTime);

//        // Check if reached
//        if (Vector3.Distance(transform.position, targetFood.transform.position) < 1f)
//        {
//            agent.worldState.SetState("isAtTarget", true);
//            StartCoroutine(EatFoodCoroutine());
//            return true;
//        }

//        return false;
//    }

//    public IEnumerator EatFoodCoroutine()
//    {
//        Debug.Log("[MoveToFood] Started eating...");
//        Food foodComponent = targetFood.GetComponent<Food>();

//        while (foodComponent != null && hungerSystem.IsHungry)
//        {
//            foodComponent.Consume(eatAmount);
//            hungerSystem.Eat(eatAmount);
//            yield return new WaitForSeconds(eatInterval);
//        }

//        if (foodComponent != null)
//            foodComponent.isClaimed = false;

//        Debug.Log("[MoveToFood] Finished eating!");
//        targetFood = null;
//        StartCoroutine(agent.AgentLoop()); // Start the agent’s goal-processing loop

//    }

//    public override bool PostPerform()
//    {
//        return true;
//    }
//}
































using UnityEngine;
using System.Collections;

public class MoveToFood : Action
{
    public float speed = 3f;
    public float eatAmount = 1f;
    public float eatInterval = 0.5f;

    private CharacterController controller;
    private GOAPAgent agent;

    private Food targetFood;
    private GameObject targetObject;

    private bool isEating = false;
    private bool isFinished = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        agent = GetComponent<GOAPAgent>();
    }

    public override bool PrePerform()
    {
        FindNearestFood();
        isFinished = false;

        return targetFood != null;
    }

    private void FindNearestFood()
    {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");

        Food nearestFood = null;
        GameObject nearestObj = null;
        float closest = Mathf.Infinity;

        foreach (GameObject f in foods)
        {
            Food foodComp = f.GetComponent<Food>();

            // Skip if already claimed
            if (foodComp.isClaimed)
                continue;

            float dist = Vector3.Distance(transform.position, f.transform.position);
            if (dist < closest)
            {
                closest = dist;
                nearestObj = f;
                nearestFood = foodComp;
            }
        }

        if (nearestFood != null)
        {
            targetFood = nearestFood;
            targetObject = nearestObj;

            // Claim the food so other players ignore it
            targetFood.isClaimed = true;
        }
        else
        {
            targetFood = null;
            targetObject = null;
        }
    }


    public override bool Perform()
    {
        // If we are done eating, tell GOAP the action is finished
        if (isFinished)
        {
            return true;
        }

        // If food was destroyed while moving, complete the action
        if (targetFood == null)
        {
            isFinished = true;
            return true;
        }

        // If we just started eating, wait for coroutine
        if (isEating)
        {
            return false;
        }

        // ---------- MOVEMENT ----------
        Vector3 direction = (targetObject.transform.position - transform.position).normalized;
        controller.Move(direction * speed * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * 6f
        );

        // Close enough → start eating
        if (Vector3.Distance(transform.position, targetObject.transform.position) < 1.2f)
        {
            isEating = true;
            StartCoroutine(EatFood());
        }

        return false;
    }

    private IEnumerator EatFood()
    {
        while (targetFood != null && targetFood.health > 0f)
        {
            targetFood.Consume(eatAmount);
            yield return new WaitForSeconds(eatInterval);
        }

        // Release claim
        if (targetFood != null)
            targetFood.isClaimed = false;

        isEating = false;
        isFinished = true;

        targetFood = null;
        targetObject = null;
    }


    public override bool PostPerform()
    {
        return true;
    }
}
