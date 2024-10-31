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
        yield return new WaitForSeconds(2.5f);
        while (isDropNextCard())
        {
            rm.tableDealer.SendOneCard(false);
            yield return new WaitForSeconds(1.5f);
        }
        rm.tableDealer.DeclearWinner(isJackPot);
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
