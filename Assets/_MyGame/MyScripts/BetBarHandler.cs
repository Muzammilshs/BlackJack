using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles the bet bar UI, bet button creation, chip animations, and bet logic for the blackjack game.
/// </summary>
public class BetBarHandler : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField] Rm refMgr;                         // Reference to the main game manager
    [SerializeField] BetAmounts[] betAmounts;           // Array of available bet amounts and their icons
    [SerializeField] GameObject betBar;                 // The bet bar UI object
    [SerializeField] GameObject betBtnPrefab;           // Prefab for a single bet button
    [SerializeField] GameObject placeYourBetMsg;        // "Place your bet" message UI
    [SerializeField] GameObject clearBetBtn;            // Clear bet button UI
    [SerializeField] GameObject dealBtn;                // Deal button UI
    [SerializeField] GameObject dealnClearBtnGroup;     // Group containing deal and clear buttons
    [SerializeField] GameObject betBarPos1;             // Bet bar position when visible
    [SerializeField] GameObject betBarPos2;             // Bet bar position when hidden

    #endregion

    #region Runtime Fields

    public List<GameObject> chipsBtnsObjects;           // List of instantiated bet button objects
    CardsData playerCardsData;                          // Reference to the player's CardsData

    #endregion

    #region Unity Methods

    void Start()
    {
        OnResettingBet();
    }

    #endregion

    #region Bet Button Creation

    /// <summary>
    /// Creates bet buttons for each available bet amount, based on player's balance.
    /// </summary>
    public void CreateBetButtons()
    {
        chipsBtnsObjects = refMgr.gameManager.ClearList(chipsBtnsObjects);
        for (int i = 0; i < betAmounts.Length; i++)
        {
            if (refMgr.potHandler.IsHaveAmount(betAmounts[i].amount * 10) || i == 0)
            {
                GameObject btn = Instantiate(betBtnPrefab);
                btn.SetActive(true);
                chipsBtnsObjects.Add(btn);
                btn.GetComponent<Image>().sprite = betAmounts[i].coinIcon;
                LocalSettingBlackJack.SetPositionAndRectTransform(btn, betBtnPrefab.GetComponent<RectTransform>(), betBtnPrefab.transform.parent);
                Button betBtn = btn.GetComponent<Button>();
                int amt = betAmounts[i].amount;
                btn.name = amt.ToString();
                betBtn.onClick.AddListener(() => PlaceBetBtnClick(amt));
                btn.transform.GetChild(0).GetComponent<TMP_Text>().text = amt.ToString();
            }
            else break;
        }
        if (chipsBtnsObjects.Count <= 0)
            refMgr.gameManager.ShowShopPanel();
    }

    #endregion

    #region Bet Bar UI

    /// <summary>
    /// Animates the bet bar to show or hide it.
    /// </summary>
    public void ShowBetbar(bool isShow)
    {
        GameObject targetpos = isShow ? betBarPos1 : betBarPos2;
        betBar.transform.DOMove(targetpos.transform.position, 0.3f)
            .SetEase(Ease.OutCubic);
    }

    #endregion

    #region Bet Placement and Chip Handling

    /// <summary>
    /// Called when a bet button is clicked. Places the bet and animates the chip.
    /// </summary>
    public void PlaceBetBtnClick(int betAmount)
    {
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.ButtonSound);
        if (!refMgr.potHandler.IsHaveAmount(betAmount))
        {
            refMgr.gameManager.shopPanel.SetActive(true);
            return;
        }
        if (!refMgr.potHandler.IsHaveAmount(refMgr.potHandler.GetPotAmount + betAmount))
        {
            refMgr.gameManager.shopPanel.SetActive(true);
            return;
        }
        refMgr.potHandler.PlaceBetAmount(betAmount);
        dealnClearBtnGroup.SetActive(true);
        placeYourBetMsg.SetActive(false);
        CloneChipBtn(EventSystem.current.currentSelectedGameObject);
    }

    /// <summary>
    /// Clones the chip button to the player's bet area and animates it.
    /// </summary>
    void CloneChipBtn(GameObject chip)
    {
        if (playerCardsData == null)
            playerCardsData = refMgr.GetCardData(HandType.HANDTYPE.PLAYERHAND);
        GameObject chipBtn = Instantiate(chip);

        playerCardsData.chipsList.Add(chipBtn);
        chipBtn.transform.GetChild(1).gameObject.SetActive(false);
        Button btn = chipBtn.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => WhenBettedChipBtnClick());
        LocalSettingBlackJack.SetPositionAndRectTransform(chipBtn, playerCardsData.chipsPosRect, playerCardsData.chipsPosRect.transform.parent);
        chipBtn.transform.position = chip.transform.position;
        playChipAnimation(chipBtn, playerCardsData.chipsPosRect.gameObject, true);
    }

    /// <summary>
    /// Called when a betted chip is clicked. Only active during the betting phase.
    /// </summary>
    void WhenBettedChipBtnClick()
    {
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.ButtonSound);
        if (refMgr.gameStateManager.GetCurrentGameState() != GameState.State.CHOOSINGBET)
            return;
        ShowBetbar(true);
        dealnClearBtnGroup.gameObject.SetActive(true);
    }

    #endregion

    #region Deal and Clear Logic

    /// <summary>
    /// Called when the deal button is clicked. Finalizes the bet and starts the game.
    /// </summary>
    public void DealBtnClick()
    {
        if (!refMgr.potHandler.IsHaveAmount(refMgr.potHandler.GetPotAmount))
        {
            refMgr.gameManager.shopPanel.SetActive(true);
            ClearBtnClick();
            return;
        }
        ShowBetbar(false);
        dealnClearBtnGroup.gameObject.SetActive(false);
        refMgr.gameStateManager.UpDateGameState(GameState.State.CARDDROP);
        refMgr.potHandler.BetAmountDeduction(refMgr.potHandler.GetPotAmount);
        LocalSettingBlackJack.TotalGamesPlayed++;
    }

    /// <summary>
    /// Called when the clear button is clicked. Resets the bet.
    /// </summary>
    public void ClearBtnClick()
    {
        OnResettingBet();
    }

    /// <summary>
    /// Resets the bet UI and chips to the initial state.
    /// </summary>
    public void OnResettingBet()
    {
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.clearAllChips);
        refMgr.potHandler.ResetTotalBet();
        placeYourBetMsg.SetActive(true);
        dealnClearBtnGroup.SetActive(false);
        ShowBetbar(true);
        if (playerCardsData == null)
            playerCardsData = refMgr.GetCardData(HandType.HANDTYPE.PLAYERHAND);
        playerCardsData.ClearAllChips();
    }

    #endregion

    #region Chip Animation

    /// <summary>
    /// Animates a chip to its target position, with optional offset for stacking.
    /// </summary>
    void playChipAnimation(GameObject ObjectToAnimate, GameObject targetObj, bool isOffSetUse)
    {
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.chipBeting);
        GameObject TgtObj = targetObj;
        GameObject chip = ObjectToAnimate;
        chip.transform.SetParent(TgtObj.transform.parent.transform);
        if (isOffSetUse)
        {
            Vector3 tgtPos = TgtObj.transform.position + new Vector3(UnityEngine.Random.Range(-15, 16), playerCardsData.chipsList.Count * 1.5f, 0);
            ObjectToAnimate.transform.DOMove(tgtPos, 0.25f);
        }
        else
            ObjectToAnimate.transform.DOMove(TgtObj.transform.position, 0.25f);
        ObjectToAnimate.transform.DORotateQuaternion(targetObj.transform.rotation, 0.25f);
    }

    #endregion

    #region Utility

    /// <summary>
    /// Resets the bet bar and sets the bet to zero.
    /// </summary>
    public void ResetThings()
    {
        dealnClearBtnGroup.SetActive(true);
        refMgr.potHandler.PlaceBetAmount(0);
    }

    #endregion
}
/// <summary>
/// Serializable class for bet amount and its associated coin icon.
/// </summary>
[Serializable]
public class BetAmounts
{
    public int amount;         // The bet amount value
    public Sprite coinIcon;    // The icon representing this bet amount
}
