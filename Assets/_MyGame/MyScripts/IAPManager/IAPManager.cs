using System;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Linq;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    Purchase purchaseScript;

    //void Start()
    //{
    //    if (m_StoreController == null)
    //    {
    //        InitializePurchasing();
    //    }
    //}

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
        Dictionary<float, float> dict = LocalSetting.dict;

        for (int i = 0; i < dict.Count; i++)
        {
            builder.AddProduct(LocalSetting.coins + dict.Keys.ElementAt(i).ToString(), ProductType.Consumable);
        }

        //builder.AddProduct("stake_100", ProductType.Consumable);
        //builder.AddProduct("stake_1000", ProductType.Consumable);
        //builder.AddProduct("stake_10000", ProductType.Consumable);
        //builder.AddProduct("remove_ad", ProductType.Consumable);

        //builder.AddProduct("diamond_10000", ProductType.Consumable);
        //builder.AddProduct("diamond_20000", ProductType.Consumable);
        //builder.AddProduct("diamond_50000", ProductType.Consumable);
        // Add more products as needed

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
            //if (args.purchasedProduct.definition.id.Contains("diamond_"))
            //{
            //    int quantity;
            //    quantity = int.Parse(args.purchasedProduct.definition.id.Split('_')[1]);

            //    GameManager.Instance.BuyStake(quantity);
            //}
            if (args.purchasedProduct.definition.id.Contains(LocalSetting.coins))
            {
                int quantity;
                quantity = int.Parse(args.purchasedProduct.definition.id.Split('_')[1]);
                purchaseScript.AddIAPAmount(quantity);
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