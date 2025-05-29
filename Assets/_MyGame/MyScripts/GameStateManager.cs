using UnityEngine;

/// <summary>
/// Manages the overall game state transitions for the blackjack game.
/// Handles state changes, invokes appropriate logic for each state, and coordinates UI and gameplay flow.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField] Rm refMgr; // Reference to the main resource manager

    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the game state to CHOOSINGBET at the start of the game.
    /// </summary>
    void Start()
    {
        UpDateGameState(GameState.State.CHOOSINGBET);
    }

    #endregion

    #region State Management

    /// <summary>
    /// Gets the current game state.
    /// </summary>
    /// <returns>The current GameState.State value.</returns>
    public GameState.State GetCurrentGameState()
    {
        return GameState.currentState;
    }

    /// <summary>
    /// Updates the game state and triggers the corresponding state handler.
    /// </summary>
    /// <param name="statee">The new game state to set.</param>
    public void UpDateGameState(GameState.State statee)
    {
        GameState.currentState = statee;
        OnUpdateGameState();
    }

    /// <summary>
    /// Invokes the appropriate handler based on the current game state.
    /// </summary>
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

    #endregion

    #region State Handlers

    /// <summary>
    /// Handler for the CHOOSINGBET state. Prepares the UI for betting.
    /// </summary>
    void OnChoosingBet()
    {
        refMgr.betBarHandler.CreateBetButtons();
    }

    /// <summary>
    /// Handler for the CARDDROP state. Starts the initial card dealing.
    /// </summary>
    void OnCardDrop()
    {
        refMgr.tableDealer.FirstTimeDealCards();
    }

    /// <summary>
    /// Handler for the BETRAISE state. Compares scores and shows hit/stand options.
    /// </summary>
    void OnBetRaise()
    {
        refMgr.hitStandBarHandler.CompareScoresAfter4Cards();
    }

    /// <summary>
    /// Handler for the STAND state. Hides hit/stand bar and starts dealer's turn.
    /// </summary>
    void OnStand()
    {
        refMgr.hitStandBarHandler.ShowHitStandBar(false);
        refMgr.dealerAIPlay.DropDealerCard();
    }

    /// <summary>
    /// Handler for the RESULT state. Resets the game and prepares for a new round.
    /// </summary>
    void OnResult()
    {
        refMgr.hitStandBarHandler.ShowHitStandBar(false);
        refMgr.tableDealer.ResetWholeGame();
        refMgr.betBarHandler.ShowBetbar(false);
        refMgr.betBarHandler.ResetThings();

        refMgr.betBarHandler.CreateBetButtons();
        refMgr.potHandler.ResetInsuranceAmount();
        refMgr.hitStandBarHandler.isDoubleBet = false;
        refMgr.gameStateManager.UpDateGameState(GameState.State.CHOOSINGBET);
    }

    #endregion
}
