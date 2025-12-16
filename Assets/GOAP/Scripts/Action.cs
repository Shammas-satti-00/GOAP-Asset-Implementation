using UnityEngine;

public abstract class Action : MonoBehaviour
{
    public abstract bool PrePerform();
    public abstract bool Perform();
    public abstract bool PostPerform();
}
