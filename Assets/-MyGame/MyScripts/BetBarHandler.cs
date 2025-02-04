
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BetBarHandler : MonoBehaviour
{
    [SerializeField] Rm refMgr;
    [SerializeField] BetAmounts[] betAmounts;
    [SerializeField] GameObject betBar;
    [SerializeField] GameObject betBtnPrefab;
    [SerializeField] GameObject placeYourBetMsg;
    [SerializeField] GameObject clearBetBtn;
    [SerializeField] GameObject dealBtn;
    [SerializeField] GameObject dealnClearBtnGroup;
    [SerializeField] GameObject betBarPos1;
    [SerializeField] GameObject betBarPos2;
    [SerializeField] RectTransform bettedChipsPos;
    public RectTransform bettedChipsPos_1_Split;
    public RectTransform bettedChipsPos_2_Split;
    [SerializeField] RectTransform dealerBettedChipsPos;
    [SerializeField] RectTransform dealerBettedChipsPos_P1_Split;
    [SerializeField] RectTransform dealerBettedChipsPos_P2_Split;
    [SerializeField] RectTransform playerBettedChipsPos;
    [SerializeField] RectTransform playerBettedChipsPos_P1_Split;
    [SerializeField] RectTransform playerBettedChipsPos_P2_Split;

    public List<GameObject> chipsObjects;
    /*[HideInInspector] */
    public List<GameObject> betPlacedChips;
    /*[HideInInspector] */
    public List<GameObject> betPlacedChips_1_Split;
    /*[HideInInspector] */
    public List<GameObject> betPlacedChips_2_Split;
    List<GameObject> doubleBetPlacedChips;

    void Start()
    {
        OnResettingBet();
    }

    public void CreateBetButtons()
    {
        chipsObjects = refMgr.gameManager.ClearList(chipsObjects);
        for (int i = 0; i < betAmounts.Length; i++)
        {
            if (refMgr.potHandler.IsHaveAmount(betAmounts[i].amount * 10) || i == 0)
            {
                GameObject btn = Instantiate(betBtnPrefab);
                btn.SetActive(true);
                chipsObjects.Add(btn);
                btn.GetComponent<Image>().sprite = betAmounts[i].coinIcon;
                LocalSetting.SetPosAndRect(btn, betBtnPrefab.GetComponent<RectTransform>(), betBtnPrefab.transform.parent);
                Button betBtn = btn.GetComponent<Button>();
                int amt = betAmounts[i].amount;
                btn.name = amt.ToString();
                betBtn.onClick.AddListener(() => PlaceBetBtnClick(amt));
                btn.transform.GetChild(0).GetComponent<TMP_Text>().text = amt.ToString();
            }
            else break;
        }
        if (chipsObjects.Count <= 0)
            refMgr.gameManager.ShowShopPanel();
    }
    public void ShowBetbar(bool isShow)
    {
        GameObject targetpos = isShow ? betBarPos1 : betBarPos2;
        betBar.transform.DOMove(targetpos.transform.position, 0.3f)
            .SetEase(Ease.OutCubic);
    }
    #region Bet place button click
    public void PlaceBetBtnClick(int betAmount)
    {
        Debug.LogError("bet amount: " + betAmount);
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
    void CloneChipBtn(GameObject chip)
    {
        GameObject chipBtn = Instantiate(chip);
        betPlacedChips.Add(chipBtn);
        chipBtn.transform.GetChild(1).gameObject.SetActive(false);
        Button btn = chipBtn.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => WhenBettedChipBtnClick());
        LocalSetting.SetPosAndRect(chipBtn, bettedChipsPos, bettedChipsPos.transform.parent);
        chipBtn.transform.position = chip.transform.position;
        playChipAnimation(chipBtn, bettedChipsPos.gameObject, true);
    }
    public void DoubleBetChipsCreation()
    {
        int totalChipsToCreate = betPlacedChips.Count;
        foreach (GameObject chip in betPlacedChips)
            chip.transform.position += new Vector3(-100, 0, 0);
        for (int i = 0; i < totalChipsToCreate; i++)
        {
            GameObject chipBtn = Instantiate(betPlacedChips[i]);
            doubleBetPlacedChips.Add(chipBtn);
            LocalSetting.SetPosAndRect(chipBtn, betPlacedChips[i].GetComponent<RectTransform>(), bettedChipsPos.transform.parent);
            chipBtn.transform.position += new Vector3(100 * 2, 0, 0);
        }

    }
    #endregion
    void WhenBettedChipBtnClick()
    {
        if (refMgr.gameStateManager.GetCurrentGameState() != GameState.State.CHOOSINGBET)
            return;
        ShowBetbar(true);
        dealnClearBtnGroup.gameObject.SetActive(true);

    }
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
    }

    public void ClearBtnClick()
    {
        OnResettingBet();
    }


    public void OnResettingBet()
    {
        refMgr.potHandler.ResetTotalBet();
        placeYourBetMsg.SetActive(true);
        dealnClearBtnGroup.SetActive(false);
        ShowBetbar(true);
        betPlacedChips = refMgr.gameManager.ClearList(betPlacedChips);
        if (doubleBetPlacedChips != null)
        {
            if (doubleBetPlacedChips.Count > 0)
                foreach (GameObject chip in betPlacedChips)
                    chip.transform.position = new Vector3(chip.transform.position.x + 100, chip.transform.position.y, 0);
        }
        doubleBetPlacedChips = refMgr.gameManager.ClearList(doubleBetPlacedChips);

    }

    void playChipAnimation(GameObject ObjectToAnimate, GameObject targetObj, bool isOffSetUse)
    {
        GameObject TgtObj = targetObj;
        GameObject chip = ObjectToAnimate;
        chip.transform.SetParent(TgtObj.transform.parent.transform);
        if (isOffSetUse)
        {
            Vector3 tgtPos = TgtObj.transform.position + new Vector3(UnityEngine.Random.Range(-10, 11), betPlacedChips.Count * 1.5f, 0);
            ObjectToAnimate.transform.DOMove(tgtPos, 0.25f);
        }
        else
            ObjectToAnimate.transform.DOMove(TgtObj.transform.position, 0.25f);
        ObjectToAnimate.transform.DORotateQuaternion(targetObj.transform.rotation, 0.25f);

    }

    public void ResetThings()
    {
        dealnClearBtnGroup.SetActive(true);
        if (doubleBetPlacedChips.Count > 0)
        {
            foreach (GameObject obj in doubleBetPlacedChips)
            {
                Destroy(obj);
            }
            doubleBetPlacedChips.Clear();
            refMgr.potHandler.doubleBetPlacedTxt.gameObject.SetActive(false);
            foreach (GameObject chip in betPlacedChips)
            {
                chip.transform.position = new Vector3(chip.transform.position.x + 100, chip.transform.position.y, 0);
            }
            Transform txt = refMgr.potHandler.totalbetPlacedTxt.transform;
            txt.position = new Vector3(txt.position.x + 100, txt.position.y, 0);
        }

        for (int i = 0; i < betPlacedChips_2_Split.Count; i++)
            if (betPlacedChips_2_Split[i] != null)
                Destroy(betPlacedChips_2_Split[i]);
        for (int i = 0; i < betPlacedChips_1_Split.Count; i++)
        {
            betPlacedChips.Add(betPlacedChips_1_Split[i]);
            LocalSetting.SetPosAndRect(betPlacedChips[i], bettedChipsPos, bettedChipsPos.transform.parent);
        }
        betPlacedChips_1_Split.Clear();
        betPlacedChips_2_Split.Clear();
        refMgr.potHandler.PlaceBetAmount(0);
    }

    public void BettedPos(out RectTransform playerChipsPos, out RectTransform dealerChipsPos)
    {
        playerChipsPos = playerBettedChipsPos;
        dealerChipsPos = dealerBettedChipsPos;
    }
    public void BettedPosSplit(out RectTransform playerChipsPos, out RectTransform dealerChipsPos, int splitPart)
    {
        if (splitPart == 0)
        {
            playerChipsPos = playerBettedChipsPos_P1_Split;
            dealerChipsPos = dealerBettedChipsPos_P1_Split;
        }
        else
        {
            playerChipsPos = playerBettedChipsPos_P2_Split;
            dealerChipsPos = dealerBettedChipsPos_P2_Split;
        }
    }

    public void DuplicateBettedChipsAndAmountOnSplit()
    {
        float rndRange = 3f;
        for (int i = 0; i < betPlacedChips.Count; i++)
        {
            betPlacedChips_1_Split.Add(betPlacedChips[i]);
            float yPos = betPlacedChips_1_Split[i].GetComponent<RectTransform>().position.y;
            betPlacedChips_1_Split[i].GetComponent<RectTransform>().position = new Vector2(bettedChipsPos_1_Split.position.x + UnityEngine.Random.Range(-rndRange, rndRange), yPos + UnityEngine.Random.Range(-rndRange, rndRange));

            // for second
            GameObject chip = Instantiate(betPlacedChips_1_Split[i]);
            betPlacedChips_2_Split.Add(chip);
            LocalSetting.SetPosAndRect(chip, bettedChipsPos_2_Split, bettedChipsPos_2_Split.parent);
            chip.GetComponent<RectTransform>().position = new Vector2(bettedChipsPos_2_Split.position.x + UnityEngine.Random.Range(-rndRange, rndRange), yPos + UnityEngine.Random.Range(-rndRange, rndRange));
        }
        betPlacedChips.Clear();
        refMgr.potHandler.SetBetAmountForSplit();
    }

}
[Serializable]
public class BetAmounts
{
    public int amount;
    public Sprite coinIcon;
}