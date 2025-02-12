using UnityEngine;

public static class LocalSetting
{
    const string TOTALCASHKEY = "total_cash";
    const int firstTimeAmount = 3000000;
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

    const string TOTALGAMESPLAYEDKEY = "TotalGamesPlayed";
    public static int TotalGamesPlayed
    {
        get { return PlayerPrefs.GetInt(TOTALGAMESPLAYEDKEY, 0); }
        set
        {
            PlayerPrefs.SetInt(TOTALGAMESPLAYEDKEY, value);
            PlayerPrefs.Save();
        }
    }

    const string TOTALGAMESWONKEY = "TotalGamesWon";
    public static int TotalGamesWon
    {
        get { return PlayerPrefs.GetInt(TOTALGAMESWONKEY, 0); }
        set
        {
            PlayerPrefs.SetInt(TOTALGAMESWONKEY, value);
            PlayerPrefs.Save();
        }
    }

    const string TOTALGAMESLOSTKEY = "TotalGamesLost";
    public static int TotalGamesLost
    {
        get { return PlayerPrefs.GetInt(TOTALGAMESLOSTKEY, 0); }
        set { PlayerPrefs.SetInt(TOTALGAMESLOSTKEY, value); PlayerPrefs.Save(); }
    }

    const string TOTALTIEGAMESKEY = "TotalTieGames";
    public static int TotalTieGames
    {
        get { return PlayerPrefs.GetInt(TOTALTIEGAMESKEY, 0); }
        set { PlayerPrefs.SetInt(TOTALTIEGAMESKEY, value); PlayerPrefs.Save(); }
    }

    const string TOTALJACKPOTKEY = "TotalJackpOT";
    public static int TotalJackPOT
    {
        get { return PlayerPrefs.GetInt(TOTALJACKPOTKEY, 0); }
        set { PlayerPrefs.SetInt(TOTALJACKPOTKEY, value); PlayerPrefs.Save(); }
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
