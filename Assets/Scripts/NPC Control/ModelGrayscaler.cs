using UnityEngine;

public class ModelGrayscaler : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer bodyMesh, eyeMesh;

    private void Start()
    {
        float v = Mathf.Pow(Random.Range(0f, 1f), 2f);
        float vInv = 1f - v;

        bodyMesh.material.color = new Color(v, v, v);
        eyeMesh.material.color = new Color(vInv, vInv, vInv);
    }
}
