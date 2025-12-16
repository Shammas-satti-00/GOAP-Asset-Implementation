using UnityEngine;

public class HungerSystem : MonoBehaviour
{
    public float maxHunger = 100f;
    public float currentHunger;
    public float hungerRate = 5f; // How fast hunger increases per second
    public bool IsHungry => currentHunger >= maxHunger;

    private void Start()
    {
        currentHunger = 0f;
    }

    private void Update()
    {
        // Increase hunger over time
        currentHunger += hungerRate * Time.deltaTime;
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);
    }

    public void Eat(float amount)
    {
        currentHunger -= amount;
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);
    }
}
