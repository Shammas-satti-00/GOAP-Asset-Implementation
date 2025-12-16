using UnityEngine;
using System.Collections;

public class ImprovedMoveToFood : Action
{
    [Header("Movement")]
    public float speed = 3f;
    public float rotationSpeed = 6f;

    [Header("Eating")]
    public float eatDuration = 2f; // Eat for 2 seconds only
    public float hungerReductionAmount = 50f; // How much hunger is reduced

    [Header("Stuck Detection")]
    public float stuckCheckInterval = 1f; // Check every second
    public float stuckDistanceThreshold = 0.1f; // Min distance to move in interval
    public float maxStuckTime = 5f; // Give up after 5 seconds of being stuck

    private CharacterController controller;
    private GOAPAgent agent;
    private HungerSystem hungerSystem;

    private Food targetFood;
    private GameObject targetObject;

    private bool isEating = false;
    private bool isFinished = false;

    // Stuck detection variables
    private Vector3 lastPosition;
    private float timeSinceLastCheck;
    private float totalStuckTime;
    private bool isStuck = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        agent = GetComponent<GOAPAgent>();
        hungerSystem = GetComponent<HungerSystem>();
    }

    public override bool PrePerform()
    {
        FindNearestFood();
        isFinished = false;
        isStuck = false;
        totalStuckTime = 0f;
        lastPosition = transform.position;
        timeSinceLastCheck = 0f;

        if (targetFood == null)
        {
            Debug.Log("[ImprovedMoveToFood] No food available!");
            return false;
        }

        Debug.Log("[ImprovedMoveToFood] Target acquired: " + targetObject.name);
        return true;
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
            if (foodComp != null && foodComp.isClaimed)
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
        // If we're stuck for too long, give up
        if (isStuck)
        {
            Debug.Log("[ImprovedMoveToFood] Stuck! Giving up on this food.");
            CleanupFood();
            return true; // Complete action (failed)
        }

        // If finished eating, return success
        if (isFinished)
        {
            return true;
        }

        // If food was destroyed while moving, complete
        if (targetFood == null || targetObject == null)
        {
            Debug.Log("[ImprovedMoveToFood] Food disappeared!");
            isFinished = true;
            return true;
        }

        // If already eating, wait
        if (isEating)
        {
            return false;
        }

        // ---------- STUCK DETECTION ----------
        CheckIfStuck();

        // ---------- MOVEMENT ----------
        Vector3 direction = (targetObject.transform.position - transform.position).normalized;
        direction.y = 0; // Keep movement horizontal

        controller.Move(direction * speed * Time.deltaTime);

        // Rotation
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * rotationSpeed
            );
        }

        // Check if close enough to eat
        float distanceToFood = Vector3.Distance(transform.position, targetObject.transform.position);
        if (distanceToFood < 1.5f)
        {
            Debug.Log("[ImprovedMoveToFood] Reached food! Starting to eat...");
            isEating = true;
            StartCoroutine(EatForDuration());
        }

        return false;
    }

    private void CheckIfStuck()
    {
        timeSinceLastCheck += Time.deltaTime;

        if (timeSinceLastCheck >= stuckCheckInterval)
        {
            float distanceMoved = Vector3.Distance(lastPosition, transform.position);

            if (distanceMoved < stuckDistanceThreshold)
            {
                totalStuckTime += timeSinceLastCheck;
                Debug.Log($"[ImprovedMoveToFood] Possibly stuck! Total time: {totalStuckTime}s");

                if (totalStuckTime >= maxStuckTime)
                {
                    isStuck = true;
                }
            }
            else
            {
                // Reset if moving
                totalStuckTime = 0f;
            }

            lastPosition = transform.position;
            timeSinceLastCheck = 0f;
        }
    }

    private IEnumerator EatForDuration()
    {
        Debug.Log("[ImprovedMoveToFood] Eating for " + eatDuration + " seconds...");

        // Eat for the specified duration
        yield return new WaitForSeconds(eatDuration);

        // Reduce hunger
        if (hungerSystem != null)
        {
            hungerSystem.Eat(hungerReductionAmount);
            Debug.Log("[ImprovedMoveToFood] Hunger reduced by " + hungerReductionAmount);
        }

        // Update world state
        if (agent != null)
        {
            agent.worldState.SetState("IsHungry", false);
            agent.worldState.SetState("HasFood", false);
            Debug.Log("[ImprovedMoveToFood] World state updated: IsHungry = false");
        }

        CleanupFood();

        isEating = false;
        isFinished = true;

        Debug.Log("[ImprovedMoveToFood] Finished eating!");
    }

    private void CleanupFood()
    {
        // Release claim on food
        if (targetFood != null)
        {
            targetFood.isClaimed = false;
        }

        targetFood = null;
        targetObject = null;
    }

    public override bool PostPerform()
    {
        return true;
    }
}