// MoveTo.cs
using UnityEngine;
using UnityEngine.AI;

public class TestScript : MonoBehaviour
{
    public SkinnedMeshRenderer rend;


    private void Update()
    {
        rend.SetBlendShapeWeight((int)Random.Range(0f, rend.sharedMesh.blendShapeCount), Random.Range(5f, 100f));
    }
}