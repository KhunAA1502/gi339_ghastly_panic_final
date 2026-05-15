using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.UpdateCheckPoint(transform.position);

                Debug.Log("Checkpoint Reached: " + gameObject.name);
            }
        }
    }
}