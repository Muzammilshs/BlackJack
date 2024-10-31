using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Rm refMgr;

    public Sprite dummyCardSprite;
    void Start()
    {

    }

    public List<GameObject> ClearList(List<GameObject> list)
    {
        if (list == null)
        {
            list = new List<GameObject>();
            return list;
        }
        if (list.Count > 0)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                Destroy(list[i]);
            list.Clear();
        }
        return list;
    }
}
