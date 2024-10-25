using UnityEngine;

public class Rm : MonoBehaviour
{
    #region Creating Instance
    private static Rm _instance;
    public static Rm Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindAnyObjectByType<Rm>();
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    #endregion
}
