using System;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Linq;
using UnityEngine.SceneManagement;

public class IAPManagerBJ : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    Purchase purchaseScript;



    async void Start()
    {
        purchaseScript = GetComponent<Purchase>();
        await UnityServices.InitializeAsync();

        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        Dictionary<float, float> dict = LocalSettingBlackJack.dict;

        for (int i = 0; i < dict.Count; i++)
        {
            builder.AddProduct(LocalSettingBlackJack.coins + dict.Keys.ElementAt(i).ToString(), ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("Billing failed to initialize! : " + error);
        Debug.Log(message);
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Billing failed to initialize! : " + error);
    }
    public void BuyProductID(string productId)
    {
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.ButtonSound);
        if (m_StoreController != null && m_StoreExtensionProvider != null)
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {


                m_StoreController.InitiatePurchase(product);
                // Handle other product purchases


            }
            else
            {
                // Product not found or not available for purchase
            }
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.purchasedProduct.definition.id))
        {
            
            if (args.purchasedProduct.definition.id.Contains(LocalSettingBlackJack.coins))
            {
                int quantity;
                quantity = int.Parse(args.purchasedProduct.definition.id.Split('_')[1]);
                purchaseScript.AddIAPAmount(quantity);
                Debug.LogError("Purchased: " + args.purchasedProduct.definition.id + "  Quantity: " + quantity);
                //LocalSettingBlackJack.SetTotalCashWithBetLocal(quantity, true);
                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    MenuController.instance.UpDateTotalChipsTxtsWithLocalValues();
                    LocalSettingBlackJack.SetTotalCash(quantity);
                }
                else
                {
                    Rm.Instance.potHandler.CollectReward(quantity);
                }
            }


        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("Purchase of product failed: " + product.definition.id);
    }
    // Implement other IStoreListener methods here...

}