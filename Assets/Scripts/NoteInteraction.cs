using UnityEngine;

public class NoteInteraction : MonoBehaviour
{
    public GameObject pressE_Text; // ลาก PressE มาใส่
    public GameObject noteImage;   // ลาก Image (รูปกระดาษจดหมาย) มาใส่
    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // ถ้าโน้ตเปิดอยู่ ให้ปิด / ถ้าปิดอยู่ ให้เปิด
            bool isNoteActive = !noteImage.activeSelf;
            
            noteImage.SetActive(isNoteActive);
            
            // สลับสถานะ PressE (ถ้าเปิดโน้ตอยู่ ก็ซ่อนคำใบ้ไป)
            pressE_Text.SetActive(!isNoteActive);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (!noteImage.activeSelf) pressE_Text.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            pressE_Text.SetActive(false);
            noteImage.SetActive(false); // ปิดโน้ตทันทีถ้าเดินหนี
        }
    }
}