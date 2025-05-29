using com.muzammil;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the deck(s) of cards, card instantiation, shuffling, and waste pile for the blackjack game.
/// Handles card stack creation, drawing, and recycling.
/// </summary>
public class CardsManager : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField] int totalDecks = 10;           // Number of decks to use in the game
    [SerializeField] int cardsLimit;                // Maximum number of cards in the stack at any time
    [SerializeField] Rm refMgr;                     // Reference to the main resource manager
    public CardsContainer allCards;                 // ScriptableObject containing all card prefabs and back designs

    #endregion

    #region Card Stacks and Positions

    [ShowOnly]
    public List<int> cardsIndexes;                  // List of available card indexes for drawing
    [ShowOnly]
    public List<CardProperty> cardsStackList;       // The current stack of card objects in play
    public List<GameObject> wasteCardsList;         // List of cards that have been discarded
    public RectTransform cardsStackPos;             // Position for the card stack in the UI
    public RectTransform wasteCardsPos;             // Position for the waste pile in the UI

    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the card manager, sets card design, and creates the initial card stack.
    /// </summary>
    void Start()
    {
        refMgr.gameManager.SetCardDesign();
        CardsCreation();
    }

    #endregion

    #region Card Stack Creation and Management

    /// <summary>
    /// Clears the card stack and creates a new set of card indexes.
    /// </summary>
    void CardsCreation()
    {
        ClearCardsStack();
        CreateCardsIndexes();
    }

    /// <summary>
    /// Fills the cardsIndexes list with all card indexes for all decks, then creates the stack.
    /// </summary>
    void CreateCardsIndexes()
    {
        ClearIntList();
        for (int j = 0; j < totalDecks; j++)
        {
            for (int i = 0; i < allCards.Card.Length; i++)
            {
                cardsIndexes.Add(i);
            }
        }
        CreateLimitedCardsForStack();
    }

    /// <summary>
    /// Replenishes the card stack and/or indexes if running low.
    /// </summary>
    public void ReCreateLimitedCards()
    {
        if (cardsIndexes.Count < 20)
        {
            CreateCardsIndexes();
        }
        if (cardsStackList.Count < (cardsLimit - 8))
        {
            CreateLimitedCardsForStack();
        }
    }

    /// <summary>
    /// Instantiates cards from the available indexes until the stack reaches the limit.
    /// </summary>
    void CreateLimitedCardsForStack()
    {
        while (cardsStackList.Count < cardsLimit)
        {
            int cardIndex = GetRandomCard();
            GameObject card = Instantiate(allCards.Card[cardIndex].gameObject);
            if (card == null)
            {
                Debug.LogError("Failed to instantiate card.");
                return;
            }
            LocalSettingBlackJack.SetPositionAndRectTransform(card, cardsStackPos, cardsStackPos.transform.parent);
            CardProperty cardProperty = card.GetComponent<CardProperty>();
            if (cardProperty == null)
            {
                Debug.LogError("CardProperty component missing on instantiated card.");
                return;
            }
            cardsStackList.Add(cardProperty);
            cardProperty.ShowDummySkin();
        }
        RearrangeCardsStack();
    }

    /// <summary>
    /// Rearranges the card stack visually in the UI.
    /// </summary>
    public void RearrangeCardsStack()
    {
        for (int i = 0; i < cardsStackList.Count; i++)
        {
            Vector2 cardPos = cardsStackPos.transform.position + (Vector3.up * (-i * 10));
            cardsStackList[i].transform.position = cardPos;
        }
    }

    #endregion

    #region Card Drawing and Utility

    /// <summary>
    /// Returns a random card index from the available indexes and removes it from the list.
    /// </summary>
    int GetRandomCard()
    {
        int cardIndex = Random.Range(0, cardsIndexes.Count);
        int val = cardsIndexes[cardIndex];
        cardsIndexes.RemoveAt(cardIndex);
        return val;
    }

    /// <summary>
    /// Clears the list of card indexes.
    /// </summary>
    void ClearIntList()
    {
        if (cardsIndexes == null)
        {
            cardsIndexes = new List<int>();
            return;
        }
        cardsIndexes.Clear();
    }

    /// <summary>
    /// Destroys all cards in the stack and clears the list.
    /// </summary>
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
            {
                Destroy(card.gameObject);
            }
        }
        cardsStackList.Clear();
    }

    #endregion

    #region Waste Pile Management

    /// <summary>
    /// Destroys all cards in the waste pile and clears the list.
    /// </summary>
    public void ClearWasteCards()
    {
        for (int i = wasteCardsList.Count - 1; i > 0; i--)
        {
            GameObject obj = wasteCardsList[i];
            wasteCardsList.RemoveAt(i);
            Destroy(obj);
        }
    }

    #endregion
}
