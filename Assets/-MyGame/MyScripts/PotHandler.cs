using DG.Tweening;
using TMPro;
using UnityEngine;

public class PotHandler : MonoBehaviour
{

    [SerializeField] Rm refMgr;
    public TMP_Text totalAmountTxt;
    public TMP_Text totalbetPlacedTxt;
    int totalBetPlaced;
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

    #region Total Cash update
    public void UpDateTotalCash(int amount, bool isReward)
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
}
