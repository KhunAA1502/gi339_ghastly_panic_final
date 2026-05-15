using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 lastCheckPoint;

    void Start()
    {
        // กำหนดจุดเริ่มต้นให้เป็นเช็คพอยท์แรก
        lastCheckPoint = transform.position;
    }

    // ฟังก์ชันสำหรับอัปเดตจุดเช็คพอยท์
    public void UpdateCheckPoint(Vector3 newPos)
    {
        lastCheckPoint = newPos;
    }

    // ฟังก์ชันสำหรับส่งผู้เล่นกลับจุดเช็คพอยท์
    public void Respawn()
    {
        transform.position = lastCheckPoint;

        // หากใช้ฟิสิกส์ (Rigidbody) ควร Reset ความเร็วด้วย
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}