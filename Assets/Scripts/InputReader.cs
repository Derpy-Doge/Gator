using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    Gamepad gamepad;

    [SerializeField] private float deadzone = .2f;

    [Space(7f)]
    [Header("FPS Counter")]
    [SerializeField] private TMPro.TMP_Text fpsText;
    private float _pollingTime = 0.5f;
    private float _timeAccumulator;
    private int _frameCount;

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

        if(stickInput.magnitude > deadzone)
        {            
            StickDirection(stickInput);
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
    }

    private void StickDirection(Vector2 input)
    {
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        //each section in 45 degrees
        if (angle >= 22.5f && angle < 67.5f)        Debug.Log("up-right");
        else if (angle >= 67.5f && angle < 112.5f)  Debug.Log("up");
        else if (angle >= 112.5f && angle < 157.5f) Debug.Log("up-left");
        else if (angle >= 157.5f && angle < 202.5f) Debug.Log("left");
        else if (angle >= 202.5f && angle < 247.5f) Debug.Log("down-left");
        else if (angle >= 247.5f && angle < 292.5f) Debug.Log("down");
        else if (angle >= 292.5f && angle < 337.5f) Debug.Log("down-right");
        else                                        Debug.Log("right");
    }
}
