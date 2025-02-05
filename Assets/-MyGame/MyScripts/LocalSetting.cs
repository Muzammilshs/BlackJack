using UnityEngine;

public static class LocalSetting
{
    const string TOTALCASHKEY = "total_cash";
    const int firstTimeAmount = 300000;
    public const int ScoresLimit = 21;

    const string CARDBACKDESIGNKEY = "CardBackDesign";

    public static int SelectedDesignIndex
    {
        get
        {
            return PlayerPrefs.GetInt(CARDBACKDESIGNKEY, 0);
        }
        set
        {
            PlayerPrefs.SetInt(CARDBACKDESIGNKEY, value);
            PlayerPrefs.Save();
        }
    }


    public static bool IsCashAvailAble(int amount)
    {
        return amount <= GetTotalCash() ? true : false;
    }
    public static void SetTotalCash(int amount)
    {
        int cash = GetTotalCash();
        cash += amount;
        PlayerPrefs.SetString(TOTALCASHKEY, cash.ToString());
    }

    public static int GetTotalCash()
    {
        if (!PlayerPrefs.HasKey(TOTALCASHKEY))
            PlayerPrefs.SetString(TOTALCASHKEY, firstTimeAmount.ToString());
        string cash = PlayerPrefs.GetString(TOTALCASHKEY);
        return StringToInt(cash);
    }
    public static int StringToInt(string stg)
    {
        return int.Parse(stg);
    }

    public static void SetPosAndRect(GameObject InstantiatedObj, RectTransform ALReadyObjPos, Transform Parentobj)
    {
        InstantiatedObj.transform.SetParent(Parentobj);
        RectTransform MyPlayerRectTransform = InstantiatedObj.GetComponent<RectTransform>();
        MyPlayerRectTransform.localScale = ALReadyObjPos.localScale;
        MyPlayerRectTransform.localPosition = ALReadyObjPos.localPosition;
        MyPlayerRectTransform.anchorMin = ALReadyObjPos.anchorMin;
        MyPlayerRectTransform.anchorMax = ALReadyObjPos.anchorMax;
        MyPlayerRectTransform.anchoredPosition = ALReadyObjPos.anchoredPosition;
        MyPlayerRectTransform.sizeDelta = ALReadyObjPos.sizeDelta;
        MyPlayerRectTransform.localRotation = ALReadyObjPos.localRotation;

    }
}
