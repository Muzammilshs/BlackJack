using com.muzammil;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class CardsData : MonoBehaviour
{
    public int handRank;
    public HandType.HANDTYPE handType;
    [SerializeField] GameObject _scoresParent;
    [SerializeField] TMP_Text _scoresText;
    [SerializeField] RectTransform _currentCardPos;
    public RectTransform chipsPosRect;

    [ShowOnly] public List<GameObject> chipsList = new List<GameObject>();
    [SerializeField] TMP_Text totalBettedAmontTxt;

    [ShowOnly] public int highScores;
    [ShowOnly] public int lowScores;
    [ShowOnly] public List<CardProperty> cardsList = new List<CardProperty>();

    [Header("Double Betted Chips, only for player")]
    [SerializeField] TMP_Text _doubleBettedAmuntTxt;
    public RectTransform _doubleBettedChipsPosRect;
    [ShowOnly] public List<GameObject> _doubleBettedChipsList = new List<GameObject>();

    public bool isValidHand { get { return cardsList.Count < 1 ? false : true; } }

    int cardDistance;
    private void Start()
    {
        int screenWidth = Screen.width;
        cardDistance = screenWidth / 20; // Adjust this value as needed for card spacing
    }
    public void AddCard(CardProperty card, bool isFlip)
    {
        playCardAnimation(card.gameObject, _currentCardPos.gameObject, cardsList.Count, isFlip);
        cardsList.Add(card);

    }
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

    public void ClearAllChips()
    {
        foreach (GameObject chip in chipsList)
        {
            if (chip != null)
                Destroy(chip);
        }
        chipsList.Clear();
    }
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

    void playCardAnimation(GameObject ObjectToAnimate, GameObject targetObj, int offSet, bool shouldFLip)
    {
        Vector3 cardPos = ObjectToAnimate.transform.position;
        GameObject card = ObjectToAnimate;
        GameObject TgtObj = targetObj;
        LocalSettingBlackJack.SetPositionAndRectTransform(card, TgtObj.GetComponent<RectTransform>(), TgtObj.transform.parent);
        card.transform.position = cardPos;
        //card.transform.SetParent(TgtObj.transform.parent.transform);

        Vector3 tgtPos = TgtObj.transform.position + Vector3.right * offSet * cardDistance;
        ObjectToAnimate.transform.DOMove(tgtPos, 0.25f)
            .OnComplete(() => FlipCard(card, shouldFLip));

        ObjectToAnimate.transform.DORotateQuaternion(targetObj.transform.rotation, 0.25f);

    }
    public void FlipCard(GameObject obj, bool shouldFLip)
    {
        if (!shouldFLip)
        {
            MoveSlightlyUp(obj);
        }
        else
        {
            obj.transform.DORotate(Vector2.up * 90, 0.25f)
                .OnComplete(() => ReverseRotate(obj));
        }
    }
    void MoveSlightlyUp(GameObject obj)
    {
        Vector3 targetPosition = obj.transform.position + Vector3.up * 100;
        obj.transform.DOMove(targetPosition, 0.25f).SetLoops(2, LoopType.Yoyo);
    }

    public void ReverseRotate(GameObject obj)
    {
        obj.GetComponent<CardProperty>().ShowOriginalSprite();
        obj.transform.DORotate(Vector2.zero, 0.25f);

        LabelScores();
    }

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
    // clone chips
    // chips amount
    // 
    void playChipAnimation(GameObject ObjectToAnimate, GameObject targetObj, int chipIndex)
    {
        GameObject TgtObj = targetObj;
        GameObject chip = ObjectToAnimate;

        Vector3 tgtPos = TgtObj.transform.position + new Vector3(Random.Range(-10, 11), chipIndex * 2f, 0);
        chip.transform.DOMove(tgtPos, 0.25f);

        chip.transform.DORotateQuaternion(targetObj.transform.rotation, 0.25f);

    }
}
