using UnityEngine;

public class PolicyScript : MonoBehaviour
{
    public string supportURL = "https://www.termsfeed.com/live/afac3d92-8eee-4b99-9972-27bb0217cc7e";
    public string policyURL = "https://www.termsfeed.com/live/afac3d92-8eee-4b99-9972-27bb0217cc7e";

    public void OpenSupport()
    {
        Application.OpenURL(supportURL);
    }

    public void OpenPolicy()
    {
        Application.OpenURL(policyURL);
    }

    
}
