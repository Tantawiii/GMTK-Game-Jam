using UnityEngine;
using UnityEngine.Events;

public class FixWater : MonoBehaviour
{
    [SerializeField] UnityEvent onFixed;
    
    void fixWater()
    {
        onFixed.Invoke();
    }
}
