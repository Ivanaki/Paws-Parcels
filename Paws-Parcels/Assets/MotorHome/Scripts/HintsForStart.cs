using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HintsForStart : MonoBehaviour
{
    public static HintsForStart Instance;
    public List<GameObject> hints;
    public int nowHintNum = 0;
    private void Awake()
    {
        Instance = this;
        foreach (GameObject hint in hints)
        {
            hint.SetActive(false);
        }
    }
    
    public void NewHint()
    {    
        hints[nowHintNum].SetActive(false);
        if (nowHintNum < hints.Count - 1)
        {
            nowHintNum += 1;
        }
    }

    public void ShowHint()
    {
        hints[nowHintNum].SetActive(true);
        print("SHOWONHINT");
    }
    
    public void ExitHint()
    {
        hints[nowHintNum].SetActive(false);
    }
}
