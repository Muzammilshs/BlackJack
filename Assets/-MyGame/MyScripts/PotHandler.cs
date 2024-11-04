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
    void Start()
    {
        totalAmountTxt.text = LocalSetting.GetTotalCash().ToString("N0");
        totalbetPlacedTxt.gameObject.SetActive(false);
    }

    public void ResetTotalBet()
    {
        totalbetPlacedTxt.gameObject.SetActive(false);
        totalBetPlaced = 0;
        totalbetPlacedTxt.text = totalBetPlaced.ToString();
    }

    public void PlaceBetAmount(int amount)
    {
        totalbetPlacedTxt.gameObject.SetActive(true);
        totalBetPlaced += amount;
        totalbetPlacedTxt.text = totalBetPlaced.ToString();
    }


    public void PlaceDoubleBetAmount()
    {
        doubleBetPlaced = GetPotAmount;
        doubleBetPlacedTxt.text = doubleBetPlaced.ToString();
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
