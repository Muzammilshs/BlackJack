using com.muzammil;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages a hand of cards, their scores, betted chips, and related UI for a blackjack hand (player or dealer).
/// Handles adding cards, score calculation, splitting, and chip management.
/// </summary>
public class CardsData : MonoBehaviour
{
    #region Fields and Properties

    /// <summary>Rank of the hand (for sorting or comparison).</summary>
    public int handRank;

    /// <summary>Type of hand (player, dealer, split, etc.).</summary>
    public HandType.HANDTYPE handType;

    [SerializeField] GameObject _scoresParent; // UI parent for score display
    [SerializeField] TMP_Text _scoresText;     // UI text for scores
    [SerializeField] RectTransform _currentCardPos; // Position for new cards
    public RectTransform chipsPosRect;         // Position for chips

    [ShowOnly] public List<GameObject> chipsList = new List<GameObject>(); // Chips for this hand
    [SerializeField] TMP_Text totalBettedAmontTxt; // UI text for total betted amount

    [ShowOnly] public int highScores;          // Highest possible score (with Ace as 11)
    [ShowOnly] public int lowScores;           // Lowest possible score (with Ace as 1)
    [ShowOnly] public List<CardProperty> cardsList = new List<CardProperty>(); // Cards in this hand

    [Header("Double Betted Chips, only for player")]
    [SerializeField] TMP_Text _doubleBettedAmuntTxt; // UI text for double bet amount
    public RectTransform _doubleBettedChipsPosRect;  // Position for double bet chips
    [ShowOnly] public List<GameObject> _doubleBettedChipsList = new List<GameObject>(); // Double bet chips

    /// <summary>Returns true if this hand has at least one card.</summary>
    public bool isValidHand { get { return cardsList.Count < 1 ? false : true; } }

    int cardDistance; // Distance between cards for layout

    #endregion

    #region Unity Methods

    private void Start()
    {
        int screenWidth = Screen.width;
        cardDistance = screenWidth / 25;
    }

    #endregion

    #region Card Management

    /// <summary>
    /// Adds a card to this hand and animates it to the correct position.
    /// </summary>
    public void AddCard(CardProperty card, bool isFlip)
    {
        playCardAnimation(card.gameObject, _currentCardPos.gameObject, cardsList.Count, isFlip);
        cardsList.Add(card);
    }

    /// <summary>
    /// Resets all card and bet data for this hand.
    /// </summary>
    public void ResetCardDataThings()
    {
        highScores = 0;
        lowScores = 0;
        _scoresParent.SetActive(false);
        _scoresText.text = "";
        SetBettedAmount("");

        foreach (CardProperty card in cardsList)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        cardsList.Clear();

        ResetBettedChips();
    }

    /// <summary>
    /// Destroys all chips in this hand.
    /// </summary>
    public void ClearAllChips()
    {
        foreach (GameObject chip in chipsList)
        {
            if (chip != null)
                Destroy(chip);
        }
        chipsList.Clear();
    }

    #endregion

    #region Score Calculation and Display

    /// <summary>
    /// Calculates the high and low possible scores for the current cards (handles Aces).
    /// </summary>
    void CalculateCardsScores(List<CardProperty> cardsList, out int highScres, out int lowScres)
    {
        highScres = 0;
        lowScres = 0;
        int aceCount = 0;

        // Calculate initial lowScres, treating Aces as 1
        foreach (CardProperty card in cardsList)
        {
            if (card.Card == CardState.CARDVALUE.ACE)
            {
                aceCount++;
                lowScres += card.SecondPower; // Assume Ace is 1 initially
            }
            else
            {
                lowScres += card.Power;
            }
        }

        // High score calculation: Try to upgrade one Ace to 11 if it doesn't bust
        highScres = lowScres;
        if (aceCount > 0 && highScres + 10 <= LocalSettingBlackJack.ScoresLimit)
        {
            highScres += 10; // Upgrade one Ace from 1 to 11
        }
    }

    /// <summary>
    /// Updates the score label UI for this hand.
    /// </summary>
    void LabelScores()
    {
        CalculateCardsScores(cardsList, out int highScres, out int lowScres);
        highScores = highScres;
        lowScores = lowScres;
        if (!_scoresParent.activeInHierarchy)
            _scoresParent.SetActive(true);
        if (lowScores != highScores)
            _scoresText.text = $"{lowScores}/{highScores}";
        else
            _scoresText.text = highScores.ToString();
    }

    /// <summary>
    /// Shows only the high score in the UI.
    /// </summary>
    public void ShowJustHighScores()
    {
        _scoresText.text = highScores.ToString();
    }

    #endregion

    #region Betting and Chips

    /// <summary>
    /// Sets the betted amount text in the UI.
    /// </summary>
    public void SetBettedAmount(string amount)
    {
        if (totalBettedAmontTxt != null)
        {
            if (amount == "")
            {
                totalBettedAmontTxt.gameObject.SetActive(false);
            }
            else
            {
                totalBettedAmontTxt.text = amount;
                if (!totalBettedAmontTxt.gameObject.activeInHierarchy)
                    totalBettedAmontTxt.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Total Betted Amount Text is null");
        }
    }

    /// <summary>
    /// Splits this hand into two new hands, moving one card to each and cloning chips.
    /// </summary>
    public void SplitThisSetTo2(CardsData cardsData1, CardsData cardsData2)
    {
        cardsData1.AddCard(cardsList[0], true);
        cardsData2.AddCard(cardsList[1], true);

        cardsList.Clear();
        _scoresParent.SetActive(false);
        SetBettedAmount("");

        cardsData1.SetBettedAmount(totalBettedAmontTxt.text);
        cardsData2.SetBettedAmount(totalBettedAmontTxt.text);

        StartCoroutine(cloneChips(cardsData1));
        StartCoroutine(cloneChips(cardsData2));
    }

    /// <summary>
    /// Clones chips from this hand to another hand (used during split).
    /// </summary>
    IEnumerator cloneChips(CardsData cardsDataa)
    {
        CardsData cardsData = cardsDataa;
        for (int i = 0; i < chipsList.Count; i++)
        {
            GameObject newChip = Instantiate(chipsList[i]);
            LocalSettingBlackJack.SetPositionAndRectTransform(newChip, cardsData.chipsPosRect, cardsData.chipsPosRect.transform.parent);
            newChip.SetActive(true);
            cardsData.chipsList.Add(newChip);
            newChip.transform.position = chipsList[i].transform.position;
            playChipAnimation(newChip, cardsData.chipsPosRect.gameObject, i);

            chipsList[i].SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
        Invoke(nameof(ResetBettedChipsOnSplit), Random.Range(1f, 2f));
    }

    /// <summary>
    /// Resets chips after a split (destroys or hides them as appropriate).
    /// </summary>
    void ResetBettedChipsOnSplit()
    {
        if (handType != HandType.HANDTYPE.PLAYERHAND)
        {
            if (chipsList.Count > 0)
            {
                foreach (var chips in chipsList)
                {
                    if (chips)
                        Destroy(chips);
                }
            }
        }
        else
        {
            foreach (var chips in chipsList)
            {
                if (chips)
                    chips.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Resets all betted chips and double bet chips for this hand.
    /// </summary>
    void ResetBettedChips()
    {
        if (handType != HandType.HANDTYPE.PLAYERHAND)
        {
            if (chipsList.Count > 0)
            {
                foreach (var chips in chipsList)
                {
                    if (chips)
                        Destroy(chips);
                }
            }
        }
        else
        {
            foreach (var chips in chipsList)
            {
                if (chips)
                    chips.SetActive(true);
            }
            SetBettedAmount(Rm.Instance.potHandler.GetPotAmount.ToString());

            if (_doubleBettedChipsList.Count > 0)
            {
                foreach (GameObject chip in _doubleBettedChipsList)
                    if (chip != null) Destroy(chip);
                _doubleBettedChipsList.Clear();
                _doubleBettedAmuntTxt.text = "";
                _doubleBettedAmuntTxt.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Activates double bet UI and chips for this hand.
    /// </summary>
    public void ActivateDoubleBetItems()
    {
        if (_doubleBettedAmuntTxt != null)
        {
            PotHandler ph = Rm.Instance.potHandler;
            _doubleBettedAmuntTxt.gameObject.SetActive(true);
            _doubleBettedAmuntTxt.text = ph.GetPotAmount.ToString();
            // chips creation
            StartCoroutine(AddDoubleBettedChipsCoroutine());
        }
    }

    /// <summary>
    /// Coroutine to add double bet chips with animation.
    /// </summary>
    IEnumerator AddDoubleBettedChipsCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < chipsList.Count; i++)
        {
            GameObject newChip = Instantiate(chipsList[i]);
            AddDoubleBettedChips(newChip);
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Adds a single double bet chip to the UI and animates it.
    /// </summary>
    void AddDoubleBettedChips(GameObject chip)
    {
        if (chip != null)
        {
            _doubleBettedChipsList.Add(chip);
            LocalSettingBlackJack.SetPositionAndRectTransform(chip, chipsPosRect, chipsPosRect.transform.parent);
            chip.SetActive(true);
            playChipAnimation(chip, _doubleBettedChipsPosRect.gameObject, _doubleBettedChipsList.Count - 1);
        }
        else
        {
            Debug.LogError("Double Betted Chip is null");
        }
    }

    /// <summary>
    /// Animates a chip to its target position.
    /// </summary>
    void playChipAnimation(GameObject ObjectToAnimate, GameObject targetObj, int chipIndex)
    {
        GameObject TgtObj = targetObj;
        GameObject chip = ObjectToAnimate;

        Vector3 tgtPos = TgtObj.transform.position + new Vector3(Random.Range(-10, 11), chipIndex * 2f, 0);
        chip.transform.DOMove(tgtPos, 0.25f);

        chip.transform.DORotateQuaternion(targetObj.transform.rotation, 0.25f);
    }

    #endregion

    #region Card Animation

    /// <summary>
    /// Animates a card to its position and flips it if needed.
    /// </summary>
    void playCardAnimation(GameObject ObjectToAnimate, GameObject targetObj, int offSet, bool shouldFLip)
    {
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.cardDrop);
        Vector3 cardPos = ObjectToAnimate.transform.position;
        GameObject card = ObjectToAnimate;
        GameObject TgtObj = targetObj;
        LocalSettingBlackJack.SetPositionAndRectTransform(card, TgtObj.GetComponent<RectTransform>(), TgtObj.transform.parent);
        card.transform.position = cardPos;

        Vector3 tgtPos = TgtObj.transform.position + Vector3.right * offSet * cardDistance;
        ObjectToAnimate.transform.DOMove(tgtPos, 0.25f)
            .OnComplete(() => FlipCard(card, shouldFLip));

        ObjectToAnimate.transform.DORotateQuaternion(targetObj.transform.rotation, 0.25f);
    }

    /// <summary>
    /// Flips a card with animation, or moves it up if not flipping.
    /// </summary>
    public void FlipCard(GameObject obj, bool shouldFLip)
    {
        if (!shouldFLip)
        {
            MoveSlightlyUp(obj);
        }
        else
        {
            SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.cardFlip);
            obj.transform.DORotate(Vector2.up * 90, 0.25f)
                .OnComplete(() => ReverseRotate(obj));
        }
    }

    /// <summary>
    /// Moves a card slightly up and down for visual effect.
    /// </summary>
    void MoveSlightlyUp(GameObject obj)
    {
        SoundManagerBJ.Instance.PlayAudioClip(SoundManagerBJ.AllSounds.cardSlightlyUp);
        Vector3 targetPosition = obj.transform.position + Vector3.up * 100;
        obj.transform.DOMove(targetPosition, 0.25f).SetLoops(2, LoopType.Yoyo);
    }

    /// <summary>
    /// Completes the card flip and updates the card's sprite and scores.
    /// </summary>
    void ReverseRotate(GameObject obj)
    {
        obj.GetComponent<CardProperty>().ShowOriginalSprite();
        obj.transform.DORotate(Vector2.zero, 0.25f);

        LabelScores();
    }

    #endregion
}
