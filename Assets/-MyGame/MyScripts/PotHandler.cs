using DG.Tweening;
using TMPro;
using UnityEngine;

public class PotHandler : MonoBehaviour
{

    [SerializeField] Rm refMgr;
    public TMP_Text totalAmountTxt;
    public TMP_Text totalbetPlacedTxt;
    public TMP_Text doubleBetPlacedTxt;
    int totalBetPlaced;
    int doubleBetPlaced;
    [ShowOnly] public Vector2 totalBetPlacedTxtPos;
    void Start()
    {
        totalBetPlacedTxtPos = totalbetPlacedTxt.transform.position;
        totalAmountTxt.text = LocalSetting.GetTotalCash().ToString("N0");
        totalbetPlacedTxt.gameObject.SetActive(false);
    }

    public void ResetTotalBet()
    {
        Debug.LogError("Resetting double bet items");
        totalbetPlacedTxt.gameObject.SetActive(false);
        totalBetPlaced = 0;
        totalbetPlacedTxt.text = totalBetPlaced.ToString();
        totalbetPlacedTxt.transform.position = totalBetPlacedTxtPos;
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
        BetAmountDeduction(doubleBetPlaced);
        totalbetPlacedTxt.transform.position = totalBetPlacedTxtPos + new Vector2(-100, 0);
        doubleBetPlacedTxt.transform.position = totalBetPlacedTxtPos + new Vector2(100, 0);
        refMgr.betBarHandler.DoubleBetChipsCreation();
    }

    public bool IsHaveAmount(int amount)
    {
        return amount <= LocalSetting.GetTotalCash() ? true : false;
    }
    public int GetPotAmount { get { return totalBetPlaced; } }
    public int GetDoubleBetAmount { get { return doubleBetPlaced; } }



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
    }
    void UpdateCashAmount(int currentCash, int targetAmount, float duration = 1f)
    {
        int startAmount = currentCash;

        DOTween.To(() => startAmount, x => startAmount = x, targetAmount, duration)
            .OnUpdate(() =>
            {
                totalAmountTxt.text = startAmount.ToString("N0");
            })
            .OnComplete(() =>
            {
                currentCash = targetAmount;
            });
    }
    #endregion

    #endregion
}
