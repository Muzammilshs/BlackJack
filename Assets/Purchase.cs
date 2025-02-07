using System;
using UnityEngine;
using UnityEngine.UI;

public class Purchase : MonoBehaviour
{
    public Button[] buttons;
    public ProductIDs[] productIDs;

    //public IAPManager iapManager;
    [SerializeField] MenuController menuController;

    void Start()
    {
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
        LocalSetting.SetTotalCash(LocalSetting.GetTotalCash() + productIDs[index].rewardAmount);
        menuController.UpDateTotalChipsTxts();
        Debug.LogError("Added Amount: " + productIDs[index].rewardAmount + "      Index is: " + index);
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
