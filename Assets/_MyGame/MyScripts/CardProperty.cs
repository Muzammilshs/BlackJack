using UnityEngine;
using UnityEngine.UI;
namespace com.muzammil
{
    public class CardProperty : MonoBehaviour
    {
        public CardState.SUIT Suit;
        public CardState.CARDVALUE Card;
        public int Power;
        public int SecondPower;
        public int CardIndexInArray;


        Sprite originalSprite;
        public void ShowDummySkin()
        {
            if (originalSprite == null)
                originalSprite = GetComponent<Image>().sprite;
            GetComponent<Image>().sprite = Rm.Instance.gameManager.dummyCardSprite;
        }

        public void ShowOriginalSprite() => GetComponent<Image>().sprite = originalSprite;
    }
}