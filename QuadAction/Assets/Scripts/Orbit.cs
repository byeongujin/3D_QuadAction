using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float orbitSpeed;
    Vector3 offset;

    void Start()
    {
        offset = transform.position - target.position;
    }
    void Update()
    {
        transform.position = target.position + offset;
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
        offset = transform.position - target.position;
    }
}
