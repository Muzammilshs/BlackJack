
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

    List<GameObject> chipsObjects;
    List<GameObject> betPlacedChips;
    void Start()
    {
        OnResettingBet();
    }

    public void CreateBetButtons()
    {
        chipsObjects = ClearList(chipsObjects);
        for (int i = 0; i < betAmounts.Length; i++)
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

    #endregion
    void WhenBettedChipBtnClick()
    {
        ShowBetbar(true);
        dealnClearBtnGroup.gameObject.SetActive(true);

    }
    public void DealBtnClick()
    {
        ShowBetbar(false);
        dealnClearBtnGroup.gameObject.SetActive(false);
        refMgr.gameStateManager.UpDateGameState(GameState.State.CARDDROP);
    }

    public void ClearBtnClick()
    {
        OnResettingBet();
    }
    List<GameObject> ClearList(List<GameObject> list)
    {
        if (list == null)
        {
            list = new List<GameObject>();
            return list;
        }
        if (list.Count > 0)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                Destroy(list[i]);
            list.Clear();
        }
        return list;
    }

    public void OnResettingBet()
    {
        refMgr.potHandler.ResetTotalBet();
        placeYourBetMsg.SetActive(true);
        dealnClearBtnGroup.SetActive(false);
        ShowBetbar(true);
        betPlacedChips = ClearList(betPlacedChips);
    }

    void playChipAnimation(GameObject ObjectToAnimate, GameObject targetObj, bool isOffSetUse)
    {
        GameObject TgtObj = targetObj;
        GameObject chip = ObjectToAnimate;
        chip.transform.parent = TgtObj.transform.parent.transform;
        if (isOffSetUse)
        {
            Vector3 tgtPos = TgtObj.transform.position + new Vector3(UnityEngine.Random.Range(-10, 11), betPlacedChips.Count * 1.5f, 0);
            ObjectToAnimate.transform.DOMove(tgtPos, 0.25f);
        }
        else
            ObjectToAnimate.transform.DOMove(TgtObj.transform.position, 0.25f);
        ObjectToAnimate.transform.DORotateQuaternion(targetObj.transform.rotation, 0.25f);

    }
}
[Serializable]
public class BetAmounts
{
    public int amount;
    public Sprite coinIcon;
}