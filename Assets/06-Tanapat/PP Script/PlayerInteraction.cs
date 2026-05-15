using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 3f;
    public float searchTime = 1f;
    private float timer = 0f;

    [Header("UI Objects (Blob)")]
    public GameObject searchHintObj;      // ลาก "Search Text" มาใส่
    public GameObject searchingStatusObj; // ลาก "Searching Text" มาใส่
    public GameObject nothingFoundObj;    // ลาก "NOTHING HERE..." มาใส่
    public GameObject foundKeyObj;        // ลาก "YOU FOUND THE KEY!" มาใส่
    
    [Header("UI Objects (Car)")]
    public GameObject checkKeyHintObj;    // ลาก "Check Key" มาใส่
    public GameObject checkingKeyStatusObj; // ลาก "Checking key..." มาใส่
    public GameObject wrongCarObj;       // ลาก "THIS IS NOT THE RIGHT CAR..." มาใส่
    public GameObject winObj;            // ลาก "CORRECT KEY! YOU WIN!" มาใส่

    [Header("UI General")]
    public GameObject keyIcon;            // ลาก "KEY" (รูปมุมจอ) มาใส่
    public Image radialBar;               // ลาก "RadialBar" มาใส่
    
    private Blob currentBlob;
    private Car currentCar; 
    private bool isInteracting = false;
    private bool hasFoundKey = false;
    private bool isGameFinished = false; // ตัวแปรสำหรับหยุดระบบเมื่อชนะ

    void Start()
    {
        HideAllUI(); // ปิด UI ทุกอย่างตอนเริ่มเกม
    }

    void Update()
    {
        // 1. ถ้าเจอรถที่ถูกแล้ว (isGameFinished = true) จะหยุดการทำงานทั้งหมดทันที
        if (isGameFinished) return;

        CheckRaycast();

        // 2. ถ้าเจอ Blob (และยังไม่มีกุญแจ)
        if (currentBlob != null && !currentBlob.alreadySearched && !hasFoundKey)
        {
            HandleBlobInteraction();
        }
        // 3. ถ้าเจอ Car (และมีกุญแจแล้ว)
        else if (currentCar != null && !currentCar.alreadyChecked && hasFoundKey)
        {
            HandleCarInteraction();
        }
        else
        {
            ResetInteraction();
        }
    }

    void CheckRaycast()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // เช็ค Blob
            currentBlob = hit.collider.GetComponent<Blob>();
            if (currentBlob != null && !currentBlob.alreadySearched && !hasFoundKey)
            {
                if (!isInteracting) searchHintObj.SetActive(true);
                return;
            }

            // เช็ค Car
            currentCar = hit.collider.GetComponent<Car>();
            if (currentCar != null && !currentCar.alreadyChecked && hasFoundKey)
            {
                if (!isInteracting) checkKeyHintObj.SetActive(true);
                return;
            }
        }

        currentBlob = null;
        currentCar = null;
        if (!isInteracting)
        {
            searchHintObj.SetActive(false);
            checkKeyHintObj.SetActive(false);
        }
    }

    // --- ส่วนของ Blob ---
    void HandleBlobInteraction()
    {
        if (Input.GetKey(KeyCode.F))
        {
            isInteracting = true;
            searchHintObj.SetActive(false);
            searchingStatusObj.SetActive(true);
            UpdateRadialBar();

            if (timer >= searchTime) FinishBlobSearch();
        }
        else ResetInteraction();
    }

    void FinishBlobSearch()
    {
        currentBlob.alreadySearched = true;
        searchingStatusObj.SetActive(false);
        
        if (currentBlob.hasKey)
        {
            foundKeyObj.SetActive(true);
            keyIcon.SetActive(true);
            hasFoundKey = true;
        }
        else nothingFoundObj.SetActive(true);

        CompleteInteraction();
    }

    // --- ส่วนของ Car ---
    void HandleCarInteraction()
    {
        if (Input.GetKey(KeyCode.F))
        {
            isInteracting = true;
            checkKeyHintObj.SetActive(false);
            checkingKeyStatusObj.SetActive(true);
            UpdateRadialBar();

            if (timer >= searchTime) FinishCarCheck();
        }
        else ResetInteraction();
    }

    void FinishCarCheck()
    {
        currentCar.alreadyChecked = true;
        checkingKeyStatusObj.SetActive(false);

        if (currentCar.isCorrectCar)
        {
            winObj.SetActive(true);
            isGameFinished = true; // ล็อคระบบไม่ให้เช็คคันอื่นได้อีก
            
            radialBar.gameObject.SetActive(false);
            checkKeyHintObj.SetActive(false);

            // สั่งให้ข้อความชนะหายไปหลังจาก 5 วินาที
            Invoke("HideWinMessage", 5f); 
        }
        else
        {
            wrongCarObj.SetActive(true);
            CompleteInteraction();
        }
    }

    // --- Helper Functions ---
    void UpdateRadialBar()
    {
        radialBar.gameObject.SetActive(true);
        timer += Time.deltaTime;
        radialBar.fillAmount = timer / searchTime;
    }

    void CompleteInteraction()
    {
        isInteracting = false;
        radialBar.gameObject.SetActive(false);
        timer = 0;
        Invoke("HideStatusMessages", 2f);
    }

    void ResetInteraction()
    {
        isInteracting = false;
        timer = 0;
        radialBar.fillAmount = 0;
        radialBar.gameObject.SetActive(false);
        searchingStatusObj.SetActive(false);
        checkingKeyStatusObj.SetActive(false);
    }

    void HideStatusMessages()
    {
        nothingFoundObj.SetActive(false);
        foundKeyObj.SetActive(false);
        wrongCarObj.SetActive(false);
    }

    // ฟังก์ชันใหม่สำหรับปิดข้อความชนะโดยเฉพาะ
    void HideWinMessage()
    {
        if (winObj != null) winObj.SetActive(false);
    }

    void HideAllUI()
    {
        searchHintObj.SetActive(false);
        searchingStatusObj.SetActive(false);
        nothingFoundObj.SetActive(false);
        foundKeyObj.SetActive(false);
        checkKeyHintObj.SetActive(false);
        checkingKeyStatusObj.SetActive(false);
        wrongCarObj.SetActive(false);
        winObj.SetActive(false);
        keyIcon.SetActive(false);
        radialBar.gameObject.SetActive(false);
    }
}