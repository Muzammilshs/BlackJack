using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class ToggleAnimationController : MonoBehaviour
{
    [SerializeField] private Toggle[] animationToggles;     
    [SerializeField] private GameObject[] animatedObjects;  

    private List<Tween> tweens = new List<Tween>();  

    private void Start()
    {   
        for (int i = 0; i < animatedObjects.Length; i++)
        {
            int index = i;
            
            tweens.Add(SetupAnimation(animatedObjects[i])); 

          
            animationToggles[i].onValueChanged.AddListener((isOn) => ToggleAnimation(index, isOn));

            
            ToggleAnimation(index, animationToggles[i].isOn);
        }
    }

    
    private Tween SetupAnimation(GameObject obj)
    {
        
       
        return obj.transform.DOMove(new Vector3(0, 5, 0), 2f).SetLoops(-1, LoopType.Yoyo); 
    }

   
    private void ToggleAnimation(int index, bool isOn)
    {
        if (isOn)
        {
            tweens[index].Play();  
        }
        else
        {
            tweens[index].Pause();  
        }
    }
}