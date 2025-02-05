using System;
using UnityEngine;
using UnityEngine.UI;

public class Purchase : MonoBehaviour
{
    public Button[] buttons;
    public IAPManager iapManager;
    public ProductIDs[] productIDs;

    [SerializeField] MenuController menuController;

    void Start()
    {
        //for (int i = 0; i < buttons.Length; i++)
        //{
        //    int index = i;
        //    buttons[i].onClick.AddListener(() => OnButtonClick(index));
        //}
        for (int i = 0; i < productIDs.Length; i++)
        {
            productIDs[i].purchaseBtn.onClick.AddListener(() => OnButtonClick(i));
        }
    }


    int currentIndex;
    public void OnButtonClick(int buttonIndex)
    {
        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ButtonSound);

        Debug.Log("Product: " + productIDs[buttonIndex].rewardAmount + "    This item has been purchased");

        currentIndex = buttonIndex;
        iapManager.OnPurchaseSuccess = AddAmount;
        iapManager.BuyMyProduct(productIDs[currentIndex].productID);

    }

    public void AddAmount()
    {
        LocalSetting.SetTotalCash(LocalSetting.GetTotalCash() + productIDs[currentIndex].rewardAmount);
        menuController.UpDateTotalChipsTxts();
        iapManager.OnPurchaseSuccess = null;

        // Purchase success message show here
    }

}

[Serializable]
public class ProductIDs
{
    public string productID;
    public int rewardAmount;
    public Button purchaseBtn;
}
