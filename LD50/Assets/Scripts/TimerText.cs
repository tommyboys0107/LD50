using System;
using System.Collections;
using System.Collections.Generic;
using CliffLeeCL;
using TMPro;
using UnityEngine;

public class TimerText : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    
    void Update()
    {
        timerText.text = TimeSpan.FromSeconds(GameManager.Instance.roundTimer.CurrentTime).ToString("mm\\:ss\\.ff");
    }
}
