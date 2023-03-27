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
        data.IS_PLAYER_ENABLED = true;
    }

    private void FixedUpdate()
    {
        if (Input.GetAxisRaw("Fire1") > 0f)
        {
            if (data.IS_PLAYER_ENABLED
                && Physics.Raycast(transform.position, transform.forward, out info, data.MAX_DISTANCE))
            {
                Collider collider = info.collider;

                if (!previous.Equals(collider.name) && collider.TryGetComponent(out IRaycastable hit))
                {
                    if (process != null)
                    {
                        StopCoroutine(process);
                    }

                    data.IS_PLAYER_ENABLED = hit.OnHit(transform);

                    previous = collider.name;

                    process = IEUnset();
                    StartCoroutine(process);
                }
            }
        }
    }

    IEnumerator IEUnset()
    {
        yield return new WaitUntil(() => data.IS_PLAYER_ENABLED);
        yield return new WaitForSeconds(0.15f);

        previous = "";
    }
}
