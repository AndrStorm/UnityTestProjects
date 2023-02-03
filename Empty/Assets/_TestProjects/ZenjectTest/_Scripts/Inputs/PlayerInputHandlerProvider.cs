using Zenject;

public class PlayerInputHandlerProvider
{
    public IPlayerInputHandler PlayerInput { get; private set; }


    [Inject]
    public PlayerInputHandlerProvider(IPlayerInputHandler inputHandler)
    {
        PlayerInput = inputHandler;
    }
    
    
}
