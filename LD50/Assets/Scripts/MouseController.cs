using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CliffLeeCL;
using Unity.Mathematics;

public class MouseController : MonoBehaviour
{
    public bool canFollowMouse = true;
    public float speedMultiplier = 60.0f;

    Rigidbody rigid = null;
    Camera mainCamera = null;
    Vector3 worldMousePosition = Vector3.zero;
    bool isGameOver = false;

    void OnGameOver()
    {
        isGameOver = true;
        rigid.velocity = Vector3.zero;
    }

    void Start()
    {
        mainCamera = Camera.main;
        rigid = GetComponent<Rigidbody>();

        EventManager.Instance.onGameOver += OnGameOver;
    }

    void OnDisable()
    {
        EventManager.Instance.onGameOver -= OnGameOver;
    }

    void Update()
    {
        if (isGameOver)
            return;

        worldMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldMousePosition.y = 0.0f;
    }

    void FixedUpdate()
    {
        if (isGameOver)
            return;

        if (canFollowMouse)
        {
            FollowMouseWithForce();
        }
    }

    void FollowMouseWithForce()
    {
        Vector3 selfPositionToMousePosition = worldMousePosition - transform.position;
        Vector3 lookDirection = selfPositionToMousePosition.normalized;

        rigid.MoveRotation(quaternion.LookRotationSafe(lookDirection, Vector3.up));
        rigid.AddForce(selfPositionToMousePosition * speedMultiplier * Time.fixedDeltaTime, ForceMode.Force);
    }
}
