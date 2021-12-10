using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] GameObject menuCam;
    [SerializeField] GameObject gameCam;
    [SerializeField] GameObject itemShop;
    [SerializeField] GameObject weaponShop;
    [SerializeField] GameObject startZone;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject overPanel;
    [SerializeField] Transform[] enemyZones;
    [SerializeField] GameObject[] enemies;
    [SerializeField] List<int> enemyList;
    [SerializeField] RectTransform bossHealthGroup;
    [SerializeField] RectTransform bossHealthBar;
    [SerializeField] AudioSource stageClearSound;
    [SerializeField] AudioSource townMusic;
    [SerializeField] AudioSource battleMusic;
    [SerializeField] Player player;
    [SerializeField] Boss boss;
    [SerializeField] Text maxScoreTxt;
    [SerializeField] Text scoreTxt;
    [SerializeField] Text stageTxt;
    [SerializeField] Text playTimeTxt;
    [SerializeField] Text playerHealthTxt;
    [SerializeField] Text playerAmmoTxt;
    [SerializeField] Text playerCoinTxt;
    [SerializeField] Text enemyATxt;
    [SerializeField] Text enemyBTxt;
    [SerializeField] Text enemyCTxt;
    [SerializeField] Text curScoreText;
    [SerializeField] Text bestText;
    [SerializeField] Image weapon1Img;
    [SerializeField] Image weapon2Img;
    [SerializeField] Image weapon3Img;
    [SerializeField] Image weaponRImg;
    [SerializeField] bool isBattle;
    [SerializeField] float playTime;
    [SerializeField] int stage;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    void Awake()
    {
        enemyList = new List<int>();
        maxScoreTxt.text = string.Format("{0:n0}",PlayerPrefs.GetInt("MaxScore"));
        if(PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }
        boss = null;
    }
    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        player.gameObject.SetActive(true);
        townMusic.Play();
    }
    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);
        isBattle = true;
        townMusic.Stop();
        battleMusic.Play();
        StartCoroutine(InBattle());
    }
    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoreTxt.text;
        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if (player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
        battleMusic.Stop();
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f;
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);
        isBattle = false;
        townMusic.Play();
        
        stage++;
    }
    IEnumerator InBattle()
    {
        if(stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            for(int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);
                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }
            while(enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.manager = this;
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(4);
            }
        }
        while(enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }
        stageClearSound.Play();
        battleMusic.Stop();
        GameObject[] enemybullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject enemybullet in enemybullets)
        {
            Destroy(enemybullet);
        }
        yield return new WaitForSeconds(5f);
        boss = null;
        StageEnd();
    }
    void Update()
    {
        if(isBattle)
        {
            playTime += Time.deltaTime;
        }
    }
    void LateUpdate()
    {
        scoreTxt.text = string.Format("{0:n0}", player.score);
        stageTxt.text = "Stage " + stage;
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);


        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
        playerHealthTxt.text = player.health + " / " + player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        if(player.equipWeapon == null)
        {
            playerAmmoTxt.text = "- / " + player.ammo;
        }
        else if(player.equipWeapon.type == Weapon.Type.Melee)
        {
            playerAmmoTxt.text = "- / " + player.ammo;
        }
        else
        {
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;
        }
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();

        if(boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
    }
}
