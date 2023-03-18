using System.Collections;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] float LERP = 0.0125f;
    [SerializeField] float ACCURACY = 0.95f;

    Vector3 original;
    IEnumerator process;

    void Start()
    {
        original = transform.localPosition;
    }

    public void LerpToDestination(Vector3 dest, bool local_space = false)
    {
        if (process != null)
        {
            StopCoroutine(process);
        }

        process = IELerpToDestination(dest, local_space);

        StartCoroutine(process);
    }

    public void Reset()
    {
        LerpToDestination(original, true);
    }

    public IEnumerator IELerpToDestination(Vector3 dest, bool local_space)
    {
        float progress = 0f;

        Vector3 result = local_space ? transform.localPosition : transform.position;

        while (progress < ACCURACY)
        {
            progress = Mathf.Lerp(progress, 1f, LERP);

            result = Vector3.Lerp(result, dest, LERP);

            if (local_space)
            {
                transform.localPosition = result;
            }

            else
            {
                transform.position = result;
            }

            yield return new WaitForFixedUpdate();
        }

        if (local_space)
        {
            transform.localPosition = dest;
        }

        else
        {
            transform.position = dest;
        }
    }
}
