using System;
using System.Collections;
using System.Collections.Generic;
using CliffLeeCL;
using Sirenix.OdinInspector;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] Light light;
    [SerializeField] float changeLayerRadiusMultiplier = 1.0f;
    [SerializeField] float pullForceMultiplier = 1.0f;
    [SerializeField] float radiusGrowSpeed = 0.1f;
    [SerializeField] float lightIntensityGrowSpeed = 1.0f;
    [SerializeField] float lightRangeGrowSpeed = 0.5f;

    [ShowInInspector, ReadOnly] float blackHoleRadius;

    void Start()
    {
        InitBlackHole();
    }

    void OnValidate()
    {
        InitBlackHole();
    }

    void Update()
    {
        light.intensity += lightIntensityGrowSpeed * Time.deltaTime;
        light.range += lightRangeGrowSpeed * Time.deltaTime;
        blackHoleRadius += radiusGrowSpeed * Time.deltaTime;
        transform.localScale = Vector3.one * blackHoleRadius * 2.0f;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.attachedRigidbody == null)
            return;

        var forceVec = new Vector3(transform.position.x - col.transform.position.x, 0.0f,
            transform.position.z - col.transform.position.z);
        col.attachedRigidbody.AddForce(forceVec.normalized * pullForceMultiplier * blackHoleRadius * Time.deltaTime, ForceMode.Force);
        
        if (Vector3.Distance(transform.position, col.transform.position) < blackHoleRadius * changeLayerRadiusMultiplier)
        {
            col.gameObject.layer = LayerMask.NameToLayer("BlackHole");
        }
    }

    void InitBlackHole()
    {
        blackHoleRadius = transform.localScale.x / 2.0f;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, blackHoleRadius * changeLayerRadiusMultiplier);
    }
}