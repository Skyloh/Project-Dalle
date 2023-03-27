using UnityEngine;
using System.Collections;

public class InteractionBehavior : MonoBehaviour
{
    RaycastHit info;

    string previous = "";
    IEnumerator process;

    [SerializeField] PlayerDataSO data;

    private void Start()
    {
        data.CAN_CAMERA_RAYCAST = true;
    }

    private void FixedUpdate()
    {
        if (Input.GetAxisRaw("Fire1") > 0f)
        {
            if (data.CAN_CAMERA_RAYCAST
                && Physics.Raycast(transform.position, transform.forward, out info, data.MAX_DISTANCE))
            {
                Collider collider = info.collider;

                if (!previous.Equals(collider.name) && collider.TryGetComponent(out IRaycastable hit))
                {
                    if (process != null)
                    {
                        StopCoroutine(process);
                    }

                    data.CAN_CAMERA_RAYCAST = hit.OnHit(transform);

                    previous = collider.name;

                    process = IEUnset();
                    StartCoroutine(process);
                }
            }
        }
    }

    IEnumerator IEUnset()
    {
        yield return new WaitUntil(() => data.CAN_CAMERA_RAYCAST);
        yield return new WaitForSeconds(0.15f);

        previous = "";
    }
}
