using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] GameObject[] itemObj;
    [SerializeField] Transform[] itemPos;
    [SerializeField] RectTransform uiGroup;
    [SerializeField] Animator anim;
    [SerializeField] Text talkText;
    [SerializeField] int[] itemPrice;
    [SerializeField] string[] talkData;
    [SerializeField] AudioSource sound;
    Player enterPlayer;

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }
    public void Buy(int index)
    {
        int price = itemPrice[index];
        if (price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }
        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-1, 1) + Vector3.forward * Random.Range(-1, 1);
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
        sound.Play();
    }
    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
