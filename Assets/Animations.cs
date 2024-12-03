using UnityEngine;
using DG.Tweening;

public class Animations : MonoBehaviour
{
    public RectTransform settingAnimation;


    void Start()
    {
        CloseAnimation();
        // settingAnimation.DOAnchorPos(Vector2.zero , 0.5f, false);
    }

    public void OpenAnimation()
    {
        settingAnimation.DOAnchorPos(new Vector2(0f , 0f), 0.5f, false);

    }

    public void CloseAnimation()
    {
        settingAnimation.DOAnchorPos(new Vector2(0f , 2450f), 0.5f, false);
    }

}
