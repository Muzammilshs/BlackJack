using UnityEngine;
using UnityEngine.UI;
namespace com.muzammil
{
    public class CardProperty : MonoBehaviour
    {
        #region Card Properties

        // Suit of the card (Spade, Heart, Diamond, Club)
        public CardState.SUIT Suit;

        // Value of the card (2-10, Jack, Queen, King, Ace)
        public CardState.CARDVALUE Card;

        // Main power value of the card (used for game logic)
        public int Power;

        // Secondary power value (for special rules or games)
        public int SecondPower;

        // Index of this card in the card array
        public int CardIndexInArray;

        #endregion

        #region Private Fields

        // Stores the original sprite of the card for restoration
        Sprite originalSprite;

        #endregion

        #region Card Visual Methods

        /// <summary>
        /// Sets the card's image to a dummy/placeholder sprite.
        /// Stores the original sprite if not already stored.
        /// </summary>
        public void ShowDummySkin()
        {
            if (originalSprite == null)
                originalSprite = GetComponent<Image>().sprite;
            GetComponent<Image>().sprite = Rm.Instance.gameManager.dummyCardSprite;
        }

        /// <summary>
        /// Restores the card's image to its original sprite.
        /// </summary>
        public void ShowOriginalSprite() => GetComponent<Image>().sprite = originalSprite;

        #endregion
    }
}