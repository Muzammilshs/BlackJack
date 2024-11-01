using System.Collections;
using UnityEngine;

public class TaskHandler : MonoBehaviour
{
    enum Tasks
    {
        Disable,
        Destroy
    }

    [SerializeField] Tasks currentTask;
    [SerializeField] float timeDelay = 3f;
    public void PerformCurrentTask()
    {
        switch (currentTask)
        {
            case Tasks.Disable:
            case Tasks.Destroy:
                StartCoroutine(DisableOrDestroyObject());
                break;
            default:
                break;
        }
    }

    IEnumerator DisableOrDestroyObject()
    {
        yield return new WaitForSeconds(timeDelay);
        if (currentTask == Tasks.Disable)
            gameObject.SetActive(false);
        else if (currentTask == Tasks.Destroy)
            Destroy(gameObject);
    }
}
