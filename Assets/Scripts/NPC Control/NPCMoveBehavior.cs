using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCMoveBehavior : MonoBehaviour
{
    public bool isEnroute;
    public bool wasPathCanceled;
    public bool isPaused;

    Queue<Vector3> corners;
    Vector3 destination;
    Vector3 end_normal;
    float distance_to_destination;

    Vector3 facing;
    RaycastHit raycastDown;
    const int mask = ~((1 << 8) | (1 << 2));

    [SerializeField] float MIN_STEP_DISTANCE = 1f;
    [SerializeField] [Tooltip("This value is squared")] float DESTINATION_FIDELITY = 0.5f;

    public void SetupPath(NavMeshPath path, Vector3 normal)
    {
        corners = new Queue<Vector3>();

        end_normal = normal;

        foreach (Vector3 vector in path.corners)
        {
            corners.Enqueue(vector);
        }

        wasPathCanceled = false;

        GoToNextCorner();
    }

    void GoToNextCorner()
    {
        if (corners.Count > 0)
        {
            destination = corners.Dequeue();

            // we can sometimes get a 0, 0, 0 corner. fix that.
            if ((destination - transform.position).sqrMagnitude > 0.5f) {
                facing = destination - transform.position;

                facing.y = 0;
                facing = facing.normalized;

                StopAllCoroutines();

                StartCoroutine(IERotateTowards());
            }

            isEnroute = true;
        }

        else
        {
            StopAllCoroutines();

            facing = -end_normal;

            StartCoroutine(IERotateTowards());

            isEnroute = false;
        }
    }

    private void Update()
    {
        if (isEnroute)
        {
            if (isPaused)
            {
                return;
            }

            MoveNPC();

            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out raycastDown, 5f, mask))
            {
                Vector3 desired = transform.position;
                desired.y = Mathf.Lerp(transform.position.y, raycastDown.point.y, 0.1f);
                transform.position = desired;
            }

            if (Physics.Raycast(transform.position + Vector3.up, facing, 0.25f)) {
                wasPathCanceled = true;

                isEnroute = false;

                corners = new Queue<Vector3>();
            }
        }
    }

    IEnumerator IERotateTowards()
    {
        while ((transform.forward - facing).sqrMagnitude > 0.5f)
        {
            yield return new WaitForEndOfFrame();

            transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation(facing, Vector3.up), 0.0125f);
        }

        transform.forward = facing;
    }

    void MoveNPC()
    {
        distance_to_destination = (transform.position - destination).sqrMagnitude;

        if (distance_to_destination > DESTINATION_FIDELITY * DESTINATION_FIDELITY)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, MIN_STEP_DISTANCE * Time.deltaTime);
        }

        else
        {
            GoToNextCorner();
        }
    }
}
