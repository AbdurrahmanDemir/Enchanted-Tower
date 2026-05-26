using UnityEngine;
using UnityEngine.Purchasing;
using System;
using Product = UnityEngine.Purchasing.Product;

public class IAPCore : MonoBehaviour, IStoreListener
{
    public static IAPCore Instance { get; private set; }

    private IStoreController controller;
    public string[] productIds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("IAPCore: Initialized and persisted across scenes");
        
        InitializeIAP();
    }

    public void InitializeIAP()
    {
        if (controller != null)
        {
            Debug.Log("IAPCore: Already initialized");
            return;
        }

        Debug.Log("IAPCore: Starting IAP initialization...");
        var module = StandardPurchasingModule.Instance();
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

        foreach (string productId in productIds)
        {
            builder.AddProduct(productId, ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        Debug.Log("IAPCore: IAP initialized successfully!");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAPCore: Initialization failed: {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAPCore: Initialization failed: {error} - {message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"IAPCore: Purchase failed: {failureReason}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        string productId = e.purchasedProduct.definition.id;
        Debug.Log($"IAPCore: Processing purchase: {productId}");

        // IAPUIManager'ı bul ve satın alma işlemini ona devret
        IAPUIManager uiManager = FindObjectOfType<IAPUIManager>();
        if (uiManager != null)
        {
            return uiManager.HandlePurchase(e, productIds);
        }
        else
        {
            Debug.LogError("IAPCore: IAPUIManager not found in scene!");
            return PurchaseProcessingResult.Pending;
        }
    }

    public void PurchaseProduct(string productId)
    {
        if (controller == null)
        {
            Debug.LogWarning("IAPCore: Controller is null!");
            PopUpController.instance?.OpenPopUp("Purchase system not ready. Please wait...");
            return;
        }

        Product product = controller.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            controller.InitiatePurchase(product);
            Debug.Log($"IAPCore: Purchasing {productId}");
        }
        else
        {
            Debug.LogWarning($"IAPCore: Product not available: {productId}");
        }
    }

    [ContextMenu("Check IAP Status")]
    public void CheckIAPStatus()
    {
        Debug.Log("=== IAP Status ===");
        Debug.Log($"Instance exists: {Instance != null}");
        Debug.Log($"Controller is null: {controller == null}");
        Debug.Log($"Product IDs count: {productIds?.Length ?? 0}");
        
        if (controller != null)
        {
            Debug.Log($"Products available: {controller.products.all.Length}");
            foreach (var product in controller.products.all)
            {
                Debug.Log($"  - {product.definition.id}: Available={product.availableToPurchase}");
            }
        }
    }
}
