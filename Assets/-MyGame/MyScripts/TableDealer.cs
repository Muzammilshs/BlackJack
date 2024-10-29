using System.Collections;
using UnityEngine;

public class TableDealer : MonoBehaviour
{
    [SerializeField] Rm RefMgr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void FirstTimeDealCards()
    {
        StartCoroutine(FirstTimeDealCardsCrt());
    }

    IEnumerator FirstTimeDealCardsCrt()
    {
        Debug.LogError("distributing card  2");
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 4; i++)
        {

        }
    }
}
