using com.muzammil;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Rm rm;
    [ShowOnly]
    public int playerTotalScores, playerTotalScores_P1_Split, playerTotalScores_P2_Split, dealerTotalScores;
    [ShowOnly]
    public int playerTotalScoresAce, playerTotalScoresAce_P1_Split, playerTotalScoresAce_P2_Split, dealerTotalScoresAce;
    [SerializeField] TMP_Text playerScores;
    [SerializeField] TMP_Text playerScoresTxt_P1_Split;
    [SerializeField] TMP_Text playerScoresTxt_P2_Split;
    [SerializeField] TMP_Text dealerScores;

    [SerializeField] GameObject playerScoresParent;
    [SerializeField] GameObject playerScoresParent_P1_Split;
    [SerializeField] GameObject playerScoresParent_P2_Split;
    [SerializeField] GameObject dealerScoresParent;


    public void ShowScoreObjects(bool isShow)
    {
        if (!isShow)
        {
            playerTotalScores = 0;
            dealerTotalScores = 0;

            playerTotalScores_P1_Split = 0;
            playerTotalScores_P2_Split = 0;
        }
        if (!rm.hitStandBarHandler.isSplitting)
        {
            playerScoresParent.SetActive(isShow);
            playerScoresParent_P1_Split.SetActive(false);
            playerScoresParent_P2_Split.SetActive(false);
        }
        else
        {
            playerScoresParent.SetActive(false);
            playerScoresParent_P1_Split.SetActive(isShow);
            playerScoresParent_P2_Split.SetActive(isShow);
        }
        dealerScoresParent.SetActive(isShow);
    }

    public void RoundPlayerScores()
    {
        if (!rm.hitStandBarHandler.isSplitting)
            playerScores.text = playerTotalScores.ToString();
        else
        {

        }
    }
    public void RoundDealerScores()
    {
        dealerScores.text = dealerTotalScores.ToString();
    }

    public void SplitPlayerScores()
    {
        CalculateCardsScores(rm.tableDealer.playerCards_1_Split, out playerTotalScores_P1_Split, out playerTotalScoresAce_P1_Split);
        CalculateCardsScores(rm.tableDealer.playerCards_2_Split, out playerTotalScores_P2_Split, out playerTotalScoresAce_P2_Split);

        playerScoresTxt_P1_Split.text = playerTotalScores_P1_Split.ToString();
        playerScoresTxt_P2_Split.text = playerTotalScores_P2_Split.ToString();

        playerScoresParent.SetActive(false);
        playerScoresParent_P1_Split.SetActive(true);
        playerScoresParent_P2_Split.SetActive(true);
    }
    public void SetScores(bool isPlayer)
    {
        ShowScoreObjects(true);

        if (isPlayer)
        {
            playerTotalScores = 0;
            playerTotalScoresAce = 0;
            CalculateCardsScores(rm.tableDealer.playerCards, out playerTotalScores, out playerTotalScoresAce);

            if (playerTotalScoresAce == playerTotalScores)
            {
                playerScores.text = playerTotalScores.ToString(); // Avoid showing duplicate scores
            }
            else if (playerTotalScoresAce > 0 && playerTotalScores <= LocalSettingBlackJack.ScoresLimit)
            {
                playerScores.text = playerTotalScoresAce + "/" + playerTotalScores;
            }
            else if (playerTotalScoresAce > 0 && playerTotalScores > LocalSettingBlackJack.ScoresLimit)
            {
                playerScores.text = playerTotalScoresAce.ToString();
            }
            else
            {
                playerScores.text = playerTotalScores.ToString();
            }

            if (rm.tableDealer.playerCards.Count > 2 || playerTotalScores == LocalSettingBlackJack.ScoresLimit)
                StartCoroutine(CheckForPlayerScoreLimit());
        }
        else
        {
            dealerTotalScores = 0;
            dealerTotalScoresAce = 0;
            CalculateCardsScores(rm.tableDealer.dealerCards, out dealerTotalScores, out dealerTotalScoresAce);

            if (dealerTotalScoresAce == dealerTotalScores)
            {
                dealerScores.text = dealerTotalScores.ToString();
            }
            else if (dealerTotalScoresAce > 0 && dealerTotalScores <= LocalSettingBlackJack.ScoresLimit)
            {
                dealerScores.text = dealerTotalScoresAce + "/" + dealerTotalScores;
            }
            else if (dealerTotalScoresAce > 0 && dealerTotalScores > LocalSettingBlackJack.ScoresLimit)
            {
                dealerScores.text = dealerTotalScoresAce.ToString();
            }
            else
            {
                dealerScores.text = dealerTotalScores.ToString();
            }
        }
    }

    IEnumerator CheckForPlayerScoreLimit()
    {
        yield return new WaitForSeconds(0.5f);
        if (playerTotalScores < LocalSettingBlackJack.ScoresLimit)
        {
            if (!rm.dealerAIPlay.isDealerTurn)
            {
                if (!rm.hitStandBarHandler.isDoubleBet)
                {
                    rm.hitStandBarHandler.ShowHitStandBar(true);
                }
                else
                {

                    rm.hitStandBarHandler.OnStandBtnClick();
                    rm.dealerAIPlay.isDealerTurn = false;
                }
            }
        }
        else if (playerTotalScores > LocalSettingBlackJack.ScoresLimit)
        {
            // 
            rm.tableDealer.FlipCard(rm.tableDealer.dealerCards[1].gameObject, true, false);
            yield return new WaitForSeconds(0.75f);
            rm.tableDealer.UpDateWinStatus(TableDealer.Winner.BUST);
        }
        else
        {
            // show dealer cards and win declear 
            rm.tableDealer.FlipCard(rm.tableDealer.dealerCards[1].gameObject, true, false);
            yield return new WaitForSeconds(0.75f);
            if (rm.tableDealer.dealerCards.Count + rm.tableDealer.playerCards.Count != 4)
            {
                if (playerTotalScores > dealerTotalScores)
                {
                    if (rm.tableDealer.dealerCards.Count + rm.tableDealer.playerCards.Count != 4)
                        rm.tableDealer.UpDateWinStatus(TableDealer.Winner.WON);
                    else
                        rm.tableDealer.UpDateWinStatus(TableDealer.Winner.JACKPOT);

                }
                else
                    rm.tableDealer.UpDateWinStatus(TableDealer.Winner.PUSH);
            }

        }
    }

    void CalculateCardsScores(List<CardProperty> cardsList, out int highScores, out int lowScores)
    {
        highScores = 0;
        lowScores = 0;
        int aceCount = 0;

        // Calculate initial lowScores, treating Aces as 1
        foreach (CardProperty card in cardsList)
        {
            if (card.Card == CardState.CARDVALUE.ACE)
            {
                aceCount++;
                lowScores += card.SecondPower; // Assume Ace is 1 initially
            }
            else
            {
                lowScores += card.Power;
            }
        }

        // High score calculation: Try to upgrade one Ace to 11 if it doesn't bust
        highScores = lowScores;
        if (aceCount > 0 && highScores + 10 <= LocalSettingBlackJack.ScoresLimit)
        {
            highScores += 10; // Upgrade one Ace from 1 to 11
        }
    }


    public void ResetTotalScore()
    {
        playerTotalScores = 0;
        dealerTotalScores = 0;
        playerTotalScoresAce = 0;
        dealerTotalScoresAce = 0;
        playerScores.text = string.Empty;
        dealerScores.text = string.Empty;
    }

    void ResetSplitCardsAndTxt()
    {
        //playerTotalScores_P2_Split
    }
}
