using System.Collections;
using UnityEngine;
using com.muzammil;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
/// <summary>
/// Handles the main dealer logic for blackjack, including card dealing, split logic, insurance, 
/// win/lose UI, and chip animations.
/// </summary>
public class TableDealer : MonoBehaviour
{
    #region Fields and UI References

    [SerializeField] bool isCheat = true; // Enables cheat mode for testing specific cards
    [SerializeField] Rm RefMgr; // Reference to the main game manager

    public GameObject _insurancePanel; // Insurance offer panel
    [SerializeField] GameObject _insuranceWinLosePanel; // Insurance result panel
    [SerializeField] TMP_Text _insuranceWinLoseTxt; // Insurance result text

    [SerializeField] GameObject winParticles; // Particle effect for win/jackpot
    //ScoreManager scoreManager; // Reference to score manager

    /// <summary>
    /// Enum for possible hand results.
    /// </summary>
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
    [Header("Win and lose UI")]
    [SerializeField] GameObject WinLoosepanel; // Panel for win/lose result
    [SerializeField] TMP_Text winLooseStatusTxt; // Win/lose status text
    [SerializeField] TMP_Text winAmountTxt; // Win amount text
    [SerializeField] Image image; // Background image for result panel
    HitStandBarHandler hitStandBar; // Reference to hit/stand bar handler

    [HideInInspector] public bool isPlayerWonTheInsurance = false; // Insurance result flag

    #endregion

    #region Unity Methods

    void Start()
    {
        //scoreManager = RefMgr.scoreManager;
        hitStandBar = RefMgr.hitStandBarHandler;
    }

    #endregion

    #region Card Distribution and Dealing

    /// <summary>
    /// Starts the coroutine to deal the initial four cards (two to player, two to dealer).
    /// </summary>
    public void FirstTimeDealCards()
    {
        StartCoroutine(FirstTimeDealCardsCrt());
    }

    bool isInsuranceEligible = false;

    /// <summary>
    /// Coroutine for dealing the first four cards and handling insurance logic.
    /// </summary>
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

            // Cheat mode: force specific cards for testing
            if (isCheat)
            {
                if (i == 1)
                    card = GetSpecificCard(11);
                else if (i == 3)
                    card = GetSpecificCard(5);
            }

            bool isFlip = i == 3 ? false : true;
            Rm.currentCardData.AddCard(card, isFlip);

            RefMgr.cardsManager.RearrangeCardsStack();
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1.0f);

        // Insurance eligibility check
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
    }

    /// <summary>
    /// Shows the insurance win/lose panel with the appropriate message.
    /// </summary>
    public void ShowInsuranceWinLosepanel(bool isWin)
    {
        _insuranceWinLosePanel.SetActive(true);
        _insuranceWinLoseTxt.text = isWin
            ? "💰 You won the insurance bet! The dealer had Jackpot.\n\nYour insurance payout has been added to your balance."
            : "❌ You lost the insurance bet. The dealer does not have Jackpot.\n\nYour insurance amount has been deducted.";
    }

    /// <summary>
    /// Gets a specific card by power value from the deck (used for cheat/testing).
    /// </summary>
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

    /// <summary>
    /// Gets a random card from the deck.
    /// </summary>
    CardProperty Get1Card()
    {
        int card = Random.Range(0, RefMgr.cardsManager.cardsStackList.Count);
        CardProperty cp = RefMgr.cardsManager.cardsStackList[card];
        RefMgr.cardsManager.cardsStackList.Remove(cp);
        return cp;
    }

    #endregion

    #region Card Dealing (Split and General)

    /// <summary>
    /// Deals a card to the specified hand (used for split or general hands).
    /// </summary>
    public void SendOneCardGen(CardsData cardsData)
    {
        StartCoroutine(SendOneCardCrtGen(cardsData));
    }

    /// <summary>
    /// Coroutine to deal a card to a hand and update UI/logic.
    /// </summary>
    public IEnumerator SendOneCardCrtGen(CardsData cardsData)
    {
        yield return new WaitForSeconds(0.1f);

        CardProperty card = Get1Card();
        cardsData.AddCard(card, true);

        RefMgr.cardsManager.RearrangeCardsStack();
        yield return new WaitForSeconds(1f);
        RefMgr.cardsManager.ReCreateLimitedCards();

        // Check if more actions are needed after dealing
        CheckScoresForNext(false);
    }

    /// <summary>
    /// Deals a card to the current split hand.
    /// </summary>
    public void SendOneCardOnSplit()
    {
        StartCoroutine(SendOneCardOnSplitCrt());
    }

    /// <summary>
    /// Coroutine to deal a card to the current split hand and update UI/logic.
    /// </summary>
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

    #endregion

    #region Game State and Score Checking

    /// <summary>
    /// Checks the current hand's score and updates the UI and game state accordingly.
    /// </summary>
    public void CheckScoresForNext(bool isStand)
    {
        int scores = Rm.currentCardData.highScores;
        if (scores < LocalSettingBlackJack.ScoresLimit && !isStand && !hitStandBar.isDoubleBet)
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
            Rm.currentCardData.ShowJustHighScores();
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
                        // Dealer's turn to play
                        Debug.LogError("Dealer's turn to drop cards");
                        if (RefMgr.gameStateManager.GetCurrentGameState() != GameState.State.STAND)
                            RefMgr.gameStateManager.UpDateGameState(GameState.State.STAND);
                    }
                }
            }
        }
    }

    #endregion

    #region Insurance Logic

    /// <summary>
    /// Handles insurance payout and UI after a delay.
    /// </summary>
    public IEnumerator InsuranceWinner(float delayTime)
    {
        int insuranceAmount = RefMgr.potHandler.GetInsuranceAmount;
        // Insurance reward
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

    /// <summary>
    /// Called when player makes an insurance choice.
    /// </summary>
    public void InusranceChoice(bool isYes)
    {
        isInsuranceEligible = false;
        _insurancePanel.SetActive(false);
        RefMgr.potHandler.SetInsuranceAmount(isYes);
    }

    #endregion

    #region Win/Lose UI and Chip Animation

    /// <summary>
    /// Shows the win/lose panel with the result message and amount.
    /// </summary>
    IEnumerator ShowWinPanel(string winStatusMessage, int wonAmount)
    {
        RefMgr.potHandler.CollectReward(wonAmount);
        WinLoosepanel.SetActive(true);
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        winLooseStatusTxt.text = winStatusMessage;
        yield return new WaitForSeconds(2f);
        WinLoosepanel.SetActive(false);
    }

    /// <summary>
    /// Animates chips moving from a hand to a target position (used for win/lose).
    /// </summary>
    IEnumerator CloneAndSendChipsOnWinLoose(CardsData cardsData, Vector3 initialPos, Vector3 targetPos)
    {
        for (int i = 0; i < cardsData.chipsList.Count; i++)
        {
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

    /// <summary>
    /// Animates a chip to a target position and destroys it on arrival.
    /// </summary>
    void playChipAnimationNew(GameObject ObjectToAnimate, Vector3 targetPos)
    {
        Vector3 TgtPos = targetPos;
        GameObject chip = ObjectToAnimate;

        ObjectToAnimate.transform.DOMove(TgtPos, 2.5f)
            .OnComplete(() => Destroy(chip));
    }

    #endregion

    #region Results and Game Reset

    /// <summary>
    /// Starts the coroutine to show results for all hands.
    /// </summary>
    public void ShowResultsNew()
    {
        StartCoroutine(ShowResultsCrt());
    }

    /// <summary>
    /// Coroutine to process and display results for all hands, then reset the game.
    /// </summary>
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
            yield return new WaitUntil(() => !_insuranceWinLosePanel.activeInHierarchy);
        }
        StartCoroutine(CollectWasteCards());
    }

    int winAmount = 0;

    /// <summary>
    /// Shows the win/lose/jackpot/push result for a hand and animates chips accordingly.
    /// </summary>
    void ShowWinningDetailNew(Winner status, CardsData cd)
    {
        Vector3 tgtPos = Vector3.zero;
        Vector3 initPos = Vector3.zero;
        winAmountTxt.gameObject.SetActive(true);
        switch (status)
        {
            case Winner.DEALERWINS:
                winAmount = 0;
                winAmountTxt.text = "Dealer wins the round!";
                StartCoroutine(ShowWinPanel("Dealer win", winAmount));
                LocalSettingBlackJack.TotalGamesLost++;

                tgtPos = cd.chipsPosRect.transform.position + Vector3.up * 2500;
                initPos = cd.chipsPosRect.transform.position;
                StartCoroutine(CloneAndSendChipsOnWinLoose(cd, initPos, tgtPos));
                SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.loseSound);
                break;

            case Winner.PUSH:
                winAmount = RefMgr.potHandler.GetPotAmount;
                if (RefMgr.hitStandBarHandler.isDoubleBet)
                    winAmount = winAmount * 2;
                winAmountTxt.text = "It's a tie! You get\n+$" + winAmount;
                StartCoroutine(ShowWinPanel("Push", winAmount));
                LocalSettingBlackJack.TotalTieGames++;

                tgtPos = cd.chipsPosRect.transform.position + Vector3.down * 1000;
                initPos = cd.chipsPosRect.transform.position;
                StartCoroutine(CloneAndSendChipsOnWinLoose(cd, initPos, tgtPos));
                SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.tieSound);
                break;

            case Winner.JACKPOT:
                winParticles.SetActive(true);
                winAmount = (RefMgr.potHandler.GetPotAmount * 2) + (RefMgr.potHandler.GetPotAmount / 2);
                winAmountTxt.text = "Jackpot! Huge win!\n+$" + winAmount;
                StartCoroutine(ShowWinPanel("Jackpot", winAmount));
                LocalSettingBlackJack.TotalJackPOT++;

                tgtPos = cd.chipsPosRect.transform.position + Vector3.down * 1000;
                initPos = cd.chipsPosRect.transform.position + Vector3.up * 2500;
                StartCoroutine(CloneAndSendChipsOnWinLoose(cd, initPos, tgtPos));
                SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.jackpotSound);
                break;

            case Winner.BUST:
                winAmount = 0;
                winAmountTxt.text = "You busted! Better luck next time!";
                StartCoroutine(ShowWinPanel("Bust", winAmount));
                LocalSettingBlackJack.TotalGamesLost++;

                tgtPos = cd.chipsPosRect.transform.position + Vector3.up * 2500;
                initPos = cd.chipsPosRect.transform.position;
                StartCoroutine(CloneAndSendChipsOnWinLoose(cd, initPos, tgtPos));
                SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.loseSound);
                break;

            case Winner.WON:
                winAmount = RefMgr.potHandler.GetPotAmount * 2;
                if (RefMgr.hitStandBarHandler.isDoubleBet)
                    winAmount = winAmount * 2;
                winAmountTxt.text = "You beat the dealer!\n+$" + winAmount;
                StartCoroutine(ShowWinPanel("Won", winAmount));
                LocalSettingBlackJack.TotalGamesWon++;

                tgtPos = cd.chipsPosRect.transform.position + Vector3.down * 1000;
                initPos = cd.chipsPosRect.transform.position + Vector3.up * 2500;
                StartCoroutine(CloneAndSendChipsOnWinLoose(cd, cd.chipsPosRect.transform.position, tgtPos));
                SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.winSound);
                break;
        }
    }

    /// <summary>
    /// Collects all cards from the table and moves them to the waste pile.
    /// </summary>
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

    /// <summary>
    /// Resets all game data and clears the table for a new round.
    /// </summary>
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

    #region Card Animation

    /// <summary>
    /// Animates a card to a target position and shows its dummy skin.
    /// </summary>
    void PlayCardAnimation(GameObject objectToAnimate, GameObject targetObj)
    {
        if (objectToAnimate == null || targetObj == null)
        {
            Debug.LogError("Object to animate or target object is null.");
            return;
        }
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.cardCollect);
        objectToAnimate.transform.SetParent(targetObj.transform.parent);
        objectToAnimate.transform.DOMove(targetObj.transform.position, 0.2f)
            .OnComplete(() => OnCompleteShowDummyCard(objectToAnimate));
        objectToAnimate.transform.DORotateQuaternion(targetObj.transform.rotation, 0.2f);
    }

    /// <summary>
    /// Callback to show the dummy skin of a card after animation.
    /// </summary>
    void OnCompleteShowDummyCard(GameObject obj)
    {
        obj.GetComponent<CardProperty>().ShowDummySkin();
    }

    #endregion

    #region Card Splitting

    /// <summary>
    /// Splits the current hand into two new hands, or splits a split hand further if possible.
    /// </summary>
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
}
