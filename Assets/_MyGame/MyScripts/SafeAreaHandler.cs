using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        // Prevent unnecessary updates
        if (safeArea == lastSafeArea)
            return;

        lastSafeArea = safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }

    void FixedUpdate()
    {
        // Reapply safe area if the screen rotates or changes
        if (Screen.safeArea != lastSafeArea)
        {
            ApplySafeArea();
        }
    }
}
