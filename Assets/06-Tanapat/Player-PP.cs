using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 5f; // ความเร็วตอนย่อ เดียวกับ walkSpeed
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;
    public float minPitch = -40f;
    public float maxPitch = 80f;

    [Header("Climb Settings")]
    public float stepHeight = 0.3f; // ความสูงของขั้นบันไดที่ก้าวข้ามได้
    public float stepSmooth = 0.1f; // ความนุ่มนวลในการก้าว

    [Header("Crouch Settings")]
    public float normalHeight = 2f; // ความสูงปกติของ CapsuleCollider
    public float crouchHeight = 1.2f; // ความสูงตอนย่อของ CapsuleCollider

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private float pitch = 0f;
    private bool isCrouching = false;
    private Vector3 normalCameraPosition;
    private Vector3 crouchCameraPosition;
    private Vector3 normalCenter;
    private Vector3 crouchCenter;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // กันตัวละครล้มคว่ำ
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        capsuleCollider = GetComponent<CapsuleCollider>();
        normalCameraPosition = cameraTransform.localPosition;
        crouchCameraPosition = new Vector3(normalCameraPosition.x, normalCameraPosition.y - 0.5f, normalCameraPosition.z);
        normalCenter = capsuleCollider.center;
        crouchCenter = new Vector3(normalCenter.x, crouchHeight / 2, normalCenter.z);
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up, mouseX);
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }

        // ตรวจสอบการกด Ctrl เพื่อย่อ
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            isCrouching = true;
            capsuleCollider.height = crouchHeight;
            capsuleCollider.center = crouchCenter;
            cameraTransform.localPosition = crouchCameraPosition;
            transform.position += new Vector3(0, crouchCenter.y - normalCenter.y, 0); // ปรับตำแหน่ง player ลง
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            isCrouching = false;
            capsuleCollider.height = normalHeight;
            capsuleCollider.center = normalCenter;
            cameraTransform.localPosition = normalCameraPosition;
            transform.position += new Vector3(0, normalCenter.y - crouchCenter.y, 0); // ปรับตำแหน่ง player ขึ้น
        }
    }

    void FixedUpdate()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 movement = transform.forward * vertical + transform.right * horizontal;
        movement.y = 0; // ตัดแกน Y เพื่อไม่ให้เดินแล้วตัวลอย
        movement.Normalize();

        float speed = isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);
        
        // --- ส่วนที่แก้ไขเรื่องบันได ---
        if (movement.magnitude > 0.1f) 
        {
            StepClimb(movement);
        }

        Vector3 velocity = movement * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    // ฟังก์ชันช่วยก้าวข้ามขั้นบันได
    void StepClimb(Vector3 direction)
    {
        // ยิง Raycast เช็คว่าข้างหน้ามีขั้นบันไดไหม
        RaycastHit hitLower;
        if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), direction, out hitLower, 0.6f))
        {
            RaycastHit hitUpper;
            // เช็คว่าความสูงนั้นไม่เกินค่าที่เรากำหนด (Step Height)
            if (!Physics.Raycast(transform.position + new Vector3(0, stepHeight, 0), direction, out hitUpper, 0.7f))
            {
                // ถ้าข้างล่างชนแต่ข้างบนไม่ชน แสดงว่าเป็นขั้นบันได ให้เพิ่มแรงส่งขึ้นเบาๆ
                rb.position += new Vector3(0f, stepSmooth, 0f);
            }
        }
    }
}