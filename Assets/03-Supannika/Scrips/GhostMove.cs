using UnityEngine;

public class MonsterPatrol3D : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 3.0f;
    private Vector3 targetPosition;

    void Start()
    {
        // เริ่มต้นเป้าหมายไปที่จุด B
        if (pointB != null) targetPosition = pointB.position;
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        // เคลื่อนที่ในโลก 3 มิติ
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // หมุนหน้ามอนสเตอร์ไปทางเป้าหมาย (เพื่อให้ดูเป็นธรรมชาติใน 3D)
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }

        // เมื่อถึงจุดหมายให้สลับไปอีกจุด
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = (targetPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    // ฟังก์ชันตรวจสอบการชนสำหรับ 3D
    private void OnCollisionEnter(Collision collision)
    {
        // ตรวจสอบ Tag "Player" (อย่าลืมไปตั้งค่าที่ตัวผู้เล่น)
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player attacked");
        }
    }
}