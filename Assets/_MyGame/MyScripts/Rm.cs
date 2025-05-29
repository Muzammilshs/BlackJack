using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resource manager and central reference hub for all main game systems in the blackjack game.
/// Provides singleton access, hand order management, and utility methods for hand/card data.
/// </summary>
public class Rm : MonoBehaviour
{
    #region Game System References

    public GameManager gameManager;               // Reference to the main game manager
    public GameStateManager gameStateManager;     // Reference to the game state manager
    public BetBarHandler betBarHandler;           // Reference to the bet bar handler
    public PotHandler potHandler;                 // Reference to the pot handler
    public CardsManager cardsManager;             // Reference to the cards manager
    public TableDealer tableDealer;               // Reference to the table dealer
    //public ScoreManager scoreManager;             // Reference to the score manager
    public HitStandBarHandler hitStandBarHandler; // Reference to the hit/stand bar handler
    public DealerAIPlay dealerAIPlay;             // Reference to the dealer AI logic

    #endregion

    #region Singleton Pattern

    private static Rm _instance;

    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static Rm Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindAnyObjectByType<Rm>();
            return _instance;
        }
    }

    /// <summary>
    /// Ensures only one instance exists in the scene.
    /// </summary>
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    #endregion

    #region Hand Order and Card Data

    /// <summary>
    /// The order in which hands are processed (player, splits, dealer, etc.).
    /// </summary>
    public HandType.HANDTYPE[] handsOrderEnum;

    /// <summary>
    /// The currently active hand's CardsData (static for global access).
    /// </summary>
    public static CardsData currentCardData;

    /// <summary>
    /// All CardsData objects in the game (player, splits, dealer, etc.).
    /// </summary>
    [SerializeField] public List<CardsData> cardsData = new List<CardsData>();

    #endregion

    #region Hand Navigation and Utility

    /// <summary>
    /// Returns the next valid (active) hand after the currentCardData, or null if none.
    /// Used for progressing through player/split hands.
    /// </summary>
    public CardsData GetValidHandCardData()
    {
        HandType.HANDTYPE currentHandType = currentCardData.handType;
        int currentIndex = System.Array.IndexOf(handsOrderEnum, currentHandType);
        if (currentIndex == handsOrderEnum.Length - 1)
        {
            Debug.LogError("No next valid hand found, returning null.");
            return null;
        }
        CardsData cd = null;
        for (int i = currentIndex + 1; i < handsOrderEnum.Length; i++)
        {
            for (int j = 0; j < cardsData.Count; j++)
            {
                if (cardsData[j].handType == handsOrderEnum[i])
                {
                    if (cardsData[j].isValidHand)
                    {
                        cd = cardsData[j];
                        break;
                    }
                }
            }
            if (cd != null)
                break;
        }
        return cd;
    }

    #endregion

    #region Hand Results Navigation

    [HideInInspector] public int handIndexForResults = 0;      // Index for iterating through hands for results
    [HideInInspector] public CardsData cardsDataForResult;     // The current CardsData being processed for results

    /// <summary>
    /// Returns the next valid (active) player hand for result processing, skipping the dealer.
    /// Updates handIndexForResults and cardsDataForResult.
    /// </summary>
    public CardsData GetValidHandForResults()
    {
        HandType.HANDTYPE currentHandType = handsOrderEnum[handIndexForResults];

        if (handIndexForResults == handsOrderEnum.Length - 1)
        {
            Debug.LogError("No next valid hand found, returning null.");
            return null;
        }
        CardsData cd = null;
        cardsDataForResult = null;
        for (int i = handIndexForResults; i < handsOrderEnum.Length; i++)
        {
            for (int j = 0; j < cardsData.Count; j++)
            {
                if (cardsData[j].handType == handsOrderEnum[i])
                {
                    if (cardsData[j].handType != HandType.HANDTYPE.DEALERHAND)
                    {
                        if (cardsData[j].isValidHand)
                        {
                            cd = cardsData[j];
                            handIndexForResults = i;
                            cardsDataForResult = cd;
                            handIndexForResults++;
                            break;
                        }
                    }
                }
            }
            if (cd != null)
                break;
        }
        return cd;
    }

    #endregion

    #region Card Data Lookup

    /// <summary>
    /// Returns the CardsData object for the requested hand type, or null if not found.
    /// </summary>
    public CardsData GetCardData(HandType.HANDTYPE reqHandType)
    {
        foreach (CardsData card in cardsData)
        {
            if (card.handType == reqHandType)
                return card;
        }
        return null;
    }

    #endregion
}
