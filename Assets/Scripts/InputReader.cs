using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    private Vector2 moveVel;
    [Space (3f)]
    [Header("<size=12>Jumping</size>")]
    public float jumpForce;
    private bool _canJump;
    [Space(7f)]
    [Header("Ground Check Settings")]
    [Space(3f)]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundChecksize;
    public LayerMask ground;
    private bool _isGrounded;

    [Space(7f)]
    [Header("Input")]
    Gamepad gamepad;
    Rigidbody2D rb;
    InputSystem_Actions controls;
    [SerializeField] private float inputDeadzone;
    Vector3 direction;
    Vector2 currentInput;
    Vector2 lastInput;

    int testX;
    int testY;

    [Space(7f)]
    [Header("FPS")]
    [SerializeField] private TMPro.TMP_Text fpsText;
    private float _pollingTime = 0.5f;
    private float _timeAccumulator;
    private int _frameCount;

    [SerializeField] private int targetFPS = 60;

    void Awake()
    {
        controls = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        QualitySettings.vSyncCount = 0; //no vsync
        Application.targetFrameRate = targetFPS; //gonna try to keep ts at 60 for simplicities sake
    }

    void OnEnable()
    {
        controls.Player.Move.performed += OnTap;
        controls.Player.Jump.performed += Jump;
        controls.Enable();
    }

    void OnDisable( )
    {
        controls.Player.Move.performed -= OnTap;
        controls.Disable();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        gamepad = Gamepad.current;
        if(gamepad == null)
        {
            Debug.LogWarning("yo plug ts in dawg");
            return;
        }

        if (gamepad.buttonNorth.wasPressedThisFrame) Debug.Log("north button");
        if (gamepad.buttonEast.wasPressedThisFrame) Debug.Log("east button");
        if (gamepad.buttonSouth.wasPressedThisFrame) Debug.Log("south button");
        if (gamepad.buttonWest.wasPressedThisFrame) Debug.Log("west button");

        Vector2 stickInput = gamepad.leftStick.ReadValue();

        _isGrounded = Physics2D.OverlapBox(groundCheckPoint.position, groundChecksize / 2, 0, ground);
        if (_isGrounded)
        {
            _canJump = true;
        }
        moveVel.x = stickInput.x;
        if(moveVel.magnitude > inputDeadzone)
        {
            rb.linearVelocityX = moveVel.x * walkSpeed;
        }
        else
        {
            rb.linearVelocityX = 0;
        }
 
        #region FPSCounter
        _timeAccumulator += Time.deltaTime;
        _frameCount++;

        if(_timeAccumulator >= _pollingTime)
        {
            if (fpsText == null) 
            {
                return;
            } 

            int fps = Mathf.RoundToInt(_frameCount / _timeAccumulator);
            fpsText.SetText(fps + " FPS");

            _timeAccumulator = 0.0f;
            _frameCount = 0;
        }
        #endregion

        #region dihrection
        Debug.DrawRay(transform.position, direction * 3f, Color.yellow);
        LastDirection();

        currentInput = new Vector2(stickInput.x, 0);
        direction = new Vector2(lastInput.x, 0);

        if (direction.magnitude > (inputDeadzone - 0.05f))
        {
            direction.Normalize();
        }
        #endregion
    }

    void Jump(InputAction.CallbackContext ctx)
    {
        if (_canJump)
        {
            _canJump = false;
            rb.linearVelocityY = jumpForce;
        }
        else
        {
            return;
        }

    }

    void OnTap(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is UnityEngine.InputSystem.Interactions.TapInteraction)
        {
            Vector2 rawInput = ctx.ReadValue<Vector2>();
            if(rawInput.magnitude < inputDeadzone)
            {
                rawInput = Vector2.zero;
            }
            Vector2 tapDirect =rawInput;
            StickDirection(tapDirect);
            //DirectionTest(tapDirect);
        }
    }

    private void StickDirection(Vector2 input)
    {
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        //each section in 45 degrees
        if (angle == 0f) Debug.Log("neutral");
        else if (angle >= 22.5f && angle < 67.5f) Debug.Log("up-right");
        else if (angle >= 67.5f && angle < 112.5f) Debug.Log("up");
        else if (angle >= 112.5f && angle < 157.5f) Debug.Log("up-left");
        else if (angle >= 157.5f && angle < 202.5f) Debug.Log("left");
        else if (angle >= 202.5f && angle < 247.5f) Debug.Log("down-left");
        else if (angle >= 247.5f && angle < 292.5f) Debug.Log("down");
        else if (angle >= 292.5f && angle < 337.5f) Debug.Log("down-right");
        else Debug.Log("right");
    }

    private void DirectionTest(Vector2 input)
    {
        testX = Mathf.RoundToInt(input.x);
        testY = Mathf.RoundToInt(input.y);

        if(testX == testY) return;
        if(testX > 0 && testY == 0) Debug.Log("right");
        if (testX < 0 && testY == 0) Debug.Log("left");
        if (testX == 0 && testY > 0) Debug.Log("up");
        if (testX == 0 && testY < 0) Debug.Log("down");
        if (testX > 0 && testY > 0) Debug.Log("up-right");
        if (testX < 0 && testY > 0) Debug.Log("up-left");
        if (testX > 0 && testY < 0) Debug.Log("down-right");
        if (testX < 0 && testY < 0) Debug.Log("down-left");
    }

    public void LastDirection()
    {
        if (currentInput != Vector2.zero)
        {
            lastInput = currentInput;
        }
    }

    private void OnDrawGizmos()
    {
        Matrix4x4 originalMatrix = Gizmos.matrix;

        Gizmos.matrix = groundCheckPoint.localToWorldMatrix;

        Gizmos.color = Color.green; //groundcheck
        Gizmos.DrawCube(Vector3.zero, groundChecksize);

        Gizmos.matrix = originalMatrix;

        Gizmos.color = Color.red; //groundcheck ray
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
