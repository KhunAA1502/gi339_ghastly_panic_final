using UnityEngine;
using UnityEngine.AI; // จำเป็นต้องใช้สำหรับ NavMesh

public class GhostAI : MonoBehaviour
{
    public Transform player;      // ลากตัว Player มาใส่ในช่องนี้
    private NavMeshAgent agent;

    [Header("Ghost Settings")]
    public float chaseDistance = 10f; // ระยะที่ผีจะเริ่มมองเห็นและตาม
    public float stopDistance = 2f;  // ระยะที่ผีจะหยุดเมื่อใกล้ตัวผู้เล่น

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // ถ้าลืมลาก Player มาใน Inspector ให้มันหาเองอัตโนมัติจาก Tag
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (player == null || agent == null || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseDistance)
        {
            Debug.Log("ผีเห็น Player แล้ว! กำลังไปหาที่: " + player.position);
            agent.SetDestination(player.position);
        }
        else
        {
            // ถ้าขึ้นข้อความนี้บ่อยๆ แสดงว่าเราอยู่ไกลเกินไป
            Debug.Log("Player อยู่ไกลเกินไป ผีเลยขี้เกียจเดิน");
        }
    }

    // วาดเส้นวงกลมในหน้า Scene เพื่อให้เราเห็นระยะการมองเห็นของผี
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}