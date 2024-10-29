using System.Collections;
using UnityEngine;
using com.mani.muzamil.amjad;
using DG.Tweening;
using System.Collections.Generic;
public class TableDealer : MonoBehaviour
{
    [SerializeField] Rm RefMgr;
    [SerializeField] RectTransform playerCardPos;
    [SerializeField] RectTransform dealerCardPos;

    [ShowOnly]
    public List<CardProperty> playerCards;
    [ShowOnly]
    public List<CardProperty> dealerCards;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void FirstTimeDealCards()
    {
        StartCoroutine(FirstTimeDealCardsCrt());
    }

    IEnumerator FirstTimeDealCardsCrt()
    {
        Debug.LogError("distributing card  2");
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 4; i++)
        {
            int playerOrDealer = i % 2;
            RectTransform cardPos = playerOrDealer == 0 ? playerCardPos : dealerCardPos;
            CardProperty card = Get1Card(cardPos);
            int totalCards = playerOrDealer == 0 ? playerCards.Count : dealerCards.Count;
            bool isFlip = i == 3 ? false : true;
            playCardAnimation(card.gameObject, cardPos.gameObject, totalCards, isFlip, playerOrDealer == 0 ? true : false);
            if (playerOrDealer == 0)
                playerCards.Add(card);
            else
                dealerCards.Add(card);
            yield return new WaitForSeconds(0.6f);
        }
    }

    CardProperty Get1Card(RectTransform rect)
    {
        int card = Random.Range(0, RefMgr.cardsManager.cardsStackList.Count);
        CardProperty cp = RefMgr.cardsManager.cardsStackList[card];
        RefMgr.cardsManager.cardsStackList.Remove(cp);
        return cp;
    }
    void playCardAnimation(GameObject ObjectToAnimate, GameObject targetObj, int offSet, bool shouldFLip, bool isPlayer)
    {
        GameObject card = ObjectToAnimate;
        GameObject TgtObj = targetObj;
        card.transform.parent = TgtObj.transform.parent.transform;

        Vector3 tgtPos = TgtObj.transform.position + Vector3.right * offSet * 50;
        ObjectToAnimate.transform.DOMove(tgtPos, 0.25f)
            .OnComplete(() => FlipCard(card, shouldFLip, isPlayer));

        ObjectToAnimate.transform.DORotateQuaternion(targetObj.transform.rotation, 0.25f);

    }

    void FlipCard(GameObject obj, bool shouldFLip, bool isPlayer)
    {
        if (!shouldFLip)
        {
            MoveSlightlyUp(obj);
        }
        else
        {
            obj.transform.DORotate(Vector2.up * 90, 0.25f)
                .OnComplete(() => ReverseRotate(obj, isPlayer));
        }
    }

    void ReverseRotate(GameObject obj, bool isPlayer)
    {
        obj.GetComponent<CardProperty>().ShowOriginalSprite();
        obj.transform.DORotate(Vector2.zero, 0.25f);
        RefMgr.scoreManager.SetScores(obj.GetComponent<CardProperty>(), isPlayer);
    }

    void MoveSlightlyUp(GameObject obj)
    {
        Vector3 targetPosition = obj.transform.position + Vector3.up * 100;
        obj.transform.DOMove(targetPosition, 0.25f).SetLoops(2, LoopType.Yoyo);
    }

}
