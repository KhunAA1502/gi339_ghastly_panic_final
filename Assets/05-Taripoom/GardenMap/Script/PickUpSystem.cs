using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpSystem : MonoBehaviour
{
    [Header("Settings")]
    public float pickUpRange = 5f;
    public Transform holdPoint;

    [Header("Status (Read Only)")]
    [SerializeField] private GameObject grabbedObject;
    private Rigidbody grabbedRb;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (grabbedObject == null) TryPickUp();
            else DropObject();
        }
    }

    // ใช้ FixedUpdate สำหรับงานฟิสิกส์ จะทำให้การขยับนิ่งกว่า Update ปกติ
    void FixedUpdate()
    {
        if (grabbedObject != null && grabbedRb != null)
        {
            MoveObject();
        }
    }

    void TryPickUp()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange))
        {
            if (hit.collider.CompareTag("PuzzleItem"))
            {
                grabbedObject = hit.collider.gameObject;
                grabbedRb = grabbedObject.GetComponent<Rigidbody>();

                if (grabbedRb != null)
                {
                    grabbedRb.isKinematic = false;
                    grabbedRb.useGravity = false;
                    grabbedRb.drag = 15;
                    grabbedRb.constraints = RigidbodyConstraints.FreezeRotation;
                    grabbedObject.transform.SetParent(holdPoint);
                    grabbedObject.transform.localScale = Vector3.one;
                }
            }
        }
    }

    void MoveObject()
    {
        // ถ้า grabbedObject ถูกสั่งให้เป็น null จากข้างนอก (เช่น แท่นวาง) 
        // ฟังก์ชันนี้จะหยุดส่งแรงทันทีในเฟรมนี้เลย
        if (grabbedObject == null || grabbedRb == null) return;

        float distance = Vector3.Distance(grabbedObject.transform.position, holdPoint.position);
        if (distance > 0.1f)
        {
            Vector3 moveDirection = (holdPoint.position - grabbedObject.transform.position);
            grabbedRb.AddForce(moveDirection * 300f, ForceMode.Acceleration);
        }
    }

    public void DropObject()
    {
        if (grabbedObject != null)
        {
            if (grabbedRb != null)
            {
                grabbedRb.useGravity = true;
                grabbedRb.drag = 1;
                grabbedRb.constraints = RigidbodyConstraints.None;
                grabbedRb.velocity = Vector3.zero;
                grabbedRb.angularVelocity = Vector3.zero;
                grabbedRb = null; // ล้างค่า Rigidbody ทันที
            }

            grabbedObject.transform.SetParent(null);
            grabbedObject = null; // ล้างค่า Object ทันที
        }
    }
}