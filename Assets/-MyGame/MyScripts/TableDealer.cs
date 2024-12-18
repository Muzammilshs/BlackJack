using System.Collections;
using UnityEngine;
using com.muzamil;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
public class TableDealer : MonoBehaviour
{
    [SerializeField] Rm RefMgr;
    [SerializeField] RectTransform playerCardPos;
    [SerializeField] RectTransform dealerCardPos;

    [ShowOnly]
    public List<CardProperty> playerCards;
    [ShowOnly]
    public List<CardProperty> playerCardsSplitSet;
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

    void Start()
    {
        scoreManager = Rm.Instance.scoreManager;
    }
    #region Cards Distribution
    public void FirstTimeDealCards()
    {
        StartCoroutine(FirstTimeDealCardsCrt());
    }

    IEnumerator FirstTimeDealCardsCrt()
    {
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
        //card.transform.parent = TgtObj.transform.parent.transform;
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

    public void DeclearWinner(bool isJackpot)
    {

        if (isJackpot)
            UpDateWinStatus(Winner.JACKPOT);
        else if (scoreManager.playerTotalScores > scoreManager.targetScores)
            UpDateWinStatus(Winner.BUST);
        else if (scoreManager.playerTotalScores > scoreManager.dealerTotalScores || scoreManager.dealerTotalScores > scoreManager.targetScores)
            UpDateWinStatus(Winner.WON);
        else if (scoreManager.playerTotalScores < scoreManager.dealerTotalScores)
            UpDateWinStatus(Winner.DEALERWINS);
        else if (scoreManager.playerTotalScores == scoreManager.dealerTotalScores)
            UpDateWinStatus(Winner.PUSH);
        //Debug.LogError("Wineer is at pt 1");
    }
    public void UpDateWinStatus(Winner status)
    {
        winStatus = status;
        RefMgr.gameStateManager.UpDateGameState(GameState.State.RESULT);
        //Debug.LogError("Wineer is at pt 2");
        ShowWinningDetail(winStatus);
    }
    int winAmount = 0;
    void ShowWinningDetail(Winner status)
    {
        float delayTime = 0.5f;
        switch (status)
        {
            case Winner.DEALERWINS:
                Debug.LogError("Dealer Winner");
                winAmount = 0;
                winAmountTxt.gameObject.SetActive(false);
                StartCoroutine(ShowWinPanel("Dealer win"));
                StartCoroutine(CloneAndSendChips(delayTime, false, false));

                break;
            case Winner.PUSH:
                Debug.LogError("Match tie");
                winAmount = RefMgr.potHandler.GetPotAmount;
                winAmountTxt.text = "+" + winAmount;
                winAmountTxt.gameObject.SetActive(true);
                StartCoroutine(ShowWinPanel("Push"));
                StartCoroutine(CloneAndSendChips(delayTime, true, true));

                break;
            case Winner.JACKPOT:
                Debug.LogError("Jackpot");
                winParticles.SetActive(true);
                winAmount = (RefMgr.potHandler.GetPotAmount * 2) + (RefMgr.potHandler.GetPotAmount / 2);
                winAmountTxt.text = "+" + winAmount;
                winAmountTxt.gameObject.SetActive(true);
                StartCoroutine(ShowWinPanel("Jackpot"));
                StartCoroutine(CloneAndSendChips(delayTime, true, false));

                break;
            case Winner.BUST:
                Debug.LogError("Busted");
                winAmount = 0;
                winAmountTxt.gameObject.SetActive(false);
                StartCoroutine(ShowWinPanel("Bust"));
                StartCoroutine(CloneAndSendChips(delayTime, false, false));

                break;
            case Winner.WON:
                Debug.LogError("Won");
                winAmount = RefMgr.potHandler.GetPotAmount * 2;
                winAmountTxt.text = "+" + winAmount;
                winAmountTxt.gameObject.SetActive(true);
                StartCoroutine(ShowWinPanel("won"));
                StartCoroutine(CloneAndSendChips(delayTime, true, false));
                break;
        }
    }



    IEnumerator ShowWinPanel(string winStatusMessage)
    {
        Debug.LogError("Amount won: " + winAmount);
        RefMgr.potHandler.CollectReward(winAmount);
        WinLoosepanel.SetActive(true);
        winLooseStatusTxt.text = winStatusMessage;
        yield return new WaitForSeconds(1.5f);
        WinLoosepanel.SetActive(false);
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
    void playChipAnimation(GameObject ObjectToAnimate, GameObject targetObj)
    {
        GameObject TgtObj = targetObj;
        GameObject chip = ObjectToAnimate;
        chip.transform.SetParent(TgtObj.transform.parent.transform);

        ObjectToAnimate.transform.DOMove(TgtObj.transform.position, 1.5f)
            .OnComplete(() => Destroy(chip));
        //ObjectToAnimate.transform.DORotateQuaternion(targetObj.transform.rotation, 0.25f);

    }

}
