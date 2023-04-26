using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] PlayerDataSO data;

    private void Start()
    {
        data.ScrambleAndInitPaintings();
    }
}
