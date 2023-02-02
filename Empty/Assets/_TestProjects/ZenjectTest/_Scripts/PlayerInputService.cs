using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerInputService : MonoBehaviour, IPlayerInputService
{
    private IPlayerInputHandler _playerInput;

    [Inject] private Player _player;
    
    private void Awake()
    {
        _playerInput = _player.PlayerInput;
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        

        _playerInput.OnMove( new Vector2(x,y));
        
        
        if (Keyboard.current.qKey.isPressed)
        {
            _playerInput.OnRotate(new Vector2(-1,0));
        }
        if (Keyboard.current.eKey.isPressed)
        {
            _playerInput.OnRotate(new Vector2(1,0));
        }
        
    }
}