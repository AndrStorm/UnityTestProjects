using UnityEngine;


public class Player : MonoBehaviour
{

    [SerializeField] private CharacterController _characterController;
    public CharacterController PlayerController => _characterController;
    
    [SerializeField] private float _movementSpeed;
    public float MoveSpeed => _movementSpeed;
    
    [SerializeField] private float _rotationSpeed;
    public float RotationSpeed => _rotationSpeed;


    private IPlayerInputHandler _playerInput;

    public IPlayerInputHandler PlayerInput
    {
        get
        {
            return _playerInput ??= new DefaultInputHandler(this);
        }
    }


}
