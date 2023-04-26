using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] PlayerDataSO data;

    private void OnEnable()
    {
        data.ScrambleAndInitPaintings();
    }
}
