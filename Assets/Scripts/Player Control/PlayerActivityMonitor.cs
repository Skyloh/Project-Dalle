using UnityEngine;

public class PlayerActivityMonitor : MonoBehaviour
{
    [SerializeField] PlayerDataSO data;
    [SerializeField] PlayerMovement pmScript;
    [SerializeField] PlayerCameraBehavior pcScript;

    bool previous;

    // a status-based player toggle is the neatest thing i can do here
    void Update()
    {
        // only triggers the frame when player enabled changes
        if (!previous && data.IS_PLAYER_ENABLED || previous && !data.IS_PLAYER_ENABLED)
        {
            pmScript.enabled = pcScript.enabled = data.IS_PLAYER_ENABLED;
        }

        previous = data.IS_PLAYER_ENABLED;
    }
}
