using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationManager : MonoBehaviour
{
    public Animator AniConRef;

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            AniConRef.PlayInFixedTime("Walking");
        }
        else if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            AniConRef.PlayInFixedTime("Idle Walk Run Blend");
        }
    }
}
