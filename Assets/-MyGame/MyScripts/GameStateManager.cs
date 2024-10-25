using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    //public enum GameState
    //{
    //    CHOOSINGBET,
    //    CARDDROP,
    //    BETRAISE,
    //    STAND,
    //    RESULT
    //}

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

    }
    void OnCardDrop()
    {

    }
    void OnBetRaise()
    {

    }
    void OnStand()
    {

    }
    void OnResult()
    {

    }
}
