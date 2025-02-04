using com.muzamil;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    [SerializeField] int cardsLimit;
    [SerializeField] Rm refMgr;
    public CardsContainer allCards;
    [ShowOnly]
    public List<int> cardsIndexes;
    [ShowOnly]
    public List<CardProperty> cardsStackList;
    public List<GameObject> wasteCardsList;
    [SerializeField] RectTransform cardsStackPos, wasteCardsPos;

    void Start()
    {
        CardsCreation();
    }

    void CardsCreation()
    {
        ClearCardsStack();
        CreateCardsIndexes();
    }
    void CreateCardsIndexes()
    {
        ClearIntList();
        for (int j = 0; j < 5; j++)
            for (int i = 0; i < allCards.Card.Length; i++)
                cardsIndexes.Add(i);
        CreateLimitedCardsForStack();
    }
    public void ReCreateLimitedCards()
    {
        if (cardsStackList.Count < (cardsLimit - 8))
            CreateLimitedCardsForStack();
    }
    void CreateLimitedCardsForStack()
    {
        if (cardsStackList.Count < cardsLimit)
        {
            int cardIndex = GetRandomCard();
            GameObject card = Instantiate(allCards.Card[GetRandomCard()].gameObject);
            LocalSetting.SetPosAndRect(card, cardsStackPos, cardsStackPos.transform.parent);
            cardsStackList.Add(card.GetComponent<CardProperty>());
            card.GetComponent<CardProperty>().ShowDummySkin();
            CreateLimitedCardsForStack();
        }
        else
        {
            RearrangeCardsStack();
            return;
        }
    }

    public void RearrangeCardsStack()
    {
        for (int i = 0; i < cardsStackList.Count; i++)
        {
            Vector2 cardPos = cardsStackPos.transform.position + (Vector3.up * (-i * 10));
            cardsStackList[i].transform.position = cardPos;
        }
    }
    int GetRandomCard()
    {
        int cardIndex = Random.Range(0, cardsIndexes.Count);
        int val = cardsIndexes[cardIndex];
        cardsIndexes.RemoveAt(cardIndex);
        return val;
    }


    void ClearIntList()
    {
        if (cardsIndexes == null)
        {
            cardsIndexes = new List<int>();
            return;
        }
        cardsIndexes.Clear();
    }
    void ClearCardsStack()
    {
        if (cardsStackList == null)
        {
            cardsStackList = new List<CardProperty>();
            return;
        }
        if (cardsStackList.Count > 0)
        {
            foreach (CardProperty card in cardsStackList)
                Destroy(card.gameObject);
        }
        cardsStackList.Clear();
    }


    public void SendCardsToWasteCardsPos()
    {
        StartCoroutine(CollectWasteCards());
    }

    IEnumerator CollectWasteCards()
    {
        yield return new WaitForSeconds(0.1f);
        refMgr.scoreManager.ShowScoreObjects(false);
        foreach (CardProperty card in refMgr.tableDealer.playerCards)
        {
            wasteCardsList.Add(card.gameObject);
            playCardAnimation(card.gameObject, wasteCardsPos.gameObject);
            yield return new WaitForSeconds(0.1f);
        }
        foreach (CardProperty card in refMgr.tableDealer.playerCards_1_Split)
        {
            wasteCardsList.Add(card.gameObject);
            playCardAnimation(card.gameObject, wasteCardsPos.gameObject);
            yield return new WaitForSeconds(0.1f);
        }
        foreach (CardProperty card in refMgr.tableDealer.playerCards_2_Split)
        {
            wasteCardsList.Add(card.gameObject);
            playCardAnimation(card.gameObject, wasteCardsPos.gameObject);
            yield return new WaitForSeconds(0.1f);
        }
        foreach (CardProperty card in refMgr.tableDealer.dealerCards)
        {
            wasteCardsList.Add(card.gameObject);
            playCardAnimation(card.gameObject, wasteCardsPos.gameObject);
            yield return new WaitForSeconds(0.1f);
        }
        refMgr.tableDealer.playerCards.Clear();
        refMgr.tableDealer.playerCards_1_Split.Clear();
        refMgr.tableDealer.playerCards_2_Split.Clear();
        refMgr.tableDealer.dealerCards.Clear();
        yield return new WaitForSeconds(1);
        ClearCards();
    }

    void ClearCards()
    {
        for (int i = wasteCardsList.Count - 1; i > 0; i--)
        {
            GameObject obj = wasteCardsList[i];
            wasteCardsList.RemoveAt(i);
            Destroy(obj);
        }
    }
    void playCardAnimation(GameObject ObjectToAnimate, GameObject targetObj)
    {
        GameObject card = ObjectToAnimate;
        GameObject TgtObj = targetObj;
        card.transform.SetParent(TgtObj.transform.parent.transform);
        ObjectToAnimate.transform.DOMove(targetObj.transform.position, 0.2f)
            .OnComplete(() => OnCompleteShowDummyCard(card));
        ObjectToAnimate.transform.DORotateQuaternion(targetObj.transform.rotation, 0.2f);
    }

    void OnCompleteShowDummyCard(GameObject obj)
    {
        obj.GetComponent<CardProperty>().ShowDummySkin();
    }
}