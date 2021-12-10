using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;

    void Update()
    {
        transform.position = target.position + offset;
    }
}
