using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 3.0f;
    private Vector3 targetPosition;

    [Header("Detection Settings")]
    public float detectionRange = 10.0f;
    public float chaseSpeed = 5.0f;

    private Transform playerTransform;
    private Rigidbody monsterRigidbody;
    private bool isChasing = false;

    void Start()
    {
        if (pointB != null) targetPosition = pointB.position;

        monsterRigidbody = GetComponent<Rigidbody>();

        // ตั้งค่า Rigidbody ผ่านโค้ดเพื่อป้องกันความผิดพลาด
        monsterRigidbody.useGravity = true;
        monsterRigidbody.isKinematic = false;
        monsterRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        monsterRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // ล็อกการหมุนไม่ให้มอนสเตอร์ล้มคว่ำ (Freeze Rotation X, Z)
        monsterRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            else return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < detectionRange) isChasing = true;
        else if (distanceToPlayer > detectionRange + 2f) isChasing = false;

        if (isChasing) ChasePlayer();
        else Patrol();
    }

    void Patrol()
    {
        if (pointA == null || pointB == null) return;

        MoveTowardsTarget(targetPosition, patrolSpeed);

        // เช็คระยะห่างโดยไม่สนแกน Y (แก้ปัญหามอนสเตอร์เดินไม่ถึงจุดเพราะจุดลอยหรือจม)
        Vector3 flatPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flatTarget = new Vector3(targetPosition.x, 0, targetPosition.z);

        if (Vector3.Distance(flatPos, flatTarget) < 0.5f)
        {
            targetPosition = (targetPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    void ChasePlayer()
    {
        MoveTowardsTarget(playerTransform.position, chaseSpeed);
    }

    void MoveTowardsTarget(Vector3 target, float speed)
    {
        // 1. หมุนตัวไปหาเป้าหมาย
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0; // ไม่ให้เงยหน้าหรือก้มหน้าตามเป้าหมาย

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }

        // 2. เคลื่อนที่ (ใช้การย้ายตำแหน่งตรงๆ แต่คงค่า Velocity Y ไว้เพื่อให้ฟิสิกส์แรงโน้มถ่วงทำงาน)
        Vector3 movePos = Vector3.MoveTowards(transform.position,
                          new Vector3(target.x, transform.position.y, target.z),
                          speed * Time.deltaTime);

        transform.position = movePos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // เรียกใช้ Respawn จากภาพโค้ด PlayerController ของคุณ
            var pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc != null) pc.Respawn();

            // วาร์ปกลับจุดเดินที่ใกล้ที่สุดทันทีเพื่อลดอาการดีด
            isChasing = false;
            float distToA = Vector3.Distance(transform.position, pointA.position);
            transform.position = (distToA < Vector3.Distance(transform.position, pointB.position)) ? pointA.position : pointB.position;
        }
    }
}