using UnityEngine;

public class CameraFollw : MonoBehaviour
{
    public Transform player;
    public Transform[] allPlayers;
    public Transform initialPos;
    int playerIndex;
    private void LateUpdate()
    {
        float distance = 0;
        for (int i = 0; i < allPlayers.Length; i++)
        {
            float currentDistance = allPlayers[i].position.z;
            if (distance < currentDistance)
            {
                distance = currentDistance;
                playerIndex = i;
            }
        }
        player = allPlayers[playerIndex];
    }
}
