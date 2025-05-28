using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSettingManager : MonoBehaviour
{
    public CardsContainer cardsContainer;
    public CardDesigns[] cardDesigns;
    void Start()
    {
        FirstTimeCardsSet();
    }
    void FirstTimeCardsSet()
    {
        for (int i = 0; i < cardsContainer.CardBackDesigns.Length; i++)
        {
            //cardDesigns[LocalSettingBlackJack.SelectedDesignIndex].cardCardDesign.GetComponent<Image>().sprite = cardsContainer.CardBackDesigns[i];
            cardDesigns[i].cardCardDesign.GetComponent<Image>().sprite = cardsContainer.CardBackDesigns[i];
            cardDesigns[i].checkMark.SetActive(false);
        }
        //Debug.LogError(LocalSettingBlackJack.SelectedDesignIndex);
        cardDesigns[LocalSettingBlackJack.SelectedDesignIndex].checkMark.SetActive(true);
    }

    public void OnToggleChanged()
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        for (int i = 0; i < cardDesigns.Length; i++)
        {
            bool isToggleOn = clickedObject == cardDesigns[i].cardCardDesign.gameObject;
            if (isToggleOn) LocalSettingBlackJack.SelectedDesignIndex = i;
            cardDesigns[i].checkMark.SetActive(isToggleOn);
        }
    }

}

[Serializable]
public class CardDesigns
{
    public Button cardCardDesign;
    public GameObject checkMark;
}
