using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openSpeed = 2f;
    private bool shouldOpen = false;
    private Quaternion targetRotation;

    void Update()
    {
        if (shouldOpen)
        {
            // ค่อยๆ หมุนประตูไปยังจุดหมายอย่างนุ่มนวล
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);

            // ปิดระบบหมุนเมื่อใกล้ถึงจุดหมายเพื่อประหยัดทรัพยากร
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                shouldOpen = false;
            }
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกจาก PuzzleManager โดยส่งองศาเป้าหมายมาให้
    public void StartOpening(Vector3 targetEulerAngles)
    {
        targetRotation = Quaternion.Euler(targetEulerAngles);
        shouldOpen = true;
    }
}