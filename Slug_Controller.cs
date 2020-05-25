using UnityEngine;


[RequireComponent( typeof( CharacterController ) )]
public class Slug_Controller : MonoBehaviour
{
    /*Private variables*/
    CharacterController _playerController;
    Vector3 _velocity;
    Vector3 input;
    Vector3 move = Vector3.zero;
    float speed;
    bool isGrounded;

    /*Public variables*/
    public bool respawning = false;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float walkSpeed = 8f;
    public float runSpeed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float airSpeed = 6f;

    private void Awake() {
        _playerController = GetComponent<CharacterController>();
    }

    void Update()
    {
        GroundedCheck();
        if ( !respawning ) {
            HandleInput();
            HandleJump();
        } else {
            input.x = 0;
            input.z = 0;
        }
    }

    private void FixedUpdate() {
        ProcessMovement();
        ApplyGravity();
        _playerController.Move( move * Time.deltaTime );
    }

    void HandleInput() {
        input = new Vector3( Input.GetAxisRaw( "Horizontal" ), 0f, Input.GetAxisRaw( "Vertical" ) );
        input = transform.TransformDirection( input );
        input = Vector3.ClampMagnitude( input, 1f );
    }

    void ProcessMovement() {
        if ( isGrounded ) {
            speed = Input.GetKey( KeyCode.LeftShift ) ? runSpeed : walkSpeed;
            if ( input.x != 0 ) {
                move.x += input.x * speed;
            } else {
                move.x = 0;
            }
            if ( input.z != 0 ) {
                move.z += input.z * speed;
            } else {
                move.z = 0;
            }

        } else {
            move.x += input.x * airSpeed;
            move.z += input.z * airSpeed;
        }
        move = Vector3.ClampMagnitude( move, speed );
    }

    public void SetRespawn( bool value ) {
        respawning = value;
    }

    void GroundedCheck() {
        isGrounded = Physics.CheckSphere( groundCheck.position, groundDistance, groundMask );
        if ( isGrounded && _velocity.y < 0f ) { _velocity.y = -2f; }
    }

    void HandleJump() {
        if ( Input.GetButtonDown( "Jump" ) && isGrounded ) { _velocity.y = Mathf.Sqrt( jumpHeight * -2f * gravity ); }
    }

    void ApplyGravity() {
        _velocity.y += gravity * Time.deltaTime;
        _playerController.Move( _velocity * Time.deltaTime );
    }
}
