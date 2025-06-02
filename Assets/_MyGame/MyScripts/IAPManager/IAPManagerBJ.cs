using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Linq;
using UnityEngine.SceneManagement;

public class IAPManagerBJ : MonoBehaviour, IStoreListener
{
    #region Fields

    // Unity IAP store controller
    private static IStoreController m_StoreController;
    // Unity IAP extension provider
    private static IExtensionProvider m_StoreExtensionProvider;

    // Reference to the Purchase script for handling coin addition
    Purchase purchaseScript;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the IAP system and Unity Services on start.
    /// </summary>
    async void Start()
    {
        purchaseScript = GetComponent<Purchase>();
        await UnityServices.InitializeAsync();

        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    #endregion

    #region IAP Initialization

    /// <summary>
    /// Configures and initializes Unity IAP with all coin products.
    /// </summary>
    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        Dictionary<float, float> dict = LocalSettingBlackJack.dict;

        // Add all coin products as consumables
        for (int i = 0; i < dict.Count; i++)
        {
            builder.AddProduct(LocalSettingBlackJack.coins + dict.Keys.ElementAt(i).ToString(), ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    #endregion

    #region IStoreListener Implementation

    /// <summary>
    /// Called when Unity IAP is successfully initialized.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    /// <summary>
    /// Called when Unity IAP fails to initialize (with message).
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("Billing failed to initialize! : " + error);
        Debug.Log(message);
    }

    /// <summary>
    /// Called when Unity IAP fails to initialize.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Billing failed to initialize! : " + error);
    }

    /// <summary>
    /// Initiates the purchase process for a given product ID.
    /// </summary>
    public void BuyProductID(string productId)
    {
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.ButtonSound);
        if (m_StoreController != null && m_StoreExtensionProvider != null)
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                m_StoreController.InitiatePurchase(product);
                // Additional product purchase logic can be added here
            }
            else
            {
                // Product not found or not available for purchase
            }
        }
    }

    /// <summary>
    /// Processes a successful purchase and updates the user's coin balance.
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.purchasedProduct.definition.id))
        {
            // Check if the purchased product is a coin pack
            if (args.purchasedProduct.definition.id.Contains(LocalSettingBlackJack.coins))
            {
                int quantity;
                quantity = int.Parse(args.purchasedProduct.definition.id.Split('_')[1]);
                purchaseScript.AddIAPAmount(quantity);
                Debug.LogError("Purchased: " + args.purchasedProduct.definition.id + "  Quantity: " + quantity);

                // Update UI and local values based on the current scene
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

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("Purchase of product failed: " + product.definition.id);
    }

    // Additional IStoreListener methods can be implemented here if needed.

    #endregion
}
