using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the pot (betting pool) logic, including bet placement, insurance, cash updates, and UI for the blackjack game.
/// </summary>
public class PotHandler : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField] Rm refMgr;                        // Reference to the main resource manager
    public TMP_Text[] totalAmountTxt;                  // UI text(s) displaying the player's total cash
    [SerializeField] TMP_Text _insuranceAmountTxt;     // UI text for the insurance bet amount

    #endregion

    #region Private Fields

    int totalBetPlaced;            // Total bet placed for the current round
    int _insuranceBetAmount;       // Current insurance bet amount

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current insurance bet amount.
    /// </summary>
    public int GetInsuranceAmount { get { return _insuranceBetAmount; } }

    /// <summary>
    /// Gets the current total bet placed (the pot amount).
    /// </summary>
    public int GetPotAmount { get { return totalBetPlaced; } }

    #endregion

    #region Unity Methods

    void Start()
    {
        UpDateCashTxt(LocalSettingBlackJack.GetTotalCash());
    }

    #endregion

    #region Bet and Insurance Logic

    /// <summary>
    /// Resets the total bet placed and clears the betted amount UI.
    /// </summary>
    public void ResetTotalBet()
    {
        totalBetPlaced = 0;
        refMgr.GetCardData(HandType.HANDTYPE.PLAYERHAND).SetBettedAmount("");
    }

    /// <summary>
    /// Resets the insurance bet amount and hides the insurance UI.
    /// </summary>
    public void ResetInsuranceAmount()
    {
        _insuranceBetAmount = 0;
        _insuranceAmountTxt.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// Places a bet of the specified amount, updating the total and UI.
    /// </summary>
    public void PlaceBetAmount(int amount)
    {
        if (!IsHaveAmount(amount))
        {
            refMgr.gameManager.shopPanel.SetActive(true);
            return;
        }
        totalBetPlaced += amount;
        CardsData cd = refMgr.GetCardData(HandType.HANDTYPE.PLAYERHAND);
        cd.SetBettedAmount(totalBetPlaced.ToString());
    }

    /// <summary>
    /// Places a double bet (for double down), deducts the amount, and updates UI.
    /// </summary>
    public void PlaceDoubleBetAmountNew()
    {
        if (!IsHaveAmount(GetPotAmount))
        {
            refMgr.gameManager.shopPanel.SetActive(true);
            return;
        }
        CardsData cd = refMgr.GetCardData(HandType.HANDTYPE.PLAYERHAND);
        cd.ActivateDoubleBetItems();
        BetAmountDeduction(totalBetPlaced);
    }

    /// <summary>
    /// Checks if the player has enough cash for the specified amount.
    /// </summary>
    public bool IsHaveAmount(int amount)
    {
        return amount <= LocalSettingBlackJack.GetTotalCash();
    }

    /// <summary>
    /// Sets the insurance bet amount and updates UI, or shows shop if not enough cash.
    /// </summary>
    public void SetInsuranceAmount(bool isInsured)
    {
        if (isInsured)
        {
            int requiredAmount = refMgr.potHandler.GetPotAmount / 2;
            if (refMgr.potHandler.GetPotAmount % 2 != 0)
                requiredAmount += 1;
            if (refMgr.potHandler.IsHaveAmount(requiredAmount))
            {
                _insuranceBetAmount = requiredAmount;
                BetAmountDeduction(_insuranceBetAmount);
                _insuranceAmountTxt.text = _insuranceBetAmount.ToString();
                _insuranceAmountTxt.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                refMgr.gameManager.shopPanel.SetActive(true);
                refMgr.tableDealer._insurancePanel.SetActive(true);
                return;
            }
        }
        else
        {
            _insuranceAmountTxt.transform.parent.gameObject.SetActive(false);
            _insuranceBetAmount = 0;
        }
    }

    #endregion

    #region Cash Update and Animation

    /// <summary>
    /// Deducts the specified amount from the player's total cash.
    /// </summary>
    public void BetAmountDeduction(int amount)
    {
        UpDateTotalCash(amount, false);
    }

    /// <summary>
    /// Adds the specified amount to the player's total cash (reward).
    /// </summary>
    public void CollectReward(int amount)
    {
        UpDateTotalCash(amount, true);
    }

    /// <summary>
    /// Updates the player's total cash, with animation.
    /// </summary>
    /// <param name="amount">Amount to add or deduct.</param>
    /// <param name="isReward">True if adding (reward), false if deducting (bet/insurance).</param>
    void UpDateTotalCash(int amount, bool isReward)
    {
        int prevAmount = LocalSettingBlackJack.GetTotalCash();
        LocalSettingBlackJack.SetTotalCashWithBetLocal(amount, isReward);
        if (!isReward)
            amount = -amount;

        LocalSettingBlackJack.SetTotalCash(amount);
        UpdateCashAmount(prevAmount, LocalSettingBlackJack.GetTotalCashLocal());
    }

    /// <summary>
    /// Updates all cash UI texts to the current cash value.
    /// </summary>
    public void UpDateCashTexts()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            MenuController.instance.UpDateTotalChipsTxtsWithLocalValues();
        }
        else
        {
            Rm.Instance.potHandler.UpDateCashTxt(LocalSettingBlackJack.GetTotalCashLocal());
        }
    }

    /// <summary>
    /// Animates the cash value from the current to the target amount.
    /// </summary>
    void UpdateCashAmount(int currentCash, int targetAmount, float duration = 1f)
    {
        int startAmount = currentCash;

        DOTween.To(() => startAmount, x => startAmount = x, targetAmount, duration)
            .OnUpdate(() =>
            {
                UpDateCashTxt(startAmount);
            })
            .OnComplete(() =>
            {
                currentCash = targetAmount;
            });
    }

    /// <summary>
    /// Sets all cash UI texts to the specified amount.
    /// </summary>
    public void UpDateCashTxt(int amount)
    {
        foreach (var amountTxt in totalAmountTxt)
            amountTxt.text = amount.ToString("N0");
    }

    #endregion
}
