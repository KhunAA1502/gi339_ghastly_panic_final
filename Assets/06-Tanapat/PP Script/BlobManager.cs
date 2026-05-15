using UnityEngine;
using System.Collections.Generic;

public class BlobManager : MonoBehaviour
{
    public List<Blob> allBlobs = new List<Blob>();

    void Start()
    {
        if (allBlobs.Count > 0)
        {
            // สุ่มเลือก 1 อันให้มีกุญแจ
            int luckyIndex = Random.Range(0, allBlobs.Count);
            
            for (int i = 0; i < allBlobs.Count; i++)
            {
                allBlobs[i].hasKey = (i == luckyIndex);
            }
        }
    }
}