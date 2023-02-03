using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerInputService : IPlayerInputService, IInitializable, ITickable
{
    private IPlayerInputHandler _playerInput;
    
    private PlayerInputHandlerProvider _playerInputHandlerProvider;


    [Inject]
    private void Constructor(PlayerInputHandlerProvider inputHandlerProvider)
    {
        _playerInputHandlerProvider = inputHandlerProvider;
    }
    

    public void Initialize()
    {
        _playerInput = _playerInputHandlerProvider.PlayerInput;
    }

    public void Tick()
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

        if (Keyboard.current.spaceKey.isPressed)
        {
            _playerInput.OnJump();
        }
    }
}