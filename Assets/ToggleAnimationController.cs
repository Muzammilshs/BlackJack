using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 
using System.Collections.Generic;

public class ToggleAnimationController : MonoBehaviour
{
    public Toggle onToggle; 
    public Toggle offToggle; 
    public List<GameObject> animatedObjects; 

    private List<DOTweenAnimation> dotweenAnimations = new List<DOTweenAnimation>();

    private void Start()
    {
        
        foreach (GameObject obj in animatedObjects)
        {
            DOTweenAnimation anim = obj.GetComponent<DOTweenAnimation>();
            if (anim != null)
            {
                dotweenAnimations.Add(anim);
            }
            else
            {
                Debug.LogWarning("DOTweenAnimation component missing on " + obj.name);
            }
        }

        
        onToggle.onValueChanged.AddListener(OnToggleChanged);
        offToggle.onValueChanged.AddListener(OffToggleChanged);

        
        onToggle.isOn = true;
        offToggle.isOn = false;
    }

    private void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            offToggle.isOn = false; 

            
            foreach (DOTweenAnimation anim in dotweenAnimations)
            {
                anim.DOPlay();
            }
        }
    }

    private void OffToggleChanged(bool isOn)
    {
        if (isOn)
        {
            onToggle.isOn = false; 

            
            foreach (DOTweenAnimation anim in dotweenAnimations)
            {
                anim.DOPause();
            }
        }
    }

    private void OnDestroy()
    {
       
        onToggle.onValueChanged.RemoveListener(OnToggleChanged);
        offToggle.onValueChanged.RemoveListener(OffToggleChanged);
    }
}
