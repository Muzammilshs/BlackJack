using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Rm refMgr;

    public Sprite dummyCardSprite;
    public GameObject shopPanel;
    [SerializeField] Sprite[] cardDesigns;


    public void SetCardDesign()
    {
        if (LocalSetting.SelectedDesignIndex < cardDesigns.Length)
            dummyCardSprite = cardDesigns[LocalSetting.SelectedDesignIndex];
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

    #region Button functions
    public void OnHomeBtnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    #endregion
}
