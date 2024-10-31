using UnityEngine;

public static class LocalSetting
{
    const string totalCash = "total_cash";
    const int firstTimeAmount = 3000;


    public static bool IsCashAvailAble(int amount)
    {
        return amount <= GetTotalCash() ? true : false;
    }
    public static void SetTotalCash(int amount)
    {
        int cash = GetTotalCash();
        cash += amount;
        PlayerPrefs.SetString(totalCash, cash.ToString());
    }

    public static int GetTotalCash()
    {
        if (!PlayerPrefs.HasKey(totalCash))
            PlayerPrefs.SetString(totalCash, firstTimeAmount.ToString());
        string cash = PlayerPrefs.GetString(totalCash);
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
