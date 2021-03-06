using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CliffLeeCL;
using Unity.Mathematics;

public class MouseController : MonoBehaviour
{
    [SerializeField] GameObject tapHintObj;
    [SerializeField] bool canFollowMouse = true;
    [SerializeField] bool canRotation = true;
    [SerializeField] float speedMultiplier = 60.0f;
    [SerializeField] float maxDistanceSpeedMultiplier = 10.0f;
    [SerializeField] LayerMask rayLayer;

    Rigidbody rigid = null;
    Camera mainCamera = null;
    Vector3 worldMousePosition = Vector3.zero;
    bool isGameStart = false;
    bool isGameOver = false;

    void OnGameOver()
    {
        isGameOver = true;
    }

    void Start()
    {
        mainCamera = Camera.main;
        rigid = GetComponent<Rigidbody>();

        EventManager.Instance.onGameOver += OnGameOver;
    }

    void OnMouseUpAsButton()
    {
        if (isGameStart)
            return;

        isGameStart = true;
        tapHintObj.SetActive(false);
        GameManager.Instance.GameStart();
    }

    void OnDisable()
    {
        EventManager.Instance.onGameOver -= OnGameOver;
    }

    void Update()
    {
        if (!isGameStart || isGameOver || gameObject.layer == LayerMask.NameToLayer("BlackHole")) 
            return;

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out var hit, 100.0f,
                rayLayer, QueryTriggerInteraction.Ignore))
        {
            worldMousePosition = hit.point;
            worldMousePosition.y = 0.0f;
        }
        // worldMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        if (!isGameStart || isGameOver || gameObject.layer == LayerMask.NameToLayer("BlackHole"))
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
        
        lookDirection.y = 0;
        if (canRotation)
            rigid.MoveRotation(quaternion.LookRotationSafe(lookDirection, Vector3.up));
        rigid.AddForce(
            selfPositionToMousePosition.normalized *
            Mathf.Min(selfPositionToMousePosition.magnitude, maxDistanceSpeedMultiplier) * speedMultiplier *
            Time.fixedDeltaTime, ForceMode.Force);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(worldMousePosition, 1.0f);
    }
}