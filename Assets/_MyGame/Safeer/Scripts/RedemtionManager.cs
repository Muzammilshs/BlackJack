using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RedemptionManager : MonoBehaviour
{
    public TMP_InputField chipsInputField;
    public Button visitFacebookButton;
    public Button redeemButton;
    public TMP_Text statusText;
    public int userChips = 50000; // Example chips amount, should be fetched from player data.

    private const int minRedeemChips = 1000;
    private const int maxRedeemChips = 1000000;
    private const string hasVisitedFacebookKey = "HasVisitedFacebook";

    private bool hasVisitedFacebook;

    void Start()
    {
        hasVisitedFacebook = PlayerPrefs.GetInt(hasVisitedFacebookKey, 0) == 1;
        redeemButton.interactable = hasVisitedFacebook; // Only enable if visited Facebook
        visitFacebookButton.onClick.AddListener(OpenFacebookPage);
        redeemButton.onClick.AddListener(ProcessRedemption);
    }

    void OpenFacebookPage()
    {
        Application.OpenURL("https://www.facebook.com/share/1A21uqqP4f/?mibextid=wwXIfr"); // Replace with your actual page URL
        hasVisitedFacebook = true;
        PlayerPrefs.SetInt(hasVisitedFacebookKey, 1);
        PlayerPrefs.Save();
        redeemButton.interactable = true; // Enable redeem button
    }

    void ProcessRedemption()
    {
        int redeemAmount;
        if (!int.TryParse(chipsInputField.text, out redeemAmount))
        {
            statusText.text = "Enter a valid number.";
            return;
        }

        if (redeemAmount < minRedeemChips || redeemAmount > maxRedeemChips)
        {
            statusText.text = $"Enter an amount between {minRedeemChips} and {maxRedeemChips}.";
            return;
        }

        if (redeemAmount > userChips)
        {
            statusText.text = "Not enough chips.";
            return;
        }

        // Deduct chips (You need to integrate with your backend or game data)
        userChips -= redeemAmount;

        // Display redemption instructions
        statusText.text = $"Redemption request for {redeemAmount} chips sent. Please follow instructions on Facebook.";
    }
}
