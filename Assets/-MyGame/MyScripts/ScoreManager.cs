using com.mani.muzamil.amjad;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [ShowOnly]
    public int playerTotalScores, dealerTotalScores;
    [SerializeField] TMP_Text playerScores;
    [SerializeField] TMP_Text dealerScores;

    [SerializeField] GameObject playerScoresParent;
    [SerializeField] GameObject dealerScoresParent;


    public void ShowScoreObjects(bool isShow)
    {
        if (!isShow)
        {
            playerTotalScores = 0;
            dealerTotalScores = 0;
        }
        playerScoresParent.SetActive(isShow);
        dealerScoresParent.SetActive(isShow);
    }

    public void SetScores(CardProperty cp, bool isPlayer)
    {
        ShowScoreObjects(true);
        if (isPlayer)
        {
            playerTotalScores += cp.Power;
            playerScores.text = playerTotalScores.ToString();
        }
        else
        {
            dealerTotalScores += cp.Power;
            dealerScores.text = playerTotalScores.ToString();
        }
    }
}
