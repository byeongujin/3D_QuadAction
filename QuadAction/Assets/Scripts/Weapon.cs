using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bulletCase;
    [SerializeField] Transform bulletPos;
    [SerializeField] Transform bulletCasePos;
    [SerializeField] BoxCollider meleeArea;
    [SerializeField] TrailRenderer trailEffect;
    [SerializeField] AudioSource swingSound;
    [SerializeField] AudioSource shotSound;

    public enum Type { Melee, Range};
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        swingSound.Play();
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }
    IEnumerator Shot()
    {
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;
        shotSound.Play();
        yield return null;
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2,3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
}
