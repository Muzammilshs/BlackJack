using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField] TMP_Text _insuranceAmountTxt;

    int _insuranceBetAmount;
    public int GetInsuranceAmount { get { return _insuranceBetAmount; } }

    //int doubleBetPlaced;
    void Start()
    {
        UpDateCashTxt(LocalSettingBlackJack.GetTotalCash());
        totalbetPlacedTxt.gameObject.SetActive(false);
        totalbetPlacedTxt_P1_split.gameObject.SetActive(false);
        totalbetPlacedTxt_P2_split.gameObject.SetActive(false);
    }

    public void ResetTotalBet()
    {
        //Debug.LogError("Resetting double bet items");
        //totalbetPlacedTxt.gameObject.SetActive(false);
        totalBetPlaced = 0;
        refMgr.GetCardData(HandType.HANDTYPE.PLAYERHAND).SetBettedAmount("");
        //totalbetPlacedTxt.text = totalBetPlaced.ToString();
        //totalbetPlacedTxt.transform.position = totalbetPlacedTxt_Pos.transform.position;
        doubleBetPlacedTxt.gameObject.SetActive(false);
    }

    public void ResetInsuranceAmount()
    {
        _insuranceBetAmount = 0;
        _insuranceAmountTxt.transform.parent.gameObject.SetActive(false);
    }
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


    public void PlaceDoubleBetAmount()
    {
        return;
        //if (!IsHaveAmount(GetPotAmount))
        //{
        //    refMgr.gameManager.shopPanel.SetActive(true);
        //    return;
        //}
        //doubleBetPlaced = GetPotAmount;
        //doubleBetPlacedTxt.text = doubleBetPlaced.ToString();
        //doubleBetPlacedTxt.gameObject.SetActive(true);
        ////totalBetPlaced *= 2;
        //BetAmountDeduction(doubleBetPlaced);
        //totalbetPlacedTxt.transform.position = totalbetPlacedTxt_Pos.position + new Vector3(-100, 0, 0);
        //doubleBetPlacedTxt.transform.position = totalbetPlacedTxt_Pos.position + new Vector3(100, 0, 0);
        //refMgr.betBarHandler.DoubleBetChipsCreation();
    }

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

    public bool IsHaveAmount(int amount)
    {
        return amount <= LocalSettingBlackJack.GetTotalCash() ? true : false;
    }
    public int GetPotAmount { get { return totalBetPlaced; } }

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
        //UpDateCashTexts();
        int prevAmount = 0;
        prevAmount = LocalSettingBlackJack.GetTotalCash();
        LocalSettingBlackJack.SetTotalCashWithBetLocal(amount, isReward);
        if (!isReward)
            amount = -amount;

        LocalSettingBlackJack.SetTotalCash(amount);
        UpdateCashAmount(prevAmount, LocalSettingBlackJack.GetTotalCashLocal());
    }

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
    public void UpDateCashTxt(int amount)
    {
        foreach (var amountTxt in totalAmountTxt)
            amountTxt.text = amount.ToString("N0");
    }
    #endregion

    #endregion
}
