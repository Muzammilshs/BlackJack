using UnityEngine;
using com.muzamil;

public class HitStandBarHandler : MonoBehaviour
{
    public Rm rm;
    public GameObject hitStandBar;
    public GameObject splitBtn;
    public GameObject doubleBtn;

    [ShowOnly] public bool isDoubleBet;

    [ShowOnly] public bool isSplitting;

    [ShowOnly] public int splitingTurnNumber;

    private void Start()
    {
        hitStandBar.SetActive(false);
    }
    public void CompareScoresAfter4Cards()
    {
        TableDealer td = rm.tableDealer;
        ScoreManager sm = rm.scoreManager;

        int playerScores = 0;
        int dealerScores = 0;

        // getting player score
        if (td.playerCards[0].Card == CardState.CARDVALUE.ACE && td.playerCards[1].Card == CardState.CARDVALUE.ACE)
            playerScores = td.playerCards[0].Power + td.playerCards[1].SecondPower;
        else
            playerScores = td.playerCards[0].Power + td.playerCards[1].Power;

        // getting dealer scores
        if (td.dealerCards[0].Card == CardState.CARDVALUE.ACE && td.dealerCards[1].Card == CardState.CARDVALUE.ACE)
            dealerScores = td.dealerCards[0].Power + td.dealerCards[1].SecondPower;
        else
            dealerScores = td.dealerCards[0].Power + td.dealerCards[1].Power;

        //Debug.LogError("Player scores after 4 card: " + playerScores + "     Dealer Scores: " + dealerScores);
        // When win with first 2 cards
        if (playerScores == LocalSetting.ScoresLimit || dealerScores == LocalSetting.ScoresLimit)
        {
            ShowHitStandBar(false);
            if (playerScores > dealerScores)
            {
                rm.dealerAIPlay.isJackPot = true;
                rm.dealerAIPlay.isDealerTurn = true;
                rm.dealerAIPlay.DropDealerCard();
            }
            else if (playerScores < dealerScores)
            {
                rm.dealerAIPlay.isJackPot = false;
                rm.dealerAIPlay.isDealerTurn = true;
                rm.dealerAIPlay.DropDealerCard();

            }
            else
                rm.dealerAIPlay.DropDealerCard();
        }
        else
        {
            // when not won with first 2 cards
            ShowHitStandBar(true);
            if (rm.potHandler.IsHaveAmount(rm.potHandler.GetPotAmount))
                doubleBtn.SetActive(true);
            else
                doubleBtn.SetActive(false);
            if (isPlayerHaveSamePowerCards())
            {
                if (rm.potHandler.IsHaveAmount(rm.potHandler.GetPotAmount))
                    splitBtn.SetActive(true);
                else
                    splitBtn.SetActive(false);
            }
            else
                splitBtn.SetActive(false);
        }
    }

    bool isPlayerHaveSamePowerCards()
    {
        return rm.tableDealer.playerCards[0].Power == rm.tableDealer.playerCards[1].Power;
        //return false;
    }
    public void ShowHitStandBar(bool isShow)
    {
        hitStandBar.SetActive(isShow);
        doubleBtn.SetActive(false);
    }

    public void OnStandBtnClick()
    {
        if (!isSplitting)
            rm.gameStateManager.UpDateGameState(GameState.State.STAND);
        else
            rm.tableDealer.OnSplitCheckNextTurn(true);
    }

    public void OnSplitBtnClick()
    {
        if (!rm.potHandler.IsHaveAmount(rm.potHandler.GetPotAmount))
        {
            rm.gameManager.shopPanel.SetActive(true);
            return;
        }
        rm.potHandler.BetAmountDeduction(rm.potHandler.GetPotAmount);
        isSplitting = true;
        splitingTurnNumber = 0;
        rm.tableDealer.SplitCardsOnSplit();
        rm.betBarHandler.DuplicateBettedChipsAndAmountOnSplit();
        hitStandBar.SetActive(true);
        splitBtn.SetActive(false);
        doubleBtn.SetActive(false);
        rm.tableDealer.SendOneCardOnSplit();
    }

    public void OnDoubleBtnClick()
    {
        if (!rm.potHandler.IsHaveAmount(rm.potHandler.GetPotAmount))
        {
            rm.gameManager.shopPanel.SetActive(true);
            return;
        }
        isDoubleBet = true;
        rm.potHandler.PlaceDoubleBetAmount();
        Invoke(nameof(OnHitBtnClick), 0.5f);
    }

    public void OnHitBtnClick()
    {
        ShowHitStandBar(false);
        if (!isSplitting)
        {
            rm.tableDealer.SendOneCard(true);
        }
        else
        {
            if (splitingTurnNumber < 2)
                rm.tableDealer.SendOneCardOnSplit();
        }
        splitBtn.SetActive(false);
    }


    public void ResetThings()
    {
        isSplitting = false;
        isDoubleBet = false;
        doubleBtn.SetActive(false);
        splitingTurnNumber = 0;
    }
}
