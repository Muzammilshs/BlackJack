using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] Rm refMgr;
    //GameState currentState;
    void Start()
    {
        UpDateGameState(GameState.State.CHOOSINGBET);
    }

    public GameState.State GetCurrentGameState()
    {
        return GameState.currentState;
    }

    public void UpDateGameState(GameState.State statee)
    {
        GameState.currentState = statee;
        OnUpdateGameState();
    }

    void OnUpdateGameState()
    {
        switch (GameState.currentState)
        {
            case GameState.State.CHOOSINGBET:
                OnChoosingBet(); break;
            case GameState.State.CARDDROP:
                OnCardDrop(); break;
            case GameState.State.BETRAISE:
                OnBetRaise(); break;
            case GameState.State.STAND:
                OnStand(); break;
            case GameState.State.RESULT:
                OnResult(); break;
        }
    }

    void OnChoosingBet()
    {
        refMgr.betBarHandler.CreateBetButtons();

    }
    void OnCardDrop()
    {
        refMgr.tableDealer.FirstTimeDealCards();
    }
    void OnBetRaise()
    {
        refMgr.hitStandBarHandler.CompareScoresAfter4Cards();
    }
    void OnStand()
    {
        refMgr.dealerAIPlay.isDealerTurn = true;
        refMgr.hitStandBarHandler.ShowHitStandBar(false);
        refMgr.dealerAIPlay.DropDealerCard();
    }
    void OnResult()
    {
        refMgr.hitStandBarHandler.ShowHitStandBar(false);

        StartCoroutine(ResetGame());
    }

    IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(1);
        refMgr.dealerAIPlay.ResetDealer();
        refMgr.cardsManager.SendCardsToWasteCardsPos();
        refMgr.scoreManager.ResetTotalScore();
        refMgr.betBarHandler.ShowBetbar(false);
        yield return new WaitForSeconds(1);
        refMgr.hitStandBarHandler.ShowHitStandBar(true);
        refMgr.betBarHandler.ResetThings();

    }


}
