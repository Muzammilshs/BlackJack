using UnityEngine;
using com.muzammil;

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
        //TableDealer td = rm.tableDealer;
        //ScoreManager sm = rm.scoreManager;

        int playerScores = rm.GetCardData(HandType.HANDTYPE.PLAYERHAND).highScores;
        int dealerScores = rm.GetCardData(HandType.HANDTYPE.DEALERHAND).highScores;

        //// getting player score
        //if (td.playerCards[0].Card == CardState.CARDVALUE.ACE && td.playerCards[1].Card == CardState.CARDVALUE.ACE)
        //    playerScores = td.playerCards[0].Power + td.playerCards[1].SecondPower;
        //else
        //    playerScores = td.playerCards[0].Power + td.playerCards[1].Power;

        //// getting dealer scores
        //if (td.dealerCards[0].Card == CardState.CARDVALUE.ACE && td.dealerCards[1].Card == CardState.CARDVALUE.ACE)
        //    dealerScores = td.dealerCards[0].Power + td.dealerCards[1].SecondPower;
        //else
        //    dealerScores = td.dealerCards[0].Power + td.dealerCards[1].Power;

        //Debug.LogError("Player scores after 4 card: " + playerScores + "     Dealer Scores: " + dealerScores);
        // When win with first 2 cards
        if (playerScores == LocalSettingBlackJack.ScoresLimit || dealerScores == LocalSettingBlackJack.ScoresLimit)
        {
            ShowHitStandBar(false);
            if (playerScores > dealerScores)
            {
                rm.dealerAIPlay.isJackPot = true;
                rm.dealerAIPlay.isDealerTurn = true;
                //rm.dealerAIPlay.DropDealerCard();
            }
            else if (playerScores < dealerScores)
            {
                rm.dealerAIPlay.isJackPot = false;
                rm.dealerAIPlay.isDealerTurn = true;
                //rm.dealerAIPlay.DropDealerCard();

            }
            //rm.dealerAIPlay.DropDealerCard();
            rm.gameStateManager.UpDateGameState(GameState.State.STAND);
        }
        else
        {
            // when not won with first 2 cards
            ShowHitStandBar(true);

            // Checking to show Double bet Button.
            if (rm.potHandler.IsHaveAmount(rm.potHandler.GetPotAmount))
                doubleBtn.SetActive(true);
            else
                doubleBtn.SetActive(false);

            // current card data after 4 cards to player hand cards
            Rm.currentCardData = rm.GetCardData(HandType.HANDTYPE.PLAYERHAND);

            // Checking to show Split button
            CheckForSplitButtonActivation();

        }
    }

    public void CheckForSplitButtonActivation()
    {
        if (Rm.currentCardData.handType == HandType.HANDTYPE.PLAYERHAND ||
            Rm.currentCardData.handType == HandType.HANDTYPE.PLAYERHAND_SPLIT_P1 ||
            Rm.currentCardData.handType == HandType.HANDTYPE.PLAYERHAND_SPLIT_P2)
        {
            if (isPlayerHaveSamePowerCards(Rm.currentCardData))
                splitBtn.SetActive(true);
            else
                splitBtn.SetActive(false);
        }
        else
            splitBtn.SetActive(false);
    }

    bool isPlayerHaveSamePowerCards(CardsData cardsData)
    {
        if (cardsData.cardsList.Count == 2)
            return cardsData.cardsList[0].Power == cardsData.cardsList[1].Power;
        return false;
    }
    public void ShowHitStandBar(bool isShow)
    {
        hitStandBar.SetActive(isShow);
        doubleBtn.SetActive(false);
    }

    public void OnStandBtnClick()
    {
        rm.tableDealer.CheckScoresForNext(true);
    }

    public void OnSplitBtnClick()
    {
        if (!rm.potHandler.IsHaveAmount(rm.potHandler.GetPotAmount))
        {
            rm.gameManager.shopPanel.SetActive(true);
            return;
        }
        rm.potHandler.BetAmountDeduction(rm.potHandler.GetPotAmount);
        rm.tableDealer.SplitCardsOnSplit();
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
        ShowHitStandBar(false);
        rm.potHandler.PlaceDoubleBetAmountNew();
        Invoke(nameof(OnHitBtnClick), 0.5f);
    }

    public void OnHitBtnClick()
    {
        ShowHitStandBar(false);
        rm.tableDealer.SendOneCardGen(Rm.currentCardData);
        splitBtn.SetActive(false);
        doubleBtn.SetActive(false);
    }


    public void ResetThings()
    {
        isSplitting = false;
        isDoubleBet = false;
        doubleBtn.SetActive(false);
        splitingTurnNumber = 0;
    }
}
