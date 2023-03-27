using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRaycastable : MonoBehaviour, IRaycastable
{
    [SerializeField] NPCController controller;

    public bool OnHit(Transform t)
    {
        return controller.ProcessHit(t);
    }
}
