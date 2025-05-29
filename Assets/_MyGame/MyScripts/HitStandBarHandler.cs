using UnityEngine;
using com.muzammil;

/// <summary>
/// Handles the UI and logic for the Hit/Stand bar in blackjack, including split and double bet actions.
/// </summary>
public class HitStandBarHandler : MonoBehaviour
{
    #region Inspector Fields

    public Rm rm;                        // Reference to the main resource manager
    public GameObject hitStandBar;       // The main hit/stand bar UI
    public GameObject splitBtn;          // Split button UI
    public GameObject doubleBtn;         // Double bet button UI

    [ShowOnly] public bool isDoubleBet;          // True if double bet is active
    [ShowOnly] public bool isSplitting;          // True if currently splitting
    [ShowOnly] public int splitingTurnNumber;    // Current split turn number

    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the hit/stand bar to be hidden at the start.
    /// </summary>
    private void Start()
    {
        hitStandBar.SetActive(false);
    }

    #endregion

    #region Main Logic

    /// <summary>
    /// Compares player and dealer scores after the initial 4 cards and updates the UI accordingly.
    /// </summary>
    public void CompareScoresAfter4Cards()
    {
        int playerScores = rm.GetCardData(HandType.HANDTYPE.PLAYERHAND).highScores;
        int dealerScores = rm.GetCardData(HandType.HANDTYPE.DEALERHAND).highScores;

        // Check for natural blackjack or dealer blackjack
        if (playerScores == LocalSettingBlackJack.ScoresLimit || dealerScores == LocalSettingBlackJack.ScoresLimit)
        {
            ShowHitStandBar(false);
            if (playerScores > dealerScores)
            {
                rm.dealerAIPlay.isJackPot = true;
                rm.dealerAIPlay.isDealerTurn = true;
            }
            else if (playerScores < dealerScores)
            {
                rm.dealerAIPlay.isJackPot = false;
                rm.dealerAIPlay.isDealerTurn = true;
            }
            rm.GetCardData(HandType.HANDTYPE.PLAYERHAND).ShowJustHighScores();
            rm.GetCardData(HandType.HANDTYPE.DEALERHAND).ShowJustHighScores();
            rm.gameStateManager.UpDateGameState(GameState.State.STAND);
        }
        else
        {
            // No blackjack, show hit/stand bar and check for double/split options
            ShowHitStandBar(true);

            // Show double bet button if player has enough cash
            if (rm.potHandler.IsHaveAmount(rm.potHandler.GetPotAmount))
                doubleBtn.SetActive(true);
            else
                doubleBtn.SetActive(false);

            // Set current card data to player hand for split check
            Rm.currentCardData = rm.GetCardData(HandType.HANDTYPE.PLAYERHAND);

            // Check if split button should be enabled
            CheckForSplitButtonActivation();
        }
    }

    /// <summary>
    /// Checks if the split button should be enabled for the current hand.
    /// </summary>
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
        {
            splitBtn.SetActive(false);
        }
    }

    /// <summary>
    /// Checks if the current hand has two cards of the same power (for split eligibility).
    /// </summary>
    bool isPlayerHaveSamePowerCards(CardsData cardsData)
    {
        if (cardsData.cardsList.Count == 2)
            return cardsData.cardsList[0].Power == cardsData.cardsList[1].Power;
        return false;
    }

    /// <summary>
    /// Shows or hides the hit/stand bar and disables the double button.
    /// </summary>
    public void ShowHitStandBar(bool isShow)
    {
        hitStandBar.SetActive(isShow);
        doubleBtn.SetActive(false);
    }

    #endregion

    #region Button Event Handlers

    /// <summary>
    /// Called when the Stand button is clicked. Proceeds to the next hand or dealer turn.
    /// </summary>
    public void OnStandBtnClick()
    {
        rm.tableDealer.CheckScoresForNext(true);
    }

    /// <summary>
    /// Called when the Split button is clicked. Deducts bet, splits cards, and deals new cards.
    /// </summary>
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

    /// <summary>
    /// Called when the Double button is clicked. Deducts double bet, disables UI, and hits.
    /// </summary>
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

    /// <summary>
    /// Called when the Hit button is clicked. Deals a card to the current hand and disables split/double.
    /// </summary>
    public void OnHitBtnClick()
    {
        ShowHitStandBar(false);
        rm.tableDealer.SendOneCardGen(Rm.currentCardData);
        splitBtn.SetActive(false);
        doubleBtn.SetActive(false);
    }

    #endregion
}
