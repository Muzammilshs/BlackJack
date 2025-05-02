using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class Purchase : MonoBehaviour
{
    public GameObject inAppBtn;
    public ProductIDs[] productIDs;
    IAPManager iapManager;
    //public IAPManager iapManager;
    [SerializeField] MenuController menuController;

    void Start()
    {

        iapManager = GetComponent<IAPManager>();
        Dictionary<float, float> dict = LocalSetting.dict;
        for (int i = 0; i < dict.Count; i++)
        {
            GameObject btn = Instantiate(inAppBtn.gameObject, inAppBtn.transform.parent);
            InAppButtonVariables btnThings = btn.GetComponent<InAppButtonVariables>();
            btnThings.coinsQuantity.text = dict.Keys.ElementAt(i).ToString();
            btnThings.price.text = dict.Values.ElementAt(i).ToString() + " $";
            btnThings.IAPBtn.onClick.AddListener(() =>
            {
                iapManager.BuyProductID(LocalSetting.coins + btnThings.coinsQuantity.text);
            });
            btn.name = btnThings.price.text;
            btn.SetActive(true);
        }

        //for (int i = 0; i < productIDs.Length; i++)
        //{
        //    int index = i;
        //    productIDs[i].purchaseBtn.onClick.AddListener(() => OnButtonClick(index));
        //    //iapManager.InitializePurchasing(productIDs[i].productID);
        //}
    }


    int currentIndex;
    public void OnButtonClick(int buttonIndex)
    {
        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound);

        currentIndex = buttonIndex;
        //iapManager.OnPurchaseSuccess = AddAmount;
        //iapManager.BuyMyProduct(productIDs[currentIndex].productID);

    }

    public void AddAmount(int index)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            LocalSetting.SetTotalCash(LocalSetting.GetTotalCash() + productIDs[index].rewardAmount);
            menuController.UpDateTotalChipsTxts();
        }
        else
            Rm.Instance.potHandler.CollectReward(productIDs[index].rewardAmount);
        Debug.LogError("Added Amount: " + productIDs[index].rewardAmount + "      Index is: " + index);
        //iapManager.OnPurchaseSuccess = null;

        // Purchase success message show here
    }

    public void AddIAPAmount(int coins)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            LocalSetting.SetTotalCash(LocalSetting.GetTotalCash() + coins);
            menuController.UpDateTotalChipsTxts();
        }
        else
            Rm.Instance.potHandler.CollectReward(coins);
        Debug.LogError("Added Amount: " + coins + "      Index is: " + coins);
        //iapManager.OnPurchaseSuccess = null;

        // Purchase success message show here
    }

}

[Serializable]
public class ProductIDs
{
    public string productID;
    public int rewardAmount;
    public Button purchaseBtn;
    public TMPro.TMP_Text priceTxt;
}
