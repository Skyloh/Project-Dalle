using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController; // reference to the CC component
    float yVelo; // accumulator for the player's yVelo. without this, they wouldn't have gravity.

    [SerializeField] float MOVE_SPEED = 15f; // how fast the player moves
    [SerializeField] float GRAVITY = 9.81f; // how fast the player falls

    // Start:
    // assign reference to characterController of this player's CC
    // set yVelo to an initial 0
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        yVelo = 0f;
    }

    // Update:
    // not my favorite method of input by a long shot, but it works well enough for 
    // the specified scope of this small project.
    // Update Cycle:
    // resets the yVelo to 0 is the character is grounded, otherwise incrementing it by
    // the acceleration due to Gravity.
    // after this, gets the player's input and moves the player in the correct direction,
    // scaled by time and speed
    void Update()
    {
        
        if (characterController.isGrounded)
        {
            yVelo = 0f;
        }

        yVelo -= GRAVITY * Time.deltaTime;
        

        // GetAxis(...) smoothens the value from 0 to 1, resulting in a gradual increase
        // up to the desired value, followed by a decrease. I.e. it's not an instant change.
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // make sure we move in the direction we are looking
        // normalize input so that moving diagonally (1, 1 as input) doesn't result in moving
        // faster than if you just moved laterally.
        Vector3 vec = transform.TransformDirection(new Vector3(horizontal, 0f, vertical).normalized);

        vec.y = yVelo;

        characterController.Move(MOVE_SPEED * Time.deltaTime * vec);
        // dont repalce with simplemove because the gravity there is too slow
    }
}
