using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputReader : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    private Vector2 moveVel;
    [Space (1f)]
    [Header("<size=11>Jumping</size>")]
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
    [Header("Healthbar")]
    public Image healthBar;

    [Space(7f)]
    [Header("Input")]
    [SerializeField] private float inputDeadzone;
    Vector2 stickInput;
    Gamepad gamepad;

    Vector2 keyboardInput;

    Rigidbody2D rb;
    InputSystem_Actions controls;

    Vector3 direction;
    Vector2 currentInput;
    Vector2 lastInput;

    public int tester = 0; // just so i can see dpad value in inspector
    public static int dpad = 5;
    public static int hori = 0;
    public static int vert = 0;

    const int bufferlength = 60;
    public List<int> dirBuffer = new List<int>(); // MAKE THIS STATIC LATER


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
        controls.Player.Move.performed += ctx =>
        {
            if (gamepad != null)
            {               
                OnTap(ctx);
            }
            else
            {
                keyboardInput = ctx.ReadValue<Vector2>();
                KeyboardDirection();
                dirBuffer.Insert(0, dpad);

            }     
        };
        controls.Player.Move.canceled += ctx =>
        {
            if (gamepad != null)
            {

            }
            else
            {              
                keyboardInput = Vector2.zero;
                KeyboardDirection();
            }
        };

        controls.Player.Jump.performed += Jump;
        controls.Enable();
    }

    void OnDisable( )
    {
        controls.Player.Move.performed -= ctx =>
        {
            KeyboardDirection();

            OnTap(ctx);
        };
        controls.Disable();
    }

    void Start()
    {
        gamepad = Gamepad.current;
        Debug.Log(gamepad);

        dpad = 5;

        if (gamepad == null)
        {
            Debug.LogWarning("yo plug ts in dawg");
        }

        dirBuffer = new List<int>();
    }

    
    void Update()
    {
        tester = dpad;
        gamepad = Gamepad.current;          

        if(gamepad != null)
        {
            stickInput = gamepad.leftStick.ReadValue();
            moveVel.x = stickInput.x;

            if (gamepad.buttonNorth.wasPressedThisFrame) Debug.Log("north button");
            if (gamepad.buttonEast.wasPressedThisFrame) Debug.Log("east button");
            if (gamepad.buttonSouth.wasPressedThisFrame) Debug.Log("south button");
            if (gamepad.buttonWest.wasPressedThisFrame) Debug.Log("west button");
        }
        else
        {
            moveVel.x = keyboardInput.x;
        }

        if(moveVel.magnitude > inputDeadzone)
        {
            rb.linearVelocityX = moveVel.x * walkSpeed;
        }
        else
        {
            rb.linearVelocityX = 0;
        }

        _isGrounded = Physics2D.OverlapBox(groundCheckPoint.position, groundChecksize / 2, 0, ground);
        if (_isGrounded)
        {
            _canJump = true;
        }

        #region healthbar
        Stats stats = GetComponent<Stats>();
        if(stats != null)
        {
            healthBar.fillAmount = stats.healthCurrent / stats.healthMax;
        }
        #endregion

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
        if (gamepad != null)
        {
            currentInput = new Vector2(stickInput.x, 0);
        }
        else
        {
            currentInput = new Vector2(keyboardInput.x, 0);
        }       
        direction = new Vector2(lastInput.x, 0);

        if (direction.magnitude > (inputDeadzone - 0.05f))
        {
            direction.Normalize();
        }
        #endregion

        while (dirBuffer.Count > bufferlength)
        {
            dirBuffer.RemoveAt(dirBuffer.Count - 1);
        }
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
        }
    }

    private void StickDirection(Vector2 input)
    {
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        //each section in 45 degrees
        if (angle == 0f)
        {
            Debug.Log("neutral");
            dpad = 5;
        }
        else if (angle >= 22.5f && angle < 67.5f)
        {
            Debug.Log("up-right");
            dpad = 9;
        }
        else if (angle >= 67.5f && angle < 112.5f)
        {
            Debug.Log("up");
            dpad = 8;
        }
        else if (angle >= 112.5f && angle < 157.5f)
        {
            Debug.Log("up-left");
            dpad = 7;
        }
        else if (angle >= 157.5f && angle < 202.5f)
        {
            Debug.Log("left");
            dpad = 4;
        }
        else if (angle >= 202.5f && angle < 247.5f)
        {
            Debug.Log("down-left");
            dpad = 1;
        }
        else if (angle >= 247.5f && angle < 292.5f)
        {
            Debug.Log("down");
            dpad = 2;
        }
        else if (angle >= 292.5f && angle < 337.5f)
        {
            Debug.Log("down-right");
            dpad = 3;
        }
        else 
        {
            Debug.Log("right");
            dpad = 6;
        } 
    }

    private void KeyboardDirection()
    {
        
        
        if(keyboardInput.x == 0 && keyboardInput.y == 0)
        {
            dpad = 5;
            hori = 0;
            vert = 0;
        }
        else
        {
            if (keyboardInput.x == 0)
            {
                hori = 0;
            }
            else if (keyboardInput.x > 0) //right
            {
                hori = 1;
            }
            else if (keyboardInput.x < 0) //left
            {
                hori = -1;
            }

            if(keyboardInput.y == 0)
            {
                vert = 0;
            }
            else if (keyboardInput.y > 0) //up
            {
                vert = 1;
            }
            else if (keyboardInput.y < 0) //down
            {
                vert = -1;
            }
        }

        dpad = hori + 2 + ((vert + 1) * 3);
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
