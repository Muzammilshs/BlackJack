using System.Collections;
using UnityEngine;

public class DealerAIPlay : MonoBehaviour
{
    [SerializeField] Rm rm;
    [ShowOnly]
    public bool isDealerTurn;
    [ShowOnly]
    public bool isJackPot = false;
    void Start()
    {
    }

    public void DropDealerCard()
    {
        // flip dealer card
        rm.tableDealer.FlipCard(rm.tableDealer.dealerCards[1].gameObject, true, false);
        StartCoroutine(DropCards());
    }

    IEnumerator DropCards()
    {
        //isJackPot = isDropNextCard();
        yield return new WaitForSeconds(1.25f);
        if (!rm.hitStandBarHandler.isSplitting)
        {
            while (isDropNextCard() && !isJackPot)
            {
                rm.tableDealer.SendOneCard(false);
                yield return new WaitForSeconds(1.5f);
            }
        }
        else
        {
            while (isDropNextCard() && (rm.scoreManager.playerTotalScores_P1_Split <= 21 || rm.scoreManager.playerTotalScores_P2_Split <= 21))
            {
                rm.tableDealer.SendOneCard(false);
                yield return new WaitForSeconds(1.5f);
            }
        }
        //asdf
        rm.scoreManager.RoundDealerScores();
        if (!rm.hitStandBarHandler.isSplitting)
            rm.tableDealer.DeclearWinner(isJackPot);
        else
            rm.tableDealer.DeclearWinnerWithSplit();
    }

    bool isDropNextCard()
    {
        if (rm.scoreManager.dealerTotalScores >= LocalSetting.ScoresLimit - 3)
            return false;
        else if (isJackPot)
            return false;
        else return true;
    }

    public void ResetDealer()
    {
        isDealerTurn = false;
        isJackPot = false;
    }
}
