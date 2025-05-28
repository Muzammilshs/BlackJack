using com.muzammil;
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
        //rm.tableDealer.FlipCard(rm.tableDealer.dealerCards[1].gameObject, true, false);
        Rm.currentCardData = rm.GetCardData(HandType.HANDTYPE.DEALERHAND);
        Rm.currentCardData.FlipCard(Rm.currentCardData.cardsList[1].gameObject, true);
        StartCoroutine(DropCards());
    }

    IEnumerator DropCards()
    {
        yield return new WaitForSeconds(1.25f);
  
        Rm.currentCardData = rm.GetCardData(HandType.HANDTYPE.DEALERHAND);
        while (isDropNextCard() && !isJackPot)
        {
            rm.tableDealer.SendOneCardGen(Rm.currentCardData);
            yield return new WaitForSeconds(1.5f);
        }

        rm.tableDealer.ShowResultsNew();
    }

    bool isDropNextCard()
    {
        int dealerScores = Rm.currentCardData.highScores;
        if (dealerScores >= LocalSettingBlackJack.ScoresLimit - 3)
            return false;
        else if (isJackPot)
            return false;
        else return true;
    }

    public void ResetDealer()
    {
        return;
        isDealerTurn = false;
        isJackPot = false;
    }
}
