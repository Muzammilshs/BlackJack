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
        while (isDropNextCard() && !isJackPot)
        {
            rm.tableDealer.SendOneCard(false);
            yield return new WaitForSeconds(1.5f);
        }
        //asdf
        rm.tableDealer.DeclearWinner(isJackPot);
        //int totalCards = rm.tableDealer.dealerCards.Count + rm.tableDealer.playerCards.Count;
        //if (totalCards != 4)
        //    rm.tableDealer.DeclearWinner(false);
    }

    bool isDropNextCard()
    {
        if (rm.scoreManager.dealerTotalScores >= rm.scoreManager.targetScores - 3)
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
