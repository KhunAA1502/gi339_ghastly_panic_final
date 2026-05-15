using UnityEngine;
using UnityEngine.UI;

public class Blob : MonoBehaviour
{
    public bool hasKey = false;
    public float searchTime = 3f;
    private float currentSearchTime = 0f;
    private bool isSearching = false;
    private bool isOpened = false;

    [Header("UI Elements")]
    public Image radialBarImage; // ลาก Image ที่เป็น Filled มาใส่ตรงนี้
    public GameObject uiCanvas;   // ลาก Canvas หรือ Group ของ UI มาใส่เพื่อ เปิด/ปิด

    void Start()
    {
        if (uiCanvas) uiCanvas.SetActive(false); // ปิดไว้ก่อนเริ่ม
    }

    public void StartSearching()
    {
        if (isOpened) return;
        isSearching = true;
        if (uiCanvas) uiCanvas.SetActive(true);
    }

    public void StopSearching()
    {
        isSearching = false;
        currentSearchTime = 0f;
        if (uiCanvas) uiCanvas.SetActive(false);
        if (radialBarImage) radialBarImage.fillAmount = 0;
    }

    void Update()
    {
        if (isSearching && !isOpened)
        {
            currentSearchTime += Time.deltaTime;
            
            // คำนวณค่า 0.0 - 1.0 เพื่อใส่ใน Fill Amount
            if (radialBarImage) 
                radialBarImage.fillAmount = currentSearchTime / searchTime;

            if (currentSearchTime >= searchTime)
            {
                FinishSearch();
            }
        }
    }

    void FinishSearch()
    {
        isOpened = true;
        isSearching = false;
        if (uiCanvas) uiCanvas.SetActive(false);
        
        if (hasKey) { Debug.Log("FOUND KEY!"); }
        else { Debug.Log("EMPTY!"); }
    }
}