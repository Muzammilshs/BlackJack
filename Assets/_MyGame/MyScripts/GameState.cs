using UnityEngine;

public class GameState : MonoBehaviour
{
    public enum State
    {
        CHOOSINGBET,
        CARDDROP,
        BETRAISE,
        STAND,
        RESULT
    }
    public static State currentState;
}
