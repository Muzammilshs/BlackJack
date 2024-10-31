using com.muzamil;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Rm rm;
    [ShowOnly]
    public int targetScores = 21;
    [ShowOnly]
    public int playerTotalScores, dealerTotalScores;
    [ShowOnly]
    public int playerTotalScoresAce, dealerTotalScoresAce;
    [SerializeField] TMP_Text playerScores;
    [SerializeField] TMP_Text dealerScores;

    [SerializeField] GameObject playerScoresParent;
    [SerializeField] GameObject dealerScoresParent;


    public void ShowScoreObjects(bool isShow)
    {
        if (!isShow)
        {
            playerTotalScores = 0;
            dealerTotalScores = 0;
        }
        playerScoresParent.SetActive(isShow);
        dealerScoresParent.SetActive(isShow);
    }
    public void SetScores(bool isPlayer)
    {
        ShowScoreObjects(true);
        if (isPlayer)
        {
            playerTotalScores = 0;
            playerTotalScoresAce = 0;
            CalculateCardsScores(rm.tableDealer.playerCards, out playerTotalScores, out playerTotalScoresAce);
            if (playerTotalScoresAce == 0)
            {
                playerScores.text = playerTotalScores.ToString();
            }
            else if (playerTotalScoresAce > 0 && playerTotalScores <= targetScores)
            {
                playerScores.text = playerTotalScoresAce + "/" + playerTotalScores;
                if (playerTotalScores == targetScores)
                    playerScores.text = playerTotalScores.ToString();

            }
            else if (playerTotalScoresAce > 0 && playerTotalScores > targetScores)
            {
                playerTotalScores = playerTotalScoresAce;
                playerScores.text = playerTotalScores.ToString();
            }
            else
                playerScores.text = playerTotalScores.ToString();
            //if (rm.tableDealer.playerCards.Count > 1)
            StartCoroutine(CheckForPlayerScoreLimit());
        }
        else
        {
            dealerTotalScores = 0;
            dealerTotalScoresAce = 0;
            CalculateCardsScores(rm.tableDealer.dealerCards, out dealerTotalScores, out dealerTotalScoresAce);
            if (dealerTotalScoresAce == 0)
            {
                dealerScores.text = dealerTotalScores.ToString();
            }
            else if (dealerTotalScoresAce > 0 && dealerTotalScores <= targetScores)
            {
                dealerScores.text = dealerTotalScoresAce + "/" + dealerTotalScores;
                if (dealerTotalScores == targetScores)
                    dealerScores.text = dealerTotalScores.ToString();
            }
            else if (dealerTotalScoresAce > 0 && dealerTotalScores > targetScores)
            {
                dealerTotalScores = dealerTotalScoresAce;
                dealerScores.text = dealerTotalScores.ToString();
            }
            else
                dealerScores.text = dealerTotalScores.ToString();

        }

    }
    IEnumerator CheckForPlayerScoreLimit()
    {
        yield return new WaitForSeconds(1);
        if (playerTotalScores < targetScores)
        {
            if (!rm.dealerAIPlay.isDealerTurn)
            {
                Debug.LogError("activating bar       " + playerTotalScores);
                rm.hitStandBarHandler.ShowHitStandBar(true);
            }
        }
        else if (playerTotalScores > targetScores)
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
            if (playerTotalScores > dealerTotalScores)
                rm.tableDealer.UpDateWinStatus(TableDealer.Winner.WON);
            else
                rm.tableDealer.UpDateWinStatus(TableDealer.Winner.PUSH);

        }
        //rm.hitStopBarHandler.CheckPlayerScoresLimit();
    }
    void CalculateCardsScores(List<CardProperty> cardsList, out int highScores, out int lowScores)
    {
        bool isHaveAce = isContainAce(cardsList);
        highScores = 0;
        lowScores = 0;
        if (!isHaveAce)
        {
            foreach (CardProperty card in cardsList)
                highScores += card.Power;
        }
        else
        {
            // scores with ace low power
            foreach (CardProperty card in cardsList)
            {
                if (card.Card == CardState.CARDVALUE.ACE)
                    lowScores += card.SecondPower;
                else
                    lowScores += card.Power;
            }

            // set scores with high power
            int totalAceCards = 0;
            foreach (CardProperty card in cardsList)
            {
                if (card.Card == CardState.CARDVALUE.ACE)
                    totalAceCards++;
            }

            foreach (CardProperty card in cardsList)
                highScores += card.Power;
            if (totalAceCards > 1)
            {
                highScores = 0;
                // calculate non ace cards
                foreach (CardProperty card in cardsList)
                    if (card.Card != CardState.CARDVALUE.ACE)
                        highScores += card.Power;
                int index = 0;
                foreach (CardProperty card in cardsList)
                {
                    if (card.Card == CardState.CARDVALUE.ACE)
                    {
                        int tempScore = highScores;
                        tempScore += card.Power;
                        index++;
                        if (tempScore >= targetScores && index == 1)
                            highScores = card.SecondPower;
                        else if (tempScore < targetScores && index == 1)
                            highScores = tempScore;
                        else
                            highScores += card.SecondPower;
                    }
                }
            }
        }
    }
    bool isContainAce(List<CardProperty> cardsList)
    {
        bool isAce = false;
        foreach (CardProperty card in cardsList)
            if (card.Card == CardState.CARDVALUE.ACE)
                isAce = true;
        return isAce;
    }

    public void ResetTotalScore()
    {
        playerTotalScores = 0;
        dealerTotalScores = 0;
        playerTotalScoresAce = 0;
        dealerTotalScoresAce = 0;
    }
}
