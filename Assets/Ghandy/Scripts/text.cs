using UnityEngine;

public class text : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlayLoop(SoundType.Footstep, gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
