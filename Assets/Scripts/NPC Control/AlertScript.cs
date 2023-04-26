using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertScript : MonoBehaviour
{
    [SerializeField] float anim_speed = 50f;
    Quaternion original_rotation;

    private void Awake()
    {
        original_rotation = transform.rotation;
    }

    private void OnEnable()
    {
        transform.rotation = original_rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(anim_speed * Time.deltaTime * transform.up);
    }
}
