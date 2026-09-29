using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif

public class SnapbackProcessor : InputProcessor<Vector2>
{
    private Vector2 lastValue;

#if UNITY_EDITOR
    static SnapbackProcessor() { Initialize(); }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<SnapbackProcessor>("SnapbackProcessor");
    }

    public override Vector2 Process(Vector2 value, InputControl control)
    {
        if(value.magnitude > .1f && lastValue.magnitude > .4f)
        {
            if (Vector2.Dot(value.normalized, lastValue.normalized) < -.7f)
            {
                //this is overshoot
                return Vector2.zero;
            }
        }

        lastValue = value;
        return value;  
    }
}
