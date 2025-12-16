//using UnityEngine;

//public class Food : MonoBehaviour
//{
//    public float health = 1f;
//    public bool isClaimed = false;

//    public void Consume(float amount)
//    {
//        health -= amount;
//        if (health <= 0f)
//        {
//            Destroy(gameObject);
//        }
//    }
//}





using UnityEngine;

public class Food : MonoBehaviour
{
    public float health = 1f;
    public bool isClaimed = false;

    public void Consume(float amount)
    {
        health -= amount;
        Debug.Log("[Food] " + gameObject.name + " consumed. Remaining health: " + health);

        if (health <= 0f)
        {
            Debug.Log("[Food] " + gameObject.name + " destroyed!");
            Destroy(gameObject);
        }
    }
}
