using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSettingManager : MonoBehaviour
{
    #region Inspector Fields

    // Reference to the ScriptableObject containing all card back designs
    public CardsContainer cardsContainer;

    // Array of card design UI elements (button and checkmark)
    public CardDesigns[] cardDesigns;

    #endregion

    #region Unity Methods

    // Called when the script instance is being loaded
    void Start()
    {
        FirstTimeCardsSet();
    }

    #endregion

    #region Card Design Initialization

    /// <summary>
    /// Sets up the card back designs and initializes the checkmark for the selected design.
    /// </summary>
    void FirstTimeCardsSet()
    {
        for (int i = 0; i < cardsContainer.CardBackDesigns.Length; i++)
        {
            // Assign the card back sprite to each card design button
            cardDesigns[i].cardCardDesign.GetComponent<Image>().sprite = cardsContainer.CardBackDesigns[i];
            // Hide all checkmarks initially
            cardDesigns[i].checkMark.SetActive(false);
        }
        // Show checkmark for the currently selected design
        cardDesigns[LocalSettingBlackJack.SelectedDesignIndex].checkMark.SetActive(true);
    }

    #endregion

    #region UI Event Handlers

    /// <summary>
    /// Called when a card design toggle is changed by the user.
    /// Updates the selected design and checkmarks.
    /// </summary>
    public void OnToggleChanged()
    {
        // Play button click sound
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.ButtonSound);

        // Get the currently clicked UI object
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < cardDesigns.Length; i++)
        {
            // Check if this design was clicked
            bool isToggleOn = clickedObject == cardDesigns[i].cardCardDesign.gameObject;
            if (isToggleOn)
                LocalSettingBlackJack.SelectedDesignIndex = i;

            // Show or hide the checkmark accordingly
            cardDesigns[i].checkMark.SetActive(isToggleOn);
        }
    }

    #endregion
}

#region Helper Classes

/// <summary>
/// Holds references to a card design button and its checkmark UI element.
/// </summary>
[Serializable]
public class CardDesigns
{
    public Button cardCardDesign;
    public GameObject checkMark;
}
#endregion