using UnityEngine;

public class MonsterTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าสิ่งที่ชนคือผู้เล่น (ใช้ Tag "Player")
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.Respawn();
                Debug.Log("Player touched monster! Respawning...");
            }
        }
    }
}