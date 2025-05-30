using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CanvasGroupFader : MonoBehaviour
{
    [SerializeField] float fadeDuration = 0.3f;
    [SerializeField] CanvasGroup[] _activateThis;
    [SerializeField] CanvasGroup[] _deactivateThis;

    Button btn;

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClickAction);
    }

    void BtnClickAction()
    {
        // Fade in active ones
        foreach (CanvasGroup cg in _activateThis)
        {
            cg.gameObject.SetActive(true); // optional: enable object if disabled
            cg.DOFade(1f, fadeDuration).SetUpdate(UpdateType.Normal);
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        // Fade out inactive ones
        foreach (CanvasGroup cg in _deactivateThis)
        {
            cg.DOFade(0f, fadeDuration).SetUpdate(UpdateType.Normal).OnComplete(() =>
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;
            });
        }
    }
}
