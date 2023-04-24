using UnityEngine;

public class PlayerCameraBehavior : MonoBehaviour
{
    float xRot = 0f;
    Transform playerBody;

    [SerializeField] PlayerDataSO data;

    void Start()
    {
        playerBody = transform.parent.transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // hacky solution
        if (Time.timeScale == 0f)
        {
            return;
        }

        float moveX = Input.GetAxis("Mouse X") * data.SENSITIVITY;// * Time.deltaTime;
        float moveY = Input.GetAxis("Mouse Y") * data.SENSITIVITY;// * Time.deltaTime;

        playerBody.Rotate(Vector3.up, moveX);

        xRot -= moveY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);
    }
}
