using UnityEngine;
using UnityEngine.Purchasing;
using System;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public Purchase purchaseScript;
    private static IStoreController storeController;

    public Action OnPurchaseSuccess;

    void Start()
    {
        if (storeController == null)
        {
            for (int i = 0; i < purchaseScript.productIDs.Length; i++)
            {
                InitializePurchasing(purchaseScript.productIDs[i].productID);
                purchaseScript.productIDs[i].priceTxt.text = GetProductPrice(purchaseScript.productIDs[i].productID);
            }
        }
    }

    public void InitializePurchasing(string productID)
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add product IDs (Replace "your_product_id" with actual product ID)
        builder.AddProduct(productID, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    // Called when Unity IAP is initialized
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        Debug.Log("Unity IAP Initialized");

        // Fetch and display the price of a product
        string price = GetProductPrice(PRODUCT_ID);
        Debug.Log("Product Price: " + price);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAP Init Failed: {error}");
    }


    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAP Init Failed: {error}, {message}");
    }

    // Define your product IDs (these must match with Play Store / App Store)
    private const string PRODUCT_ID = "your_product_id"; // Replace with actual product ID

    // Function to get price
    public string GetProductPrice(string productId)
    {
        if (storeController != null)
        {
            Product product = storeController.products.WithID(productId);
            if (product != null && product.hasReceipt)
            {
                return product.metadata.localizedPriceString; // Returns localized price string (e.g., "$4.99")
            }
        }
        return "Price Not Available";
    }

    // Purchase a product
    public void BuyProduct()
    {
        if (storeController != null && storeController.products.WithID(PRODUCT_ID).availableToPurchase)
        {
            Debug.Log($"🛒 Purchasing: {PRODUCT_ID}");
            storeController.InitiatePurchase(PRODUCT_ID);
        }
        else
        {
            Debug.Log("❌ Product not available for purchase.");
        }
    }

    public void BuyMyProduct(string productID)
    {
        //if (storeController != null /*&& storeController.products.WithID(productID).availableToPurchase*/)
        //{
        //    Debug.Log($"🛒 Purchasing start: {productID}");
        //    storeController.InitiatePurchase(productID);
        //}
        //else
        //{
        //    Debug.Log("❌ Product not available for purchase.");
        //}
        if (storeController != null)
        {
            Product product = storeController.products.WithID(productID);
            if (product != null /*&& product.availableToPurchase*/)
            {
                Debug.Log($"🛒 Purchasing start: {productID}");
                storeController.InitiatePurchase(productID);
            }
            else
            {
                Debug.Log($"❌ Product not available for purchase: {productID}");
            }
        }
        else
        {
            Debug.Log("❌ StoreController is not initialized.");
        }
    }
    // Called when a purchase is completed
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.LogError($"whoooo Purchase Successful  {args.purchasedProduct.definition.id}");
        OnPurchaseSuccess?.Invoke();
        return PurchaseProcessingResult.Complete;
    }

    // Called when a purchase fails
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Purchase failed: {product.definition.id}, Reason: {failureReason}");
    }

}



//using UnityEngine;
//using UnityEngine.Purchasing;
//using System.Collections.Generic;
//using System;

//public class IAPManager : MonoBehaviour, IStoreListener
//{
//    public Purchase purchaseScript;
//    private static IStoreController storeController;

//    public Action OnPurchaseSuccess;
//    void Start()
//    {
//        //if (storeController == null)
//        //{
//        //    InitializePurchasing();
//        //}
//    }

//    //public void InitializePurchasing()
//    //{
//    //    var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

//    //    // Add product IDs (Replace "your_product_id" with actual product ID)
//    //    builder.AddProduct("your_product_id", ProductType.Consumable);

//    //    UnityPurchasing.Initialize(this, builder);
//    //}
//    public void InitializePurchasing(string productID)
//    {
//        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

//        // Add product IDs (Replace "your_product_id" with actual product ID)
//        builder.AddProduct(productID, ProductType.Consumable);

//        UnityPurchasing.Initialize(this, builder);
//    }

//    // Called when Unity IAP is initialized
//    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//    {
//        storeController = controller;
//        Debug.Log("Unity IAP Initialized");

//        // Fetch and display the price of a product
//        string price = GetProductPrice("your_product_id");
//        Debug.Log("Product Price: " + price);
//    }

//    public void OnInitializeFailed(InitializationFailureReason error)
//    {
//        Debug.LogError($"IAP Init Failed: {error}");
//    }
//    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
//    {
//        Debug.LogError($"Purchase failed: {product.definition.id}, Reason: {failureReason}");
//    }
//    public void OnInitializeFailed(InitializationFailureReason error, string message)
//    {
//        Debug.LogError($"IAP Init Failed: {error}, {message}");
//    }



//    // Define your product IDs (these must match with Play Store / App Store)
//    private const string PRODUCT_ID = "your_product_id"; // Replace with actual product ID

//    // Function to get price
//    public string GetProductPrice(string productId)
//    {
//        if (storeController != null)
//        {
//            Product product = storeController.products.WithID(productId);
//            if (product != null && product.hasReceipt)
//            {
//                return product.metadata.localizedPriceString; // Returns localized price string (e.g., "$4.99")
//            }
//        }
//        return "Price Not Available";
//    }

//    // Purchase a product
//    public void BuyProduct()
//    {
//        if (storeController != null && storeController.products.WithID(PRODUCT_ID).availableToPurchase)
//        {
//            Debug.Log($"🛒 Purchasing: {PRODUCT_ID}");
//            storeController.InitiatePurchase(PRODUCT_ID);
//        }
//        else
//        {
//            Debug.Log("❌ Product not available for purchase.");
//        }
//    }

//    public void BuyMyProduct(string productID)
//    {
//        //if (storeController != null && storeController.products.WithID(productID).availableToPurchase)
//        {
//            Debug.Log($"🛒 Purchasing start: {productID}");
//            storeController.InitiatePurchase(productID);
//        }
//        //else
//        //{
//        //    Debug.Log("❌ Product not available for purchase.");
//        //}
//    }

//    // Called when a purchase is completed
//    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
//    {
//        Debug.LogError($"whoooo Purchase Successful  {args.purchasedProduct.definition.id}");
//        OnPurchaseSuccess?.Invoke();
//        //if (args.purchasedProduct.definition.id == PRODUCT_ID)
//        //{
//        //    Debug.Log("🎉 Purchase Successful! Granting item...");
//        //    PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + 100); // Example: Grant 100 Coins
//        //    PlayerPrefs.Save();
//        //}
//        return PurchaseProcessingResult.Complete;
//    }


//    //// Restore Purchases (for iOS)
//    //public void RestorePurchases()
//    //{
//    //    if (Application.platform == RuntimePlatform.IPhonePlayer)
//    //    {
//    //        var extension = storeController.extensions.GetExtension<IAppleExtensions>();
//    //        extension.RestoreTransactions((success) =>
//    //        {
//    //            Debug.Log("🔄 Restore Purchases: " + (success ? "Success" : "Failed"));
//    //        });
//    //    }
//    //}
//}
