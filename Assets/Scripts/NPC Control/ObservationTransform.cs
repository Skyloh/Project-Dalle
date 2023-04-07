using UnityEngine;

[System.Serializable]
public class ObservationTransform
{
    public Vector3 world_position;
    public Vector3 painting_normal;
    public Vector2 max_observational_area;

    public ObservationTransform(Transform t)
    {
        world_position = t.position;
        painting_normal = t.forward;
        max_observational_area = new Vector2(t.localScale.x * 0.5f, t.localScale.y * 0.5f);
    }
}
