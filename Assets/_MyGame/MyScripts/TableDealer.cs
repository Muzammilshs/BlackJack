using System.Collections;
using UnityEngine;
using com.muzammil;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
public class TableDealer : MonoBehaviour
{
    [SerializeField] bool isCheat = true;
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

    public GameObject _insurancePanel;
    [SerializeField] GameObject _insuranceWinLosePanel;
    [SerializeField] TMP_Text _insuranceWinLoseTxt;

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
    [SerializeField] Image image;
    HitStandBarHandler hitStandBar;

    [HideInInspector] public bool isPlayerWonTheInsurance = false;
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
    bool isInsuranceEligible = false;
    //IEnumerator FirstTimeDealCardsCrt()
    //{
    //    RefMgr.cardsManager.ReCreateLimitedCards();
    //    yield return new WaitForSeconds(0.5f);
    //    for (int i = 0; i < 4; i++)
    //    {
    //        int playerOrDealer = i % 2;
    //        RectTransform cardPos = playerOrDealer == 0 ? playerCardPos : dealerCardPos;
    //        CardProperty card = Get1Card(cardPos);

    //        if (isCheat)
    //        {
    //            if (i == 1)
    //                card = GetSpecificCard(11);
    //            else if (i == 3)
    //                card = GetSpecificCard(5);
    //        }

    //        int totalCards = playerOrDealer == 0 ? playerCards.Count : dealerCards.Count;
    //        bool isFlip = i == 3 ? false : true;
    //        playCardAnimation(card.gameObject, cardPos.gameObject, totalCards, isFlip, playerOrDealer == 0 ? true : false);
    //        if (playerOrDealer == 0)
    //            playerCards.Add(card);
    //        else
    //            dealerCards.Add(card);
    //        RefMgr.cardsManager.RearrangeCardsStack();
    //        yield return new WaitForSeconds(1f);
    //    }
    //    yield return new WaitForSeconds(1.0f);

    //    // Checking for insurance eligibility   
    //    Debug.LogError("Dealer cards: " + dealerCards.Count + "       val: " + dealerCards[1].Power);
    //    int playerCardsPower = playerCards[0].Power + playerCards[1].Power;
    //    if (playerCardsPower != 21)
    //    {
    //        if (dealerCards[0].Power == 11)
    //            isInsuranceEligible = true;
    //    }
    //    if (isInsuranceEligible)
    //    {
    //        _insurancePanel.SetActive(true);
    //        yield return new WaitUntil(() => !isInsuranceEligible);
    //        if (RefMgr.potHandler.GetInsuranceAmount > 0)
    //            isPlayerWonTheInsurance = (dealerCards[0].Power == 11 && dealerCards[1].Power == 10);
    //    }
    //    RefMgr.gameStateManager.UpDateGameState(GameState.State.BETRAISE);
    //}
    IEnumerator FirstTimeDealCardsCrt()
    {
        RefMgr.cardsManager.ReCreateLimitedCards();
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 4; i++)
        {
            int playerOrDealer = i % 2;
            HandType.HANDTYPE ht = playerOrDealer == 0 ? HandType.HANDTYPE.PLAYERHAND : HandType.HANDTYPE.DEALERHAND;

            Rm.currentCardData = RefMgr.GetCardData(ht);
            CardProperty card = Get1Card();

            if (isCheat)
            {
                if (i == 1)
                    card = GetSpecificCard(11);
                else if (i == 3)
                    card = GetSpecificCard(5);
            }

            int totalCards = playerOrDealer == 0 ? playerCards.Count : this.dealerCards.Count;
            bool isFlip = i == 3 ? false : true;
            Rm.currentCardData.AddCard(card, isFlip);

            RefMgr.cardsManager.RearrangeCardsStack();
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1.0f);

        // Checking for insurance eligibility   
        CardsData dealerCards = RefMgr.GetCardData(HandType.HANDTYPE.DEALERHAND);
        int playerCardsPower = dealerCards.cardsList[0].Power + dealerCards.cardsList[1].Power;
        if (playerCardsPower != 21)
        {
            if (dealerCards.cardsList[0].Power == 11)
                isInsuranceEligible = true;
        }
        if (isInsuranceEligible)
        {
            _insurancePanel.SetActive(true);
            yield return new WaitUntil(() => !isInsuranceEligible);
            if (RefMgr.potHandler.GetInsuranceAmount > 0)
                isPlayerWonTheInsurance = (dealerCards.cardsList[0].Power == 11 && dealerCards.cardsList[1].Power == 10);
        }
        RefMgr.gameStateManager.UpDateGameState(GameState.State.BETRAISE);
        //////////////////////////// Upper step completed
    }
    public void ShowInsuranceWinLosepanel(bool isWin)
    {
        _insuranceWinLosePanel.SetActive(true);
        _insuranceWinLoseTxt.text = isWin ? "💰 You won the insurance bet! The dealer had Jackpot.\n\nYour insurance payout has been added to your balance." : "❌ You lost the insurance bet. The dealer does not have Jackpot.\n\nYour insurance amount has been deducted.";
    }
    CardProperty GetSpecificCard(int power)
    {

        int card = Random.Range(0, RefMgr.cardsManager.cardsStackList.Count);
        for (int i = 0; i < RefMgr.cardsManager.cardsStackList.Count; i++)
        {
            if (RefMgr.cardsManager.cardsStackList[i].Power == power)
            {
                card = i;
                break;
            }
        }
        CardProperty cp = RefMgr.cardsManager.cardsStackList[card];
        RefMgr.cardsManager.cardsStackList.Remove(cp);

        if (cp.Power != power)
        {
            GetSpecificCard(power);
        }
        return cp;
    }
    public void SendOneCard(bool isPlayer)
    {
        StartCoroutine(SendOneCardCrt(isPlayer));
    }
    public void SendOneCardGen(CardsData cardsData)
    {
        StartCoroutine(SendOneCardCrtGen(cardsData));
    }

    IEnumerator SendOneCardCrt(bool isPlayer)
    {
        yield break;
        yield return new WaitForSeconds(0.1f);

        RectTransform cardPos = isPlayer ? playerCardPos : dealerCardPos;
        CardProperty card = Get1Card();
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

    public IEnumerator SendOneCardCrtGen(CardsData cardsData)
    {
        yield return new WaitForSeconds(0.1f);

        CardProperty card = Get1Card();
        cardsData.AddCard(card, true);

        RefMgr.cardsManager.RearrangeCardsStack();
        yield return new WaitForSeconds(1f);
        RefMgr.cardsManager.ReCreateLimitedCards();

        // check scores limit and show drop card or standup 
        CheckScoresForNext(false);
    }

    public void SendOneCardOnSplit()
    {
        StartCoroutine(SendOneCardOnSplitCrt());
    }

    IEnumerator SendOneCardOnSplitCrt()
    {
        hitStandBar.ShowHitStandBar(false);
        yield return new WaitForSeconds(1f);
        CardProperty card = Get1Card();

        Rm.currentCardData.AddCard(card, true);
        RefMgr.cardsManager.RearrangeCardsStack();
        yield return new WaitForSeconds(1.5f);
        RefMgr.cardsManager.ReCreateLimitedCards();

        CheckScoresForNext(false);
    }

    public void CheckScoresForNext(bool isStand)
    {
        int scores = Rm.currentCardData.highScores;
        if (scores < LocalSettingBlackJack.ScoresLimit && !isStand)
        {
            if (Rm.currentCardData.handType != HandType.HANDTYPE.DEALERHAND)
            {
                hitStandBar.ShowHitStandBar(true);
                hitStandBar.doubleBtn.SetActive(false);
                hitStandBar.splitBtn.SetActive(false);
                RefMgr.hitStandBarHandler.CheckForSplitButtonActivation();
            }
            else
                hitStandBar.ShowHitStandBar(false); // Dealer's turn, hide hit/stand bar
        }
        else
        {
            hitStandBar.ShowHitStandBar(false);
            CardsData cd = Rm.Instance.GetValidHandCardData();
            if (cd != null)
            {
                Rm.currentCardData = cd;
                if (Rm.currentCardData.cardsList.Count < 2)
                    StartCoroutine(SendOneCardOnSplitCrt());
                else
                {
                    if (Rm.currentCardData.handType != HandType.HANDTYPE.DEALERHAND)
                    {
                        hitStandBar.ShowHitStandBar(true);
                        hitStandBar.doubleBtn.SetActive(false);
                        hitStandBar.splitBtn.SetActive(false);
                        RefMgr.hitStandBarHandler.CheckForSplitButtonActivation();
                    }
                    else
                    {
                        // drop dealer cards
                        Debug.LogError("Dealer's turn to drop cards");
                        if (RefMgr.gameStateManager.GetCurrentGameState() != GameState.State.STAND)
                            RefMgr.gameStateManager.UpDateGameState(GameState.State.STAND);
                    }
                }
            }
        }
    }
    public void OnSplitCheckNextTurn(bool isStand)
    {
        //CardsData cd =
        return;

        int splitingNumber = hitStandBar.splitingTurnNumber;
        if (splitingNumber == 0)
        {
            if (isStand || RefMgr.scoreManager.playerTotalScores_P1_Split >= LocalSettingBlackJack.ScoresLimit)
            {
                hitStandBar.splitingTurnNumber++;
                hitStandBar.ShowHitStandBar(false);
                SendOneCardOnSplit();
            }
        }
        else if (splitingNumber == 1)
        {
            if (isStand || RefMgr.scoreManager.playerTotalScores_P2_Split >= LocalSettingBlackJack.ScoresLimit)
                hitStandBar.splitingTurnNumber++;
        }
        splitingNumber = hitStandBar.splitingTurnNumber;
        if (splitingNumber < 2)
            hitStandBar.ShowHitStandBar(true);
        else
            RefMgr.gameStateManager.UpDateGameState(GameState.State.STAND);
    }

    CardProperty Get1Card()
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
        return;
        if (resultIndex == 0)
        {
            if (scoreManager.playerTotalScores_P1_Split > LocalSettingBlackJack.ScoresLimit)
                UpDateWinStatus(Winner.BUST);
            else if (scoreManager.playerTotalScores_P1_Split > scoreManager.dealerTotalScores || scoreManager.dealerTotalScores > LocalSettingBlackJack.ScoresLimit)
                UpDateWinStatus(Winner.WON);
            else if (scoreManager.playerTotalScores_P1_Split < scoreManager.dealerTotalScores)
                UpDateWinStatus(Winner.DEALERWINS);
            else if (scoreManager.playerTotalScores_P1_Split == scoreManager.dealerTotalScores)
                UpDateWinStatus(Winner.PUSH);
        }
        else
        {
            if (scoreManager.playerTotalScores_P2_Split > LocalSettingBlackJack.ScoresLimit)
                UpDateWinStatus(Winner.BUST);
            else if (scoreManager.playerTotalScores_P2_Split > scoreManager.dealerTotalScores || scoreManager.dealerTotalScores > LocalSettingBlackJack.ScoresLimit)
                UpDateWinStatus(Winner.WON);
            else if (scoreManager.playerTotalScores_P2_Split < scoreManager.dealerTotalScores)
                UpDateWinStatus(Winner.DEALERWINS);
            else if (scoreManager.playerTotalScores_P2_Split == scoreManager.dealerTotalScores)
                UpDateWinStatus(Winner.PUSH);
        }

    }

    public void DeclearWinner(bool isJackpot)
    {
        return;
        if (isJackpot)
            UpDateWinStatus(Winner.JACKPOT);
        else if (scoreManager.playerTotalScores > LocalSettingBlackJack.ScoresLimit)
            UpDateWinStatus(Winner.BUST);
        else if (scoreManager.playerTotalScores > scoreManager.dealerTotalScores || scoreManager.dealerTotalScores > LocalSettingBlackJack.ScoresLimit)
            UpDateWinStatus(Winner.WON);
        else if (scoreManager.playerTotalScores < scoreManager.dealerTotalScores)
            UpDateWinStatus(Winner.DEALERWINS);
        else if (scoreManager.playerTotalScores == scoreManager.dealerTotalScores)
            UpDateWinStatus(Winner.PUSH);

        //StartCoroutine(InsuranceWinner(1.8f));

    }
    public void UpDateWinStatus(Winner status)
    {
        return;
        winStatus = status;
        if (!hitStandBar.isSplitting || resultIndex > 0)
            RefMgr.gameStateManager.UpDateGameState(GameState.State.RESULT);
        ShowWinningDetail(winStatus);
    }
    int winAmount = 0;
    void ShowWinningDetail(Winner status)
    {
        return;
        float delayTime = 0.5f;
        switch (status)
        {  // 0 => Player win
           // 1 => Dealer win
           // 2 => Push

            case Winner.DEALERWINS:
                //Debug.Log("Dealer Winner");
                winAmount = 0;
                winAmountTxt.gameObject.SetActive(false);
                StartCoroutine(ShowWinPanel("Dealer win", winAmount));
                //if (!hitStandBar.isSplitting)
                //    StartCoroutine(CloneAndSendChips(delayTime, 1));
                //else
                //    StartCoroutine(CloneAndSendChipsOnSplit(delayTime, resultIndex, 1));
                LocalSettingBlackJack.TotalGamesLost++;
                break;
            case Winner.PUSH:
                //Debug.Log("Match tie");
                winAmount = RefMgr.potHandler.GetPotAmount;
                if (RefMgr.hitStandBarHandler.isDoubleBet)
                    winAmount = winAmount * 2;
                winAmountTxt.text = "+" + winAmount;
                winAmountTxt.gameObject.SetActive(true);
                StartCoroutine(ShowWinPanel("Push", winAmount));
                //if (!hitStandBar.isSplitting)
                //    StartCoroutine(CloneAndSendChips(delayTime, 2));
                //else
                //    StartCoroutine(CloneAndSendChipsOnSplit(delayTime, resultIndex, 2));
                LocalSettingBlackJack.TotalTieGames++;
                break;
            case Winner.JACKPOT:
                //Debug.Log("Jackpot");
                winParticles.SetActive(true);
                winAmount = (RefMgr.potHandler.GetPotAmount * 2) + (RefMgr.potHandler.GetPotAmount / 2);
                winAmountTxt.text = "+" + winAmount;
                winAmountTxt.gameObject.SetActive(true);
                StartCoroutine(ShowWinPanel("Jackpot", winAmount));
                //if (!hitStandBar.isSplitting)
                //    StartCoroutine(CloneAndSendChips(delayTime, 0));
                //else
                //    StartCoroutine(CloneAndSendChipsOnSplit(delayTime, resultIndex, 0));
                LocalSettingBlackJack.TotalJackPOT++;
                break;
            case Winner.BUST:
                //Debug.Log("Busted");
                winAmount = 0;
                winAmountTxt.gameObject.SetActive(false);
                StartCoroutine(ShowWinPanel("Bust", winAmount));
                //if (!hitStandBar.isSplitting)
                //    StartCoroutine(CloneAndSendChips(delayTime, 1));
                //else
                //    StartCoroutine(CloneAndSendChipsOnSplit(delayTime, resultIndex, 1));
                LocalSettingBlackJack.TotalGamesLost++;
                break;
            case Winner.WON:
                //Debug.Log("Won");
                winAmount = RefMgr.potHandler.GetPotAmount * 2;
                if (RefMgr.hitStandBarHandler.isDoubleBet)
                    winAmount = winAmount * 2;
                winAmountTxt.text = "+" + winAmount;
                winAmountTxt.gameObject.SetActive(true);
                StartCoroutine(ShowWinPanel("won", winAmount));
                //if (!hitStandBar.isSplitting)
                //    StartCoroutine(CloneAndSendChips(delayTime, 0));
                //else
                //    StartCoroutine(CloneAndSendChipsOnSplit(delayTime, resultIndex, 0));
                LocalSettingBlackJack.TotalGamesWon++;
                break;
        }
    }
    public IEnumerator InsuranceWinner(float delayTime)
    {
        int insuranceAmount = RefMgr.potHandler.GetInsuranceAmount;
        // Insuracne  reward
        if (insuranceAmount > 0)
            RefMgr.tableDealer.ShowInsuranceWinLosepanel(false);
        if (RefMgr.tableDealer.isPlayerWonTheInsurance)
        {
            RefMgr.potHandler.CollectReward(insuranceAmount * 2);
            RefMgr.tableDealer.isPlayerWonTheInsurance = false;
            RefMgr.tableDealer.ShowInsuranceWinLosepanel(true);
        }
        yield return new WaitForSeconds(delayTime);
    }


    IEnumerator ShowWinPanel(string winStatusMessage, int wonAmount)
    {
        RefMgr.potHandler.CollectReward(wonAmount);
        WinLoosepanel.SetActive(true);


        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);

        winLooseStatusTxt.text = winStatusMessage;
        yield return new WaitForSeconds(1.5f);
        WinLoosepanel.SetActive(false);

    }

    IEnumerator CloneAndSendChips(float delay, int winLoose)
    {
        // 0 => Player win
        // 1 => Dealer win
        // 2 => Push
        yield return new WaitForSeconds(delay);
        RectTransform initialPos = null;
        RectTransform finalPos = null;
        RefMgr.betBarHandler.BettedPos(out initialPos, out finalPos, winLoose);

        BetBarHandler bbh = RefMgr.betBarHandler;

        for (int i = 0; i < bbh.betPlacedChips.Count; i++)
        {
            GameObject chip = Instantiate(bbh.betPlacedChips[i]);
            LocalSettingBlackJack.SetPositionAndRectTransform(chip, initialPos, initialPos.transform.parent);
            bbh.betPlacedChips[i].SetActive(false);
            playChipAnimation(chip, finalPos.gameObject);
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator CloneAndSendChipsOnWinLoose(CardsData cardsData, Vector3 initialPos, Vector3 targetPos)
    {
        for (int i = 0; i < cardsData.chipsList.Count; i++)
        {
            Debug.LogError("Cloning chip: " + i + "  " + cardsData.chipsList[i].name);
            GameObject chip = Instantiate(cardsData.chipsList[i]);
            LocalSettingBlackJack.SetPositionAndRectTransform(chip, cardsData.chipsPosRect, cardsData.chipsPosRect.transform.parent);
            cardsData.chipsList[i].SetActive(false);
            chip.SetActive(true);
            chip.transform.position = initialPos;
            playChipAnimationNew(chip, targetPos);
            yield return new WaitForSeconds(0.03f);
        }
        if (cardsData._doubleBettedChipsList.Count > 0)
        {
            float xPos = cardsData._doubleBettedChipsPosRect.transform.position.x - cardsData.chipsPosRect.transform.position.x;
            initialPos = new Vector3(initialPos.x + xPos, initialPos.y, initialPos.z);
            targetPos = new Vector3(targetPos.x + xPos, targetPos.y, targetPos.z);

            for (int i = 0; i < cardsData._doubleBettedChipsList.Count; i++)
            {
                GameObject chip = Instantiate(cardsData._doubleBettedChipsList[i]);
                LocalSettingBlackJack.SetPositionAndRectTransform(chip, cardsData._doubleBettedChipsPosRect, cardsData.chipsPosRect.transform.parent);
                cardsData._doubleBettedChipsList[i].SetActive(false);
                chip.SetActive(true);
                chip.transform.position = initialPos;
                playChipAnimationNew(chip, targetPos);
                yield return new WaitForSeconds(0.03f);
            }
        }
    }
    public void ToggleChipsStatus(bool isActive)
    {
        return;
        BetBarHandler bbh = RefMgr.betBarHandler;
        for (int i = 0; i < bbh.betPlacedChips.Count; i++)
        {
            bbh.betPlacedChips[i].SetActive(isActive);
        }
    }
    IEnumerator CloneAndSendChipsOnSplit(float delay, int splitPart, int winLoose)
    {
        // 0 => Player win
        // 1 => Dealer win
        // 2 => Push
        yield return new WaitForSeconds(delay);
        RectTransform initialPos = null;
        RectTransform finalPos = null;

        RefMgr.betBarHandler.BettedPosSplit(out initialPos, out finalPos, splitPart, winLoose);

        BetBarHandler bbh = RefMgr.betBarHandler;
        if (splitPart == 0)
        {
            for (int i = 0; i < bbh.betPlacedChips_1_Split.Count; i++)
            {
                GameObject chip = Instantiate(bbh.betPlacedChips_1_Split[i]);
                LocalSettingBlackJack.SetPositionAndRectTransform(chip, initialPos, initialPos.transform.parent);
                bbh.betPlacedChips_1_Split[i].SetActive(false);
                playChipAnimation(chip, finalPos.gameObject);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            for (int i = 0; i < bbh.betPlacedChips_2_Split.Count; i++)
            {
                GameObject chip = Instantiate(bbh.betPlacedChips_2_Split[i]);
                LocalSettingBlackJack.SetPositionAndRectTransform(chip, initialPos, initialPos.transform.parent);
                bbh.betPlacedChips_2_Split[i].SetActive(false);
                playChipAnimation(chip, finalPos.gameObject);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
    void playChipAnimation(GameObject ObjectToAnimate, GameObject targetObj)
    {
        return;
        GameObject TgtObj = targetObj;
        GameObject chip = ObjectToAnimate;
        chip.transform.SetParent(TgtObj.transform.parent.transform);

        ObjectToAnimate.transform.DOMove(TgtObj.transform.position, 1.5f)
            .OnComplete(() => Destroy(chip));
    }
    void playChipAnimationNew(GameObject ObjectToAnimate, Vector3 targetPos)
    {
        Vector3 TgtPos = targetPos;
        GameObject chip = ObjectToAnimate;

        ObjectToAnimate.transform.DOMove(TgtPos, 2.5f)
            .OnComplete(() => Destroy(chip));
    }

    #region Show win lose of player hands

    public void ShowResultsNew()
    {
        StartCoroutine(ShowResultsCrt());
    }

    IEnumerator ShowResultsCrt()
    {
        yield return new WaitForSeconds(0.2f);
        Rm rm = Rm.Instance;

        int dealerScores = rm.GetCardData(HandType.HANDTYPE.DEALERHAND).highScores;

        while (rm.GetValidHandForResults() != null)
        {
            CardsData cd = rm.cardsDataForResult;
            int playerScores = cd.highScores;
            if (cd.cardsList.Count == 2 && playerScores == LocalSettingBlackJack.ScoresLimit)
            {
                ShowWinningDetailNew(Winner.JACKPOT, cd);
            }
            else if (playerScores > LocalSettingBlackJack.ScoresLimit)
            {
                ShowWinningDetailNew(Winner.BUST, cd);
            }
            else if (dealerScores > LocalSettingBlackJack.ScoresLimit || playerScores > dealerScores)
            {
                ShowWinningDetailNew(Winner.WON, cd);
            }
            else if (playerScores < dealerScores)
            {
                ShowWinningDetailNew(Winner.DEALERWINS, cd);
            }
            else if (playerScores == dealerScores)
            {
                ShowWinningDetailNew(Winner.PUSH, cd);
            }

            float delayTime = 2.5f;
            yield return new WaitForSeconds(delayTime);
        }

        Debug.LogError("reset the game");
        if (RefMgr.potHandler.GetInsuranceAmount > 0)
        {
            StartCoroutine(InsuranceWinner(0));
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => !RefMgr.tableDealer._insuranceWinLosePanel.activeInHierarchy);
        }
        StartCoroutine(CollectWasteCards());

    }
    void ShowWinningDetailNew(Winner status, CardsData cd)
    {
        Vector3 tgtPos = Vector3.zero;
        Vector3 initPos = Vector3.zero;
        switch (status)
        {
            case Winner.DEALERWINS:
                winAmount = 0;
                winAmountTxt.gameObject.SetActive(false);
                StartCoroutine(ShowWinPanel("Dealer win", winAmount));
                LocalSettingBlackJack.TotalGamesLost++;

                tgtPos = cd.chipsPosRect.transform.position + Vector3.up * 2500;
                initPos = cd.chipsPosRect.transform.position;
                StartCoroutine(CloneAndSendChipsOnWinLoose(cd, initPos, tgtPos));
                
                break;
            case Winner.PUSH:
                winAmount = RefMgr.potHandler.GetPotAmount;
                if (RefMgr.hitStandBarHandler.isDoubleBet)
                    winAmount = winAmount * 2;
                winAmountTxt.text = "+" + winAmount;
                winAmountTxt.gameObject.SetActive(true);
                StartCoroutine(ShowWinPanel("Push", winAmount));
                LocalSettingBlackJack.TotalTieGames++;

                tgtPos = cd.chipsPosRect.transform.position + Vector3.down * 1000;
                initPos = cd.chipsPosRect.transform.position;
                StartCoroutine(CloneAndSendChipsOnWinLoose(cd, initPos, tgtPos));

                break;
            case Winner.JACKPOT:
                winParticles.SetActive(true);
                winAmount = (RefMgr.potHandler.GetPotAmount * 2) + (RefMgr.potHandler.GetPotAmount / 2);
                winAmountTxt.text = "+" + winAmount;
                winAmountTxt.gameObject.SetActive(true);
                StartCoroutine(ShowWinPanel("Jackpot", winAmount));
                LocalSettingBlackJack.TotalJackPOT++;

                tgtPos = cd.chipsPosRect.transform.position + Vector3.down * 1000;
                initPos = cd.chipsPosRect.transform.position + Vector3.up * 2500;
                StartCoroutine(CloneAndSendChipsOnWinLoose(cd, initPos, tgtPos));

                break;
            case Winner.BUST:
                winAmount = 0;
                winAmountTxt.gameObject.SetActive(false);
                StartCoroutine(ShowWinPanel("Bust", winAmount));
                LocalSettingBlackJack.TotalGamesLost++;

                tgtPos = cd.chipsPosRect.transform.position + Vector3.up * 2500;
                initPos = cd.chipsPosRect.transform.position;
                StartCoroutine(CloneAndSendChipsOnWinLoose(cd, initPos, tgtPos));

                break;
            case Winner.WON:
                winAmount = RefMgr.potHandler.GetPotAmount * 2;
                if (RefMgr.hitStandBarHandler.isDoubleBet)
                    winAmount = winAmount * 2;
                winAmountTxt.text = "+" + winAmount;
                winAmountTxt.gameObject.SetActive(true);
                StartCoroutine(ShowWinPanel("won", winAmount));
                LocalSettingBlackJack.TotalGamesWon++;

                tgtPos = cd.chipsPosRect.transform.position + Vector3.down * 1000;
                initPos = cd.chipsPosRect.transform.position + Vector3.up * 2500;
                StartCoroutine(CloneAndSendChipsOnWinLoose(cd, cd.chipsPosRect.transform.position, tgtPos));

                break;
        }
    }


    #region Collecting waste cards
    IEnumerator CollectWasteCards()
    {
        Rm rm = Rm.Instance;
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < rm.cardsData.Count; i++)
        {
            if (rm.cardsData[i].isValidHand)
            {
                CardsData cd = rm.cardsData[i];
                for (int j = 0; j < cd.cardsList.Count; j++)
                {
                    rm.cardsManager.wasteCardsList.Add(cd.cardsList[j].gameObject);
                    PlayCardAnimation(cd.cardsList[j].gameObject, rm.cardsManager.wasteCardsPos.gameObject);
                    yield return new WaitForSeconds(0.15f);
                }
                cd.cardsList.Clear();
                yield return new WaitForSeconds(0.2f);
            }
        }
        RefMgr.gameStateManager.UpDateGameState(GameState.State.RESULT);
    }
    #endregion


    public void ResetWholeGame()
    {
        Rm rm = Rm.Instance;
        rm.cardsDataForResult = null;
        rm.handIndexForResults = 0;

        for (int i = 0; i < rm.cardsData.Count; i++)
        {
            rm.cardsData[i].ResetCardDataThings();
        }
        rm.cardsManager.ClearWasteCards();
    }


    #endregion

    #region cards animation

    void PlayCardAnimation(GameObject objectToAnimate, GameObject targetObj)
    {
        if (objectToAnimate == null || targetObj == null)
        {
            Debug.LogError("Object to animate or target object is null.");
            return;
        }

        objectToAnimate.transform.SetParent(targetObj.transform.parent);
        objectToAnimate.transform.DOMove(targetObj.transform.position, 0.2f)
            .OnComplete(() => OnCompleteShowDummyCard(objectToAnimate));
        objectToAnimate.transform.DORotateQuaternion(targetObj.transform.rotation, 0.2f);
    }
    void OnCompleteShowDummyCard(GameObject obj)
    {
        obj.GetComponent<CardProperty>().ShowDummySkin();
    }
    #endregion

    #region Cards splitting w.r.t current hand
    public void SplitCardsOnSplit()
    {
        Rm rm = Rm.Instance;
        if (Rm.currentCardData.handType == HandType.HANDTYPE.PLAYERHAND)
        {
            CardsData cd = Rm.currentCardData;
            cd.SplitThisSetTo2(rm.GetCardData(HandType.HANDTYPE.PLAYERHAND_SPLIT_P1), rm.GetCardData(HandType.HANDTYPE.PLAYERHAND_SPLIT_P2));

            Rm.currentCardData = rm.GetCardData(HandType.HANDTYPE.PLAYERHAND_SPLIT_P1);
        }
        else if (Rm.currentCardData.handType == HandType.HANDTYPE.PLAYERHAND_SPLIT_P1)
        {
            CardsData cd = Rm.currentCardData;
            cd.SplitThisSetTo2(rm.GetCardData(HandType.HANDTYPE.PLAYERHAND_SPLIT_P1_P1), rm.GetCardData(HandType.HANDTYPE.PLAYERHAND_SPLIT_P1_P2));

            Rm.currentCardData = rm.GetCardData(HandType.HANDTYPE.PLAYERHAND_SPLIT_P1_P1);
        }
        else if (Rm.currentCardData.handType == HandType.HANDTYPE.PLAYERHAND_SPLIT_P2)
        {
            CardsData cd = Rm.currentCardData;
            cd.SplitThisSetTo2(rm.GetCardData(HandType.HANDTYPE.PLAYERHAND_SPLIT_P2_P1), rm.GetCardData(HandType.HANDTYPE.PLAYERHAND_SPLIT_P2_P2));

            Rm.currentCardData = rm.GetCardData(HandType.HANDTYPE.PLAYERHAND_SPLIT_P2_P1);
        }
    }
    #endregion


    #region Insurance section
    public void InusranceChoice(bool isYes)
    {
        isInsuranceEligible = false;
        _insurancePanel.SetActive(false);
        RefMgr.potHandler.SetInsuranceAmount(isYes);
    }

    #endregion
}
