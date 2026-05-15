using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public PuzzleManager puzzleManager;
    public Transform snapPoint;
    public AudioClip snapSound;
    private AudioSource audioSource;
    private bool isOccupied = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isOccupied && other.CompareTag("PuzzleItem"))
        {
            // 1. สั่งให้สคริปต์ผู้เล่นปล่อยมือและล้างค่าตัวแปรทันที
            PickUpSystem player = Camera.main.GetComponent<PickUpSystem>();
            if (player != null) player.DropObject();

            // 2. ตัดขาดความสัมพันธ์พ่อลูกกับ HoldPoint (กันเหนียว)
            other.transform.SetParent(null);

            SnapObject(other.gameObject);
        }
    }

    void SnapObject(GameObject item)
    {
        isOccupied = true;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // แช่แข็งฟิสิกส์
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // 3. วาร์ปเข้าจุด Snap ทันที
        item.transform.position = snapPoint.position;
        item.transform.rotation = snapPoint.rotation;
        item.transform.localScale = Vector3.one;

        // 4. เปลี่ยน Tag เพื่อให้หยิบไม่ได้อีก
        item.tag = "Untagged";

        if (snapSound != null) audioSource.PlayOneShot(snapSound);
        if (puzzleManager != null) puzzleManager.ItemAdded();

        this.enabled = false; // ปิดสคริปต์ตัวเองกันเสียงรัว
    }
}