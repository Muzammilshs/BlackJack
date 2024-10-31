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
}
