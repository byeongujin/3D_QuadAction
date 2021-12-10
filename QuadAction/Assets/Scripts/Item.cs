using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Rigidbody rigid;
    SphereCollider sphereCollider;
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    public Type type;
    public int value;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }
    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
