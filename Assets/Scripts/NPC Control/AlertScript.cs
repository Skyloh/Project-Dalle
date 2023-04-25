using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertScript : MonoBehaviour
{
    [SerializeField] float anim_speed = 50f;
    float offset;

    private void Awake()
    {
        offset = Random.Range(0f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(anim_speed * Time.deltaTime * transform.up);

        transform.position += 0.1f * Mathf.Sin(Time.realtimeSinceStartup + offset) * Time.deltaTime * transform.up;
    }
}
