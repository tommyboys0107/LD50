using System;
using System.Collections;
using System.Collections.Generic;
using CliffLeeCL;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] List<GameObject> openObjOnGameOver = new List<GameObject>();
    [SerializeField] List<GameObject> closeObjOnGameOver = new List<GameObject>();
    
    void Start()
    {
        EventManager.Instance.onGameOver += OnGameOver;
    }

    void OnDestroy()
    {
        EventManager.Instance.onGameOver -= OnGameOver; 
    }

    void OnGameOver()
    {
        foreach (var obj in openObjOnGameOver)
            obj.SetActive(true);
        foreach (var obj in closeObjOnGameOver)
            obj.SetActive(false);
    }
}
