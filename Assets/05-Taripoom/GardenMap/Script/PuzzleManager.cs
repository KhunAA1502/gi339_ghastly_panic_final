using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public int itemsPlaced = 0;
    public int requiredItems = 3;

    [Header("Door Setup")]
    public DoorController leftDoor;  // ลากประตูบานซ้ายมาใส่
    public DoorController rightDoor; // ลากประตูบานขวามาใส่
    public Vector3 leftDoorOpenRotation;  // องศาเป้าหมายบานซ้าย (เช่น Y = -90)
    public Vector3 rightDoorOpenRotation; // องศาเป้าหมายบานขวา (เช่น Y = 90)

    [Header("Audio")]
    public AudioClip doorOpenSound;
    private AudioSource audioSource;
    private bool isSolved = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void ItemAdded()
    {
        itemsPlaced++;
        if (itemsPlaced >= requiredItems && !isSolved)
        {
            Solve();
        }
    }

    public void ItemRemoved()
    {
        itemsPlaced--;
    }

    void Solve()
    {
        isSolved = true;
        Debug.Log("Puzzle Solved! Opening Gates...");

        // เล่นเสียง
        if (doorOpenSound != null) audioSource.PlayOneShot(doorOpenSound);

        // สั่งเปิดประตูพร้อมกัน 2 บาน
        if (leftDoor != null)
        {
            // คำนวณองศาเป้าหมายโดยอิงจากค่าเริ่มต้นของมัน
            Vector3 target = leftDoor.transform.eulerAngles + leftDoorOpenRotation;
            leftDoor.StartOpening(target);
        }

        if (rightDoor != null)
        {
            Vector3 target = rightDoor.transform.eulerAngles + rightDoorOpenRotation;
            rightDoor.StartOpening(target);
        }
    }
}