using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าผู้เล่นเดินเข้ามาสัมผัสหรือไม่
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // ส่งตำแหน่งของ Checkpoint นี้ไปบันทึกไว้ที่ตัวผู้เล่น
                player.UpdateCheckPoint(transform.position);

                Debug.Log("Checkpoint Reached: " + gameObject.name);

                // (Optional) คุณอาจจะใส่ Effect หรือเปลี่ยนสีเพื่อบอกว่าเซฟแล้ว
                // gameObject.GetComponent<Renderer>().material.color = Color.green;
            }
        }
    }
}