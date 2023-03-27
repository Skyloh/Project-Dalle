using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    bool active = false;

    bool locked = false;

    // using LateUpdate so that checks for if UI is triggered
    // happen after everything else. Otherwise, race conditions?
    void LateUpdate()
    {
        // we use Raw here because regular getAxis scales with time, which might be 0.
        // i.e. if we use it and we pause, the getAxis always returns 0.
        if (!locked && Input.GetAxisRaw("Cancel") > 0)
        {
            locked = true;

            TogglePauseMenu();
        }

        else if (Input.GetAxisRaw("Cancel") == 0)
        {
            locked = false;
        }
        
    }

    void TogglePauseMenu()
    {
        active = !active;

        canvas.SetActive(active);

        ToggleUIFocus(active);
    }

    public static void ToggleUIFocus(bool state)
    {
        if (state)
        {

            Debug.Log("set");
            Cursor.visible = true;

            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0f;
        }

        else
        {

            Debug.Log("unset");
            Cursor.visible = false;

            Cursor.lockState = CursorLockMode.Locked;

            Time.timeScale = 1f;
        }
    }
}
