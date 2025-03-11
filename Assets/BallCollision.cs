using UnityEngine;

public class BallCollision : MonoBehaviour
{
    public string[] zoneTags = { "Zone1", "Zone2", "Zone3" };

    void OnTriggerEnter2D(Collider2D other)
    {
        for (int i = 0; i < zoneTags.Length; i++)
        {
            if (other.CompareTag(zoneTags[i]))
            {
                Debug.Log("Ball entered " + zoneTags[i]);
               
            }
        }
    }
}