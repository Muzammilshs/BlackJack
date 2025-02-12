using System.Collections;
using UnityEngine;
using com.muzamil;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering.Universal;
public class TableDealer : MonoBehaviour
{
    [SerializeField] Rm RefMgr;
    [SerializeField] RectTransform playerCardPos;
    [SerializeField] RectTransform playerCardPos_P1_Split;
    [SerializeField] RectTransform playerCardPos_P2_Split;
    [SerializeField] RectTransform dealerCardPos;

    [ShowOnly]
    public List<CardProperty> playerCards;
    [ShowOnly]
    public List<CardProperty> playerCards_1_Split;
    [ShowOnly]
    public List<CardProperty> playerCards_2_Split;
    [ShowOnly]
    public List<CardProperty> dealerCards;

    [SerializeField] GameObject winParticles;
    ScoreManager scoreManager;
    public enum Winner
    {
        BUST,
        JACKPOT,
        DEALERWINS,
        WON,
        PUSH
    };
    Winner winStatus;

    [Space]
    [Header("Win and loose ")]
    [SerializeField] GameObject WinLoosepanel;
    [SerializeField] TMP_Text winLooseStatusTxt;
    [SerializeField] TMP_Text winAmountTxt;
    HitStandBarHandler hitStandBar;
    void Start()
    {
        scoreManager = RefMgr.scoreManager;
        hitStandBar = RefMgr.hitStandBarHandler;
    }
    #region Cards Distribution
    public void FirstTimeDealCards()
    {
        StartCoroutine(FirstTimeDealCardsCrt());
    }

    IEnumerator FirstTimeDealCardsCrt()
    {
        RefMgr.cardsManager.ReCreateLimitedCards();
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 4; i++)
        {
            int playerOrDealer = i % 2;
            RectTransform cardPos = playerOrDealer == 0 ? playerCardPos : dealerCardPos;
            CardProperty card = Get1Card(cardPos);
            int totalCards = playerOrDealer == 0 ? playerCards.Count : dealerCards.Count;
            bool isFlip = i == 3 ? false : true;
            playCardAnimation(card.gameObject, cardPos.gameObject, totalCards, isFlip, playerOrDealer == 0 ? true : false);
            if (playerOrDealer == 0)
                playerCards.Add(card);
            else
                dealerCards.Add(card);
            RefMgr.cardsManager.RearrangeCardsStack();
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1.0f);
        RefMgr.gameStateManager.UpDateGameState(GameState.State.BETRAISE);
    }
    public void SendOneCard(bool isPlayer)
    {
        StartCoroutine(SendOneCardCrt(isPlayer));
    }

    IEnumerator SendOneCardCrt(bool isPlayer)
    {
        //string aa = isPlayer ? "Player" : "dealer";
        yield return new WaitForSeconds(0.1f);

        RectTransform cardPos = isPlayer ? playerCardPos : dealerCardPos;
        CardProperty card = Get1Card(cardPos);
        int totalCards = isPlayer ? playerCards.Count : dealerCards.Count;
        playCardAnimation(card.gameObject, cardPos.gameObject, totalCards, true, isPlayer);
        if (isPlayer)
            playerCards.Add(card);
        else
            dealerCards.Add(card);
        RefMgr.cardsManager.RearrangeCardsStack();
        yield return new WaitForSeconds(1f);
        RefMgr.cardsManager.ReCreateLimitedCards();
    }

    public void SendOneCardOnSplit()
    {
        StartCoroutine(SendOneCardOnSplitCrt());
    }
    int crds = 0;
    IEnumerator SendOneCardOnSplitCrt()
    {
        yield return new WaitForSeconds(0.1f);
        int splitingNumber = hitStandBar.splitingTurnNumber;
        RectTransform cardPos = splitingNumber == 0 ? playerCardPos_P1_Split : playerCardPos_P2_Split;
        CardProperty card = Get1Card(cardPos);
        int totalCards = splitingNumber == 0 ? playerCards_1_Split.Count : playerCards_2_Split.Count;
        playCardAnimation(card.gameObject, cardPos.gameObject, totalCards, true, true);
        if (splitingNumber == 0)
            playerCards_1_Split.Add(card);
        else
            playerCards_2_Split.Add(card);
        RefMgr.cardsManager.RearrangeCardsStack();
        yield return new WaitForSeconds(0.7f);
        RefMgr.scoreManager.SplitPlayerScores();
        yield return new WaitForSeconds(0.8f);
        RefMgr.cardsManager.ReCreateLimitedCards();

        OnSplitCheckNextTurn(false);
    }
    public void OnSplitCheckNextTurn(bool isStand)
    {
        int splitingNumber = hitStandBar.splitingTurnNumber;
        if (splitingNumber == 0)
        {
            if (isStand || RefMgr.scoreManager.playerTotalScores_P1_Split >= LocalSetting.ScoresLimit)
            {
                hitStandBar.splitingTurnNumber++;
                hitStandBar.ShowHitStandBar(false);
                SendOneCardOnSplit();
            }
        }
        else if (splitingNumber == 1)
        {
            if (isStand || RefMgr.scoreManager.playerTotalScores_P2_Split >= LocalSetting.ScoresLimit)
                hitStandBar.splitingTurnNumber++;
        }
        splitingNumber = hitStandBar.splitingTurnNumber;
        if (splitingNumber < 2)
            hitStandBar.ShowHitStandBar(true);
        else
            RefMgr.gameStateManager.UpDateGameState(GameState.State.STAND);
    }
    CardProperty Get1Card(RectTransform rect)
    {
        int card = Random.Range(0, RefMgr.cardsManager.cardsStackList.Count);
        CardProperty cp = RefMgr.cardsManager.cardsStackList[card];
        RefMgr.cardsManager.cardsStackList.Remove(cp);
        return cp;
    }
    void playCardAnimation(GameObject ObjectToAnimate, GameObject targetObj, int offSet, bool shouldFLip, bool isPlayer)
    {
        GameObject card = ObjectToAnimate;
        GameObject TgtObj = targetObj;
        card.transform.SetParent(TgtObj.transform.parent.transform);

        Vector3 tgtPos = TgtObj.transform.position + Vector3.right * offSet * 50;
        ObjectToAnimate.transform.DOMove(tgtPos, 0.25f)
            .OnComplete(() => FlipCard(card, shouldFLip, isPlayer));

        ObjectToAnimate.transform.DORotateQuaternion(targetObj.transform.rotation, 0.25f);

    }


    public void FlipCard(GameObject obj, bool shouldFLip, bool isPlayer)
    {
        if (!shouldFLip)
        {
            MoveSlightlyUp(obj);
        }
        else
        {
            obj.transform.DORotate(Vector2.up * 90, 0.25f)
                .OnComplete(() => ReverseRotate(obj, isPlayer));
        }
    }

    public void ReverseRotate(GameObject obj, bool isPlayer)
    {
        bool isPlyr = isPlayer;
        scoreManager.SetScores(isPlyr);
        obj.GetComponent<CardProperty>().ShowOriginalSprite();
        obj.transform.DORotate(Vector2.zero, 0.25f);
    }

    void MoveSlightlyUp(GameObject obj)
    {
        Vector3 targetPosition = obj.transform.position + Vector3.up * 100;
        obj.transform.DOMove(targetPosition, 0.25f).SetLoops(2, LoopType.Yoyo);
    }

    #endregion
    int resultIndex = 0;
    public void DeclearWinnerWithSplit()
    {
        if (resultIndex == 0)
        {
            if (scoreManager.playerTotalScores_P1_Split > LocalSetting.ScoresLimit)
                UpDateWinStatus(Winner.BUST);
            else if (scoreManager.playerTotalScores_P1_Split > scoreManager.dealerTotalScores || scoreManager.dealerTotalScores > LocalSetting.ScoresLimit)
                UpDateWinStatus(Winner.WON);
            else if (scoreManager.playerTotalScores_P1_Split < scoreManager.dealerTotalScores)
                UpDateWinStatus(Winner.DEALERWINS);
            else if (scoreManager.playerTotalScores_P1_Split == scoreManager.dealerTotalScores)
                UpDateWinStatus(Winner.PUSH);
        }
        else
        {
            if (scoreManager.playerTotalScores_P2_Split > LocalSetting.ScoresLimit)
                UpDateWinStatus(Winner.BUST);
            else if (scoreManager.playerTotalScores_P2_Split > scoreManager.dealerTotalScores || scoreManager.dealerTotalScores > LocalSetting.ScoresLimit)
                UpDateWinStatus(Winner.WON);
            else if (scoreManager.playerTotalScores_P2_Split < scoreManager.dealerTotalScores)
                UpDateWinStatus(Winner.DEALERWINS);
            else if (scoreManager.playerTotalScores_P2_Split == scoreManager.dealerTotalScores)
                UpDateWinStatus(Winner.PUSH);
        }
        Debug.LogError("Winner exe splitter");
    }

    public void DeclearWinner(bool isJackpot)
    {

        if (isJackpot)
            UpDateWinStatus(Winner.JACKPOT);
        else if (scoreManager.playerTotalScores > LocalSetting.ScoresLimit)
            UpDateWinStatus(Winner.BUST);
        else if (scoreManager.playerTotalScores > scoreManager.dealerTotalScores || scoreManager.dealerTotalScores > LocalSetting.ScoresLimit)
            UpDateWinStatus(Winner.WON);
        else if (scoreManager.playerTotalScores < scoreManager.dealerTotalScores)
            UpDateWinStatus(Winner.DEALERWINS);
        else if (scoreManager.playerTotalScores == scoreManager.dealerTotalScores)
            UpDateWinStatus(Winner.PUSH);
        //Debug.LogError("Wineer is at pt 1");
        Debug.LogError("Winner exe");
    }
    public void UpDateWinStatus(Winner status)
    {
        winStatus = status;
        if (!hitStandBar.isSplitting || resultIndex > 0)
            RefMgr.gameStateManager.UpDateGameState(GameState.State.RESULT);
        ShowWinningDetail(winStatus);
    }
    int winAmount = 0;
    void ShowWinningDetail(Winner status)
    {
        float delayTime = 0.5f;
        switch (status)
        {
            case Winner.DEALERWINS:
                Debug.Log("Dealer Winner");
                winAmount = 0;
                winAmountTxt.gameObject.SetActive(false);
                StartCoroutine(ShowWinPanel("Dealer win"));
                if (!hitStandBar.isSplitting)
                    StartCoroutine(CloneAndSendChips(delayTime, false, false));
                else
                    StartCoroutine(CloneAndSendChipsOnSplit(delayTime, false, false, resultIndex));
                LocalSetting.TotalGamesLost++;
                break;
            case Winner.PUSH:
                Debug.Log("Match tie");
                winAmount = RefMgr.potHandler.GetPotAmount;
                if (RefMgr.hitStandBarHandler.isDoubleBet)
                    winAmount = winAmount * 2;
                winAmountTxt.text = "+" + winAmount;
                winAmountTxt.gameObject.SetActive(true);
                StartCoroutine(ShowWinPanel("Push"));
                if (!hitStandBar.isSplitting)
                    StartCoroutine(CloneAndSendChips(delayTime, true, true));
                else
                    StartCoroutine(CloneAndSendChipsOnSplit(delayTime, true, true, resultIndex));
                LocalSetting.TotalTieGames++;
                break;
            case Winner.JACKPOT:
                Debug.Log("Jackpot");
                winParticles.SetActive(true);
                winAmount = (RefMgr.potHandler.GetPotAmount * 2) + (RefMgr.potHandler.GetPotAmount / 2);
                winAmountTxt.text = "+" + winAmount;
                winAmountTxt.gameObject.SetActive(true);
                StartCoroutine(ShowWinPanel("Jackpot"));
                if (!hitStandBar.isSplitting)
                    StartCoroutine(CloneAndSendChips(delayTime, true, false));
                else
                    StartCoroutine(CloneAndSendChipsOnSplit(delayTime, true, false, resultIndex));
                LocalSetting.TotalJackPOT++;
                break;
            case Winner.BUST:
                Debug.Log("Busted");
                winAmount = 0;
                winAmountTxt.gameObject.SetActive(false);
                StartCoroutine(ShowWinPanel("Bust"));
                if (!hitStandBar.isSplitting)
                    StartCoroutine(CloneAndSendChips(delayTime, false, false));
                else
                    StartCoroutine(CloneAndSendChipsOnSplit(delayTime, false, false, resultIndex));
                LocalSetting.TotalGamesLost++;
                break;
            case Winner.WON:
                Debug.Log("Won");
                winAmount = RefMgr.potHandler.GetPotAmount * 2;
                if (RefMgr.hitStandBarHandler.isDoubleBet)
                    winAmount = winAmount * 2;
                winAmountTxt.text = "+" + winAmount;
                winAmountTxt.gameObject.SetActive(true);
                StartCoroutine(ShowWinPanel("won"));
                if (!hitStandBar.isSplitting)
                    StartCoroutine(CloneAndSendChips(delayTime, true, false));
                else
                    StartCoroutine(CloneAndSendChipsOnSplit(delayTime, true, false, resultIndex));
                LocalSetting.TotalGamesWon++;
                break;
        }
    }



    IEnumerator ShowWinPanel(string winStatusMessage)
    {
        Debug.Log("Amount won: " + winAmount);
        RefMgr.potHandler.CollectReward(winAmount);
        WinLoosepanel.SetActive(true);
        winLooseStatusTxt.text = winStatusMessage;
        yield return new WaitForSeconds(1.5f);
        WinLoosepanel.SetActive(false);
        if (hitStandBar.isSplitting)
        {
            if (resultIndex == 0)
            {
                resultIndex++;
                yield return new WaitForSeconds(1f);
                DeclearWinnerWithSplit();
            }
            else
            {
                resultIndex = 0;
            }
        }
    }

    IEnumerator CloneAndSendChips(float delay, bool isSendToPlayer, bool isPush)
    {
        yield return new WaitForSeconds(delay);
        RectTransform playerPos = null;
        RectTransform dealerPos = null;
        RefMgr.betBarHandler.BettedPos(out playerPos, out dealerPos);

        // clone chips and then send to respective player
        RectTransform InitialPos = isSendToPlayer ? dealerPos : playerPos;
        if (isPush)
            InitialPos = playerPos;
        RectTransform targetPos = isSendToPlayer ? playerPos : dealerPos;

        BetBarHandler bbh = RefMgr.betBarHandler;

        for (int i = 0; i < bbh.betPlacedChips.Count; i++)
        {
            GameObject chip = Instantiate(bbh.betPlacedChips[i]);
            LocalSetting.SetPosAndRect(chip, InitialPos, InitialPos.transform.parent);
            playChipAnimation(chip, targetPos.gameObject);
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator CloneAndSendChipsOnSplit(float delay, bool isSendToPlayer, bool isPush, int splitPart)
    {
        yield return new WaitForSeconds(delay);
        RectTransform playerPos = null;
        RectTransform dealerPos = null;

        RefMgr.betBarHandler.BettedPosSplit(out playerPos, out dealerPos, splitPart);

        // clone chips and then send to respective player
        RectTransform InitialPos = isSendToPlayer ? dealerPos : splitPart == 0 ? RefMgr.betBarHandler.bettedChipsPos_1_Split : RefMgr.betBarHandler.bettedChipsPos_2_Split;
        if (isPush)
            InitialPos = splitPart == 1 ? RefMgr.betBarHandler.bettedChipsPos_1_Split : RefMgr.betBarHandler.bettedChipsPos_2_Split;
        RectTransform targetPos = isSendToPlayer ? playerPos : dealerPos;


        BetBarHandler bbh = RefMgr.betBarHandler;
        if (splitPart == 0)
        {
            for (int i = 0; i < bbh.betPlacedChips_1_Split.Count; i++)
            {
                GameObject chip = Instantiate(bbh.betPlacedChips_1_Split[i]);
                LocalSetting.SetPosAndRect(chip, InitialPos, InitialPos.transform.parent);
                playChipAnimation(chip, targetPos.gameObject);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            for (int i = 0; i < bbh.betPlacedChips_2_Split.Count; i++)
            {
                GameObject chip = Instantiate(bbh.betPlacedChips_2_Split[i]);
                LocalSetting.SetPosAndRect(chip, InitialPos, InitialPos.transform.parent);
                playChipAnimation(chip, targetPos.gameObject);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
    void playChipAnimation(GameObject ObjectToAnimate, GameObject targetObj)
    {
        GameObject TgtObj = targetObj;
        GameObject chip = ObjectToAnimate;
        chip.transform.SetParent(TgtObj.transform.parent.transform);

        ObjectToAnimate.transform.DOMove(TgtObj.transform.position, 1.5f)
            .OnComplete(() => Destroy(chip));
    }
    public void SplitCardsOnSplit()
    {
        playerCards_1_Split.Add(playerCards[0]);
        playerCards_2_Split.Add(playerCards[1]);
        playerCards.Clear();
        LocalSetting.SetPosAndRect(playerCards_1_Split[0].gameObject, playerCardPos_P1_Split, playerCardPos_P1_Split.parent);
        LocalSetting.SetPosAndRect(playerCards_2_Split[0].gameObject, playerCardPos_P2_Split, playerCardPos_P2_Split.parent);
        RefMgr.scoreManager.SplitPlayerScores();
    }
}
