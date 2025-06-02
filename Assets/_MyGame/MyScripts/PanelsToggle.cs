using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CanvasGroupFader : MonoBehaviour
{
    #region Inspector Fields

    // Duration of the fade animation in seconds
    [SerializeField] float fadeDuration = 0.3f;

    // CanvasGroups to activate and fade in
    [SerializeField] CanvasGroup[] _activateThis;

    // CanvasGroups to deactivate and fade out
    [SerializeField] CanvasGroup[] _deactivateThis;

    #endregion

    #region Private Fields

    // Reference to the Button component on this GameObject
    Button btn;

    #endregion

    #region Unity Methods

    // Called when the script instance is being loaded
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClickAction);
    }

    #endregion

    #region Fade Logic

    /// <summary>
    /// Handles the button click event to fade in and out CanvasGroups.
    /// </summary>
    void BtnClickAction()
    {
        // Fade in and activate specified CanvasGroups
        foreach (CanvasGroup cg in _activateThis)
        {
            cg.gameObject.SetActive(true); // Ensure the object is enabled
            cg.DOFade(1f, fadeDuration).SetUpdate(UpdateType.Normal);
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        // Fade out and deactivate specified CanvasGroups
        foreach (CanvasGroup cg in _deactivateThis)
        {
            cg.DOFade(0f, fadeDuration).SetUpdate(UpdateType.Normal).OnComplete(() =>
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;
            });
        }
    }

    #endregion
}
