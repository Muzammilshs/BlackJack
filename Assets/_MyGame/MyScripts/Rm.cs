using System.Collections.Generic;
using UnityEngine;

public class Rm : MonoBehaviour
{
    public GameManager gameManager;
    public GameStateManager gameStateManager;
    public BetBarHandler betBarHandler;
    public PotHandler potHandler;
    public CardsManager cardsManager;
    public TableDealer tableDealer;
    public ScoreManager scoreManager;
    public HitStandBarHandler hitStandBarHandler;
    public DealerAIPlay dealerAIPlay;


    #region Creating Instance
    private static Rm _instance;
    public static Rm Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindAnyObjectByType<Rm>();
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    #endregion

    public HandType.HANDTYPE[] handsOrderEnum;
    public static CardsData currentCardData;

    #region Get Valid Hand Card Data
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
                        HandType.HANDTYPE currentHandType2 = cardsData[j].handType;
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

    #region Get Valid Hand For Results
    [HideInInspector] public int handIndexForResults = 0;
    [HideInInspector] public CardsData cardsDataForResult;
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
                            HandType.HANDTYPE currentHandType2 = cardsData[j].handType;
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

    #region Get card data by hand type
    [SerializeField] public List<CardsData> cardsData = new List<CardsData>();
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
