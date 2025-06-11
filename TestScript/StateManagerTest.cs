using System;
using System.Collections;
using System.Collections.Generic;
using TestScript;
using UnityEngine;

public class StateManagerTest : MonoBehaviour
{
    
    private void Start()
    {
        int round = 3;
        int currentRound = 43;

// Tính toán số lần vượt quá mốc 20
        int numThresholdsExceeded = (currentRound - 1) / 20; // Trừ 1 để đảm bảo round 20 vẫn thuộc nhóm đầu tiên

// Tính toán round mới
        round += (numThresholdsExceeded * 20);

        Debug.Log("Round mới: " + round); 
    }
}
