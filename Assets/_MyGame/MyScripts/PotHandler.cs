using DG.Tweening;
using TMPro;
using UnityEngine;

public class PotHandler : MonoBehaviour
{

    [SerializeField] Rm refMgr;
    public TMP_Text[] totalAmountTxt;
    public TMP_Text totalbetPlacedTxt;
    public TMP_Text doubleBetPlacedTxt;

    public RectTransform totalbetPlacedTxt_Pos;
    public TMP_Text totalbetPlacedTxt_P1_split;
    public TMP_Text totalbetPlacedTxt_P2_split;
    int totalBetPlaced;
    int totalBetPlaced_1_Split;
    int totalBetPlaced_2_Split;

    int doubleBetPlaced;
    void Start()
    {
        UpDateCashTxt(LocalSetting.GetTotalCash());
        totalbetPlacedTxt.gameObject.SetActive(false);
        totalbetPlacedTxt_P1_split.gameObject.SetActive(false);
        totalbetPlacedTxt_P2_split.gameObject.SetActive(false);
    }

    public void ResetTotalBet()
    {
        //Debug.LogError("Resetting double bet items");
        totalbetPlacedTxt.gameObject.SetActive(false);
        totalBetPlaced = 0;
        totalbetPlacedTxt.text = totalBetPlaced.ToString();
        totalbetPlacedTxt.transform.position = totalbetPlacedTxt_Pos.transform.position;
        doubleBetPlacedTxt.gameObject.SetActive(false);
    }

    public void PlaceBetAmount(int amount)
    {
        if (!IsHaveAmount(amount))
        {
            refMgr.gameManager.shopPanel.SetActive(true);
            return;
        }
        totalbetPlacedTxt.gameObject.SetActive(true);
        totalbetPlacedTxt_P1_split.gameObject.SetActive(false);
        totalbetPlacedTxt_P2_split.gameObject.SetActive(false);
        totalBetPlaced += amount;
        totalbetPlacedTxt.text = totalBetPlaced.ToString();
    }


    public void PlaceDoubleBetAmount()
    {
        if (!IsHaveAmount(GetPotAmount))
        {
            refMgr.gameManager.shopPanel.SetActive(true);
            return;
        }
        doubleBetPlaced = GetPotAmount;
        doubleBetPlacedTxt.text = doubleBetPlaced.ToString();
        doubleBetPlacedTxt.gameObject.SetActive(true);
        //totalBetPlaced *= 2;
        BetAmountDeduction(doubleBetPlaced);
        totalbetPlacedTxt.transform.position = totalbetPlacedTxt_Pos.position + new Vector3(-100, 0, 0);
        doubleBetPlacedTxt.transform.position = totalbetPlacedTxt_Pos.position + new Vector3(100, 0, 0);
        refMgr.betBarHandler.DoubleBetChipsCreation();
    }

    public bool IsHaveAmount(int amount)
    {
        return amount <= LocalSetting.GetTotalCash() ? true : false;
    }
    public int GetPotAmount { get { return totalBetPlaced; } }
    public int GetDoubleBetAmount { get { return doubleBetPlaced; } }

    public void SetBetAmountForSplit()
    {
        totalBetPlaced_1_Split = totalBetPlaced;
        totalBetPlaced_2_Split = totalBetPlaced;

        totalbetPlacedTxt_P1_split.text = totalBetPlaced.ToString();
        totalbetPlacedTxt_P2_split.text = totalBetPlaced.ToString();

        totalbetPlacedTxt_P1_split.gameObject.SetActive(true);
        totalbetPlacedTxt_P2_split.gameObject.SetActive(true);

        totalbetPlacedTxt.gameObject.SetActive(false);
    }

    #region Total Cash update

    public void BetAmountDeduction(int amount)
    {
        UpDateTotalCash(amount, false);
    }
    public void CollectReward(int amount)
    {
        UpDateTotalCash(amount, true);
    }

    #region Cash add/deduct animation 
    void UpDateTotalCash(int amount, bool isReward)
    {
        int prevAmount = 0;
        prevAmount = LocalSetting.GetTotalCash();
        if (!isReward)
            amount = -amount;
        LocalSetting.SetTotalCash(amount);
        UpdateCashAmount(prevAmount, LocalSetting.GetTotalCash());

        Debug.Log("Setting Local Cash " + amount);
    }
    void UpdateCashAmount(int currentCash, int targetAmount, float duration = 1f)
    {
        int startAmount = currentCash;

        DOTween.To(() => startAmount, x => startAmount = x, targetAmount, duration)
            .OnUpdate(() =>
            {
                //foreach (var amountTxt in totalAmountTxt)
                //    amountTxt.text = startAmount.ToString("N0");
                UpDateCashTxt(startAmount);
            })
            .OnComplete(() =>
            {
                currentCash = targetAmount;
            });
    }
    public void UpDateCashTxt(int amount)
    {
        foreach (var amountTxt in totalAmountTxt)
            amountTxt.text = amount.ToString("N0");
        Debug.Log(amount);
    }
    #endregion

    #endregion
}
