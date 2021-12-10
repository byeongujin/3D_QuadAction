using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    [SerializeField] GameObject missile;
    [SerializeField] Transform missilePortA;
    [SerializeField] Transform missilePortB;
    [SerializeField] bool isLook;
    [SerializeField] AudioSource missileSound;
    [SerializeField] AudioSource jumpSound;
    [SerializeField] AudioSource tauntSound;
    [SerializeField] AudioSource rockSound;
    Vector3 lookVec;
    Vector3 tauntVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        nav.isStopped = true;
        StartCoroutine(Think());
    }
    void Update()
    {
        if(isDead)
        {
            StopAllCoroutines();
            return;
        }
        if(isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        else
        {
            nav.SetDestination(tauntVec);
        }
    }
    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);
        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(Taunt());
                break;
        }
    }
    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        missileSound.Play();
        bossMissileA.target = target;
        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        missileSound.Play();
        bossMissileB.target = target;
        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }
    IEnumerator RockShot()
    {
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        rockSound.Play();
        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine(Think());
    }
    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;
        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");
        jumpSound.Play();
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;
        tauntSound.Play();
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        StartCoroutine(Think());
    }
}
