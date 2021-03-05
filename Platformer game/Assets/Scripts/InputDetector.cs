using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputEntry
{
    public float horInput;
    public bool jumpInput;

    public InputEntry(float horInput , bool jumpInput) 
    {
        this.horInput = horInput;
        this.jumpInput = jumpInput;
    }
}

public class InputDetector : MonoBehaviour
{
    [SerializeField] private string horAxis;
    [SerializeField] private string jumpAxis;

    public InputEntry GetInput()
    {
        return new InputEntry
           (
            Input.GetAxis(horAxis),
            Input.GetButtonDown(jumpAxis)
           );
    }
}
