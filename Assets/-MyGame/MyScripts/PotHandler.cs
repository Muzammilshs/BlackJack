using DG.Tweening;
using TMPro;
using UnityEngine;

public class PotHandler : MonoBehaviour
{

    public TMP_Text totalAmountTxt;
    void Start()
    {
        totalAmountTxt.text = LocalSetting.GetTotalCash().ToString("N0");
    }

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
}
