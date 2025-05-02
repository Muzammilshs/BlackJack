using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Rm refMgr;

    public Sprite dummyCardSprite;
    public GameObject shopPanel;

    [SerializeField] GameObject exitPanel;

    private void Start()
    {
        exitPanel.SetActive(false);
    }
    public void SetCardDesign()
    {
        dummyCardSprite = Rm.Instance.cardsManager.allCards.CardBackDesigns[LocalSetting.SelectedDesignIndex];
    }
    public List<GameObject> ClearList(List<GameObject> list)
    {
        if (list == null)
        {
            list = new List<GameObject>();
            return list;
        }
        if (list.Count > 0)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                Destroy(list[i]);
            list.Clear();
        }
        return list;
    }


    public void ShowShopPanel()
    {
        shopPanel.SetActive(true);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
            if (exitPanel.activeInHierarchy)
                exitPanel.SetActive(false);
            else
                exitPanel.SetActive(true);
        }
    }

    #region Button functions
    public void OnHomeBtnClick()
    {
        if (LoginWithGoogle.instance != null)
            DestroyImmediate(LoginWithGoogle.instance.gameObject);        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }
    #endregion
}
