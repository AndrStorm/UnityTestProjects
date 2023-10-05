using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator AniConRef;

    private int _walkingStateId;

    private void Start()
    {
        _walkingStateId = Animator.StringToHash("Walking");
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            AniConRef.PlayInFixedTime(_walkingStateId);
        }
        // else if (Keyboard.current.fKey.wasReleasedThisFrame)
        // {
        //     AniConRef.PlayInFixedTime("Idle Walk Run Blend");
        // }
    }
}
