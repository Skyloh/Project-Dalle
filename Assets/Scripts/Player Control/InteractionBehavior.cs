using UnityEngine;

public class InteractionBehavior : MonoBehaviour
{
    RaycastHit info;

    [SerializeField] PlayerDataSO data;

    private void Update()
    {
        if (Input.GetAxisRaw("Fire1") > 0f)
        { 
            if (data.CAN_CAMERA_RAYCAST && Physics.Raycast(transform.position, transform.forward, out info, data.MAX_DISTANCE))
            {
                IRaycastable hit = info.collider.gameObject.GetComponent<IRaycastable>();

                hit.OnHit(transform);
            }
        }
    }
}
