using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public float respawnTime = 10f;
    public Transform spawnPoint;

    private void Start()
    {
        SpawnFood();
    }

    private void SpawnFood()
    {
        GameObject food = Instantiate(foodPrefab, spawnPoint.position, Quaternion.identity);
        Food foodComponent = food.GetComponent<Food>();

        // When food is destroyed, respawn after delay
        StartCoroutine(WaitForDestruction(food));
    }

    private System.Collections.IEnumerator WaitForDestruction(GameObject food)
    {
        while (food != null)
            yield return null;

        yield return new WaitForSeconds(respawnTime);
        SpawnFood();
    }
}