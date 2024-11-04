using UnityEngine;
using com.muzamil;

public class HitStandBarHandler : MonoBehaviour
{
    public Rm rm;
    public GameObject hitStandBar;
    public GameObject splitBtn;
    public GameObject doubleBtn;

    [ShowOnly]
    public bool isDoubleBet;

    private void Start()
    {
        hitStandBar.SetActive(false);
    }
    public void CompareScoresAfter4Cards()
    {
        TableDealer td = rm.tableDealer;
        ScoreManager sm = rm.scoreManager;

        int playerScores = 0;
        int dealrScores = 0;

        // getting player score
        if (td.playerCards[0].Card == CardState.CARDVALUE.ACE && td.playerCards[1].Card == CardState.CARDVALUE.ACE)
            playerScores = td.playerCards[0].Power + td.playerCards[1].SecondPower;
        else
            playerScores = td.playerCards[0].Power + td.playerCards[1].Power;

        // getting dealer scores
        if (td.dealerCards[0].Card == CardState.CARDVALUE.ACE && td.dealerCards[1].Card == CardState.CARDVALUE.ACE)
            dealrScores = td.dealerCards[0].Power + td.dealerCards[1].SecondPower;
        else
            dealrScores = td.dealerCards[0].Power + td.dealerCards[1].Power;

        // When win with first 2 cards
        if (playerScores == sm.targetScores || dealrScores == sm.targetScores)
        {
            //td.FlipCard(td.dealerCards[1].gameObject, true, false);
            ShowHitStandBar(false);
            if (playerScores > dealrScores)
            {
                rm.dealerAIPlay.isJackPot = true;
                rm.dealerAIPlay.isDealerTurn = true;
                rm.dealerAIPlay.DropDealerCard();
            }
            else if (playerScores < dealrScores)
            {
                rm.dealerAIPlay.isJackPot = false;
                rm.dealerAIPlay.DropDealerCard();

            }
            else
                rm.dealerAIPlay.DropDealerCard();
        }
        else
        {
            // when not won with first 2 cards
            ShowHitStandBar(true);
            if (CheckIfPlayerHaveEnoughChips)
                doubleBtn.SetActive(true);
            else
                doubleBtn.SetActive(false);
        }
    }
    public void ShowHitStandBar(bool isShow)
    {
        hitStandBar.SetActive(isShow);
    }

    public void OnStandBtnClick()
    {
        rm.gameStateManager.UpDateGameState(GameState.State.STAND);

    }

    public void OnSplitBtnClick()
    {

    }

    public void OnDoubleBtnClick()
    {
        isDoubleBet = true;
        rm.potHandler.PlaceDoubleBetAmount();
        Invoke(nameof(OnHitBtnClick), 0.5f);
    }

    public void OnHitBtnClick()
    {
        ShowHitStandBar(false);
        rm.tableDealer.SendOneCard(true);
    }

    public void CheckPlayerScoresLimit()
    {

    }
    bool CheckIfPlayerHaveEnoughChips
    {
        get { return rm.potHandler.GetPotAmount * 2 <= LocalSetting.GetTotalCash(); }
    }


    public void ResetThings()
    {

        isDoubleBet = false;
        doubleBtn.SetActive(false);
    }
}
