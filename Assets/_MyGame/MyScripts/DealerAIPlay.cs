using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the dealer's AI logic for playing out the dealer's hand in blackjack,
/// including revealing the dealer's hidden card and drawing additional cards as needed.
/// </summary>
public class DealerAIPlay : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField] Rm rm;                // Reference to the main resource manager

    [ShowOnly]
    public bool isDealerTurn;              // True if it's currently the dealer's turn

    [ShowOnly]
    public bool isJackPot = false;         // True if the dealer has a natural blackjack

    #endregion

    #region Dealer Play Logic

    /// <summary>
    /// Reveals the dealer's hidden card and starts the dealer's turn logic.
    /// </summary>
    public void DropDealerCard()
    {
        // Set current hand to dealer and flip the hidden card
        Rm.currentCardData = rm.GetCardData(HandType.HANDTYPE.DEALERHAND);
        Rm.currentCardData.FlipCard(Rm.currentCardData.cardsList[1].gameObject, true);
        StartCoroutine(DropCards());
    }

    /// <summary>
    /// Coroutine to handle the dealer's card drawing logic according to blackjack rules.
    /// </summary>
    IEnumerator DropCards()
    {
        yield return new WaitForSeconds(1.25f);

        Rm.currentCardData = rm.GetCardData(HandType.HANDTYPE.DEALERHAND);
        // Dealer draws cards until reaching a safe threshold or a jackpot
        while (isDropNextCard() && !isJackPot)
        {
            rm.tableDealer.SendOneCardGen(Rm.currentCardData);
            yield return new WaitForSeconds(1.5f);
        }
        // Show final dealer score and trigger result evaluation
        Rm.currentCardData.ShowJustHighScores();
        rm.tableDealer.ShowResultsNew();
    }

    /// <summary>
    /// Determines if the dealer should draw another card.
    /// Dealer stands on 18 or higher, or if a jackpot is present.
    /// </summary>
    /// <returns>True if dealer should draw another card, false otherwise.</returns>
    bool isDropNextCard()
    {
        int dealerScores = Rm.currentCardData.highScores;
        if (dealerScores >= LocalSettingBlackJack.ScoresLimit - 3)
            return false;
        else if (isJackPot)
            return false;
        else
            return true;
    }

    #endregion
}
