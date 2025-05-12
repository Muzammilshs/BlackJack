using Firebase.Database;
using Firebase.Extensions;
using Google;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class LocalSettingBlackJack
{
    const string TOTALCASHKEY = "total_cash";
    public static int firstTimeAmount = 300;
    public const int ScoresLimit = 21;
    public const string coins = "coins_";
    const string CARDBACKDESIGNKEY = "CardBackDesign";
    private static int cachedCash = firstTimeAmount;
    private static bool isCashFetched = false;
    private static Task<int> cashFetchTask = null;

    public static Dictionary<float, float> dict = new Dictionary<float, float> { { 100, 1f }, { 500, 5f }, { 1000, 10f }, { 2000, 20f }, { 3000, 30f }, { 4000, 40f }, { 5000, 50f }, { 10000, 100f } };
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
        LoginWithGoogle.instance.AddCoins(amount);
    }


    public static async Task<int> GetTotalCashAsync()
    {
        if (LoginWithGoogle.instance == null || LoginWithGoogle.instance.databaseReference == null)
        {
            Debug.Log("LoginWithGoogle instance or database reference is null. Returning default value.");
            if (LoginWithGoogle.instance == null)
                return firstTimeAmount;

            LoginWithGoogle.instance.totalCash = firstTimeAmount;
            return firstTimeAmount;
        }
        Debug.Log("GETTING CASH");
        try
        {
            var snapshot = await LoginWithGoogle.instance.databaseReference
                .Child("users")
                .Child(LoginWithGoogle.instance.userId)
                .Child("totalCash")
                .GetValueAsync();

            if (snapshot.Exists && int.TryParse(snapshot.Value.ToString(), out int cash))
            {
                Debug.Log($"Fetched total cash: {cash} for user {LoginWithGoogle.instance.userId}");
                LoginWithGoogle.instance.totalCash = cash;
                return cash;
            }
            else
            {
                Debug.LogWarning($"No total cash value found for user {LoginWithGoogle.instance.userId}. Using default: {firstTimeAmount}.");
                LoginWithGoogle.instance.totalCash = firstTimeAmount;

                return firstTimeAmount;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to fetch total cash: {ex}");
            LoginWithGoogle.instance.totalCash = firstTimeAmount;
            return firstTimeAmount;
        }
    }


    public static int GetTotalCash()
    {
        Debug.Log("GET CASH!");
        if (cashFetchTask == null)
        {
            // Start fetching if not already started
            Debug.Log("GET CASH!2");

            cashFetchTask = GetTotalCashAsync();
        }

        if (!cashFetchTask.IsCompleted)
        {
            // Unity-friendly: wait without freezing
            var asyncOperation = WaitForTaskCompletion(cashFetchTask);
            while (!asyncOperation.MoveNext()) { }
        }

        return LoginWithGoogle.instance != null ? LoginWithGoogle.instance.totalCash : firstTimeAmount;
    }

    private static IEnumerator<object> WaitForTaskCompletion(Task task)
    {
        while (!task.IsCompleted)
        {
            yield return null;
        }
    }
    public static int StringToInt(string stg)
    {
        return int.Parse(stg);
    }

    public static void SetPositionAndRectTransform(GameObject InstantiatedObj, RectTransform ALReadyObjPos, Transform Parentobj)
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
