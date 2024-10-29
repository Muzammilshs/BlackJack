using com.mani.muzamil.amjad;
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
    public List<CardProperty> cardsStack;
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
    void CreateLimitedCardsForStack()
    {
        if (cardsStack.Count < cardsLimit)
        {
            int cardIndex = GetRandomCard();
            GameObject card = Instantiate(allCards.Card[GetRandomCard()].gameObject);
            LocalSetting.SetPosAndRect(card, cardsStackPos, cardsStackPos.transform.parent);
            Vector2 cardPos = card.transform.position + (Vector3.up * (-cardsStack.Count * 15));
            card.transform.position = cardPos;
            cardsStack.Add(card.GetComponent<CardProperty>());
            card.GetComponent<CardProperty>().ShowDummySkin();
            CreateLimitedCardsForStack();
        }
        else
            return;
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
        if (cardsStack == null)
        {
            cardsStack = new List<CardProperty>();
            return;
        }
        if (cardsStack.Count > 0)
        {
            foreach (CardProperty card in cardsStack)
                Destroy(card.gameObject);
        }
        cardsStack.Clear();
    }
}