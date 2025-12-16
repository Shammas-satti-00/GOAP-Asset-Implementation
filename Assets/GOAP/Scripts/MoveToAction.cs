using UnityEngine;
using System.Collections;

public class MoveToAction : Action
{
    public float speed = 3f;

    private GameObject target;
    private CharacterController controller;
    private GOAPAgent agent;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        agent = GetComponent<GOAPAgent>();
        Debug.Log("[MoveToAction] Start called");
    }

    public override bool PrePerform()
    {
        Debug.Log("[MoveToAction] PrePerform called");

        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        if (foods.Length == 0)
        {
            Debug.Log("[MoveToAction] No food found!");
            return false;
        }

        float closestDistance = Mathf.Infinity;
        Food chosenFood = null;

        foreach (GameObject f in foods)
        {
            Food foodComponent = f.GetComponent<Food>();
            if (foodComponent != null && !foodComponent.isClaimed)
            {
                float dist = Vector3.Distance(transform.position, f.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    chosenFood = foodComponent;
                    target = f;
                }
            }
        }

        if (chosenFood != null)
        {
            chosenFood.isClaimed = true;
            Debug.Log("[MoveToAction] Food claimed: " + target.name);
            return true;
        }

        Debug.Log("[MoveToAction] No available food to claim");
        return false;
    }

    public override bool Perform()
    {
        if (target == null)
        {
            Debug.Log("[MoveToAction] Target is null in Perform");
            return false;
        }

        Vector3 direction = (target.transform.position - transform.position).normalized;
        controller.Move(direction * speed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                8f * Time.deltaTime
            );
        }

        Debug.Log("[MoveToAction] Moving toward: " + target.name);

        if (Vector3.Distance(transform.position, target.transform.position) < 1f)
        {
            Debug.Log("[MoveToAction] Reached food: " + target.name);
            Food foodComponent = target.GetComponent<Food>();
            if (foodComponent != null)
            {
                foodComponent.Consume(1f);
                foodComponent.isClaimed = false; // Release claim after consumption
            }
            target = null;
            return true;
        }

        return false;
    }

    public override bool PostPerform()
    {
        Debug.Log("[MoveToAction] PostPerform called");
        return true;
    }
}
