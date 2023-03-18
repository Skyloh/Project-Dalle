using UnityEngine;

public class PlayerCameraBehavior : MonoBehaviour
{
    float xRot = 0f;
    Transform playerBody;

    [SerializeField] float SENSITIVITY = 10f;

    void Start()
    {
        playerBody = transform.parent.transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Mouse X") * SENSITIVITY * Time.deltaTime;
        float moveY = Input.GetAxis("Mouse Y") * SENSITIVITY * Time.deltaTime;

        playerBody.Rotate(Vector3.up, moveX);

        xRot -= moveY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);
    }
}
