using UnityEngine;
using System.Collections;

public class RoamAction : Action
{
    public float roamSpeed = 2f;
    public float roamRadius = 10f;
    public float waypointReachDistance = 0.5f;

    private CharacterController controller;
    private Vector3 targetPosition;
    private bool hasReachedWaypoint = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public override bool PrePerform()
    {
        // Pick a random point to roam to
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection.y = 0; // Keep on ground level
        targetPosition = transform.position + randomDirection;

        hasReachedWaypoint = false;

        Debug.Log("[RoamAction] Starting roam to: " + targetPosition);
        return true;
    }

    public override bool Perform()
    {
        if (hasReachedWaypoint)
            return true;

        // Move toward target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Keep movement horizontal

        controller.Move(direction * roamSpeed * Time.deltaTime);

        // Rotate toward movement direction
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 5f
            );
        }

        // Check if reached waypoint
        float distance = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(targetPosition.x, 0, targetPosition.z)
        );

        if (distance < waypointReachDistance)
        {
            hasReachedWaypoint = true;
            Debug.Log("[RoamAction] Reached waypoint!");
            return true;
        }

        return false;
    }

    public override bool PostPerform()
    {
        return true;
    }
}