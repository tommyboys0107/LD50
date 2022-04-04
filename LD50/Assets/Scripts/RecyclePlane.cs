using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecyclePlane : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("BlackHole"))
            col.gameObject.SetActive(false);
    }
}