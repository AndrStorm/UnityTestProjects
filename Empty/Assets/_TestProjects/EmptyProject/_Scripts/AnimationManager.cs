using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator AniConRef;

    private int _walkingStateId;
    private int _standWalk;

    private void Start()
    {
        _walkingStateId = Animator.StringToHash("Walking");
        _standWalk = Animator.StringToHash("StandWalk");
        
        Debug.Log("press f to stand walk");
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("was pressed");
            AniConRef.SetBool(_standWalk, true);
            AniConRef.PlayInFixedTime(_walkingStateId);
        }
        else if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            AniConRef.SetBool(_standWalk, false);
            // AniConRef.PlayInFixedTime("Idle Walk Run Blend");
        }
        /*else if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            AniConRef.PlayInFixedTime("Idle Walk Run Blend");
        }*/
    }
}
