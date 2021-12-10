using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] AudioSource jumpSound;
    [SerializeField] AudioSource damageSound;
    [SerializeField] AudioSource gameOverSound;
    [SerializeField] AudioSource coinSound;
    [SerializeField] AudioSource dodgeSound;
    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    Rigidbody rigid;
    GameObject nearObject;
    MeshRenderer[] meshs;
    public Weapon equipWeapon;
    public GameObject[] weapons;
    public GameObject[] grenades;
    public GameObject grenadeObj;
    public GameManager manager;
    public Camera followCamera;
    public bool[] hasWeapons;
    public int ammo;
    public int coin;
    public int health;
    public int score;
    public int hasGrenades;
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;
    int equipWeaponIndex = -1;

    [SerializeField] float speed;
    [SerializeField] float jumpForce = 15.0f;
    float hAxis;
    float vAxis;
    float fireDelay;
    bool wDown;
    bool jDown;
    bool fDown;
    bool gDown;
    bool rDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady = true;
    bool isReload;
    bool isBorder;
    bool isDamage;
    bool isShop;
    bool isDead;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
    }
    void Update()
    {
        GetInput();
        Movement();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interaction();
    }
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }
    void Movement()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        if(isDodge)
            moveVec = dodgeVec;
        if(isSwap || !isFireReady || isReload || isDead)
            moveVec = Vector3.zero;
        if (!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        anim.SetBool("IsRun", moveVec != Vector3.zero);
        anim.SetBool("IsWalk", wDown);
    }
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
        if(fDown && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }
    void Jump()
    {
        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isDead)
        {
            rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
            jumpSound.Play();
        }
    }
    void Grenade()
    {
        if (hasGrenades == 0)
            return;
        if (gDown && !isReload && !isSwap && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);
                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }
    void Attack()
    {
        if(equipWeapon == null)
            return;
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;
        if(fDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead && !isJump)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }
    void Reload()
    {
        if (equipWeapon == null)
            return;
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        if (ammo == 0)
            return;
        if (equipWeapon.curAmmo == equipWeapon.maxAmmo)
            return;
        if (rDown && !isJump && !isSwap && !isDodge && isFireReady && !isShop && !isDead)
        {
            anim.SetTrigger("doReload");
            isReload = true;
            Invoke("ReloadExit", 3f);
        }
    }
    void ReloadExit()
    {
        int reAmmo = equipWeapon.maxAmmo - equipWeapon.curAmmo;
        if (reAmmo > ammo)
        {
            equipWeapon.curAmmo += ammo;
            ammo = 0;
        }
        else
        {
            ammo -= reAmmo;
            equipWeapon.curAmmo += reAmmo;
        }
        isReload = false;
    }
    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isShop && !isDead)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            dodgeSound.Play();
            isDodge = true;
            Invoke("DodgeExit", 0.5f); 
        }
    }
    void DodgeExit()
    {
        speed *= 0.5f;
        isDodge = false;
    }
    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;
        int weaponIndex = -1;
        if (sDown1)
            weaponIndex = 0;
        if (sDown2)
            weaponIndex = 1;
        if (sDown3)
            weaponIndex = 2;
        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isShop && !isDead)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapExit", 0.4f);
        }
    }
    void SwapExit()
    {
        isSwap = false;
    }
    void Interaction()
    {
        if (iDown && nearObject != null && !isDodge && !isJump && !isShop && !isDead)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;
                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    coinSound.Play();
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet")
        {
            if(!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;
                bool isBossAtk = other.name == "Boss Melee Area";
                if (other.GetComponent<Rigidbody>() != null)
                    Destroy(other.gameObject);
                StartCoroutine(OnDamage(isBossAtk));
            }
        }
    }
    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
            damageSound.Play();
        }
        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        if (health <= 0 && !isDead)
            OnDie();
        yield return new WaitForSeconds(0.1f);
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
        if (isBossAtk)
            rigid.velocity = Vector3.zero;
    }
    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        gameOverSound.Play();
        manager.GameOver();
    }
    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = other.gameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
        else if (other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }
    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }
}