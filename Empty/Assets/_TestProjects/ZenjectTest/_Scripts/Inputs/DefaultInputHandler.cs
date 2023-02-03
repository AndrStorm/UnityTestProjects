using UnityEngine;
using Zenject;

public class DefaultInputHandler : IPlayerInputHandler
{
    private readonly Player _player;
    private readonly Transform _playerTransform;
    private readonly CharacterController _playerController;
    
 
    [Inject]
    public DefaultInputHandler(Player player)
    {
        _player = player;
        _playerTransform = _player._PlayerUnit.transform;
        _playerController = _player._PlayerUnit.PlayerController;
    }
    

    public void OnMove(Vector2 moveDir)
    {
        Vector3 moveVecotor = _playerTransform.right * moveDir.x + _playerTransform.forward * moveDir.y;
        
        _playerController.Move(_player._PlayerUnit.MoveSpeed * Time.deltaTime * moveVecotor);
        //_playerTransform.Translate(moveDir,Space.World);
    }
    

    public void OnRotate(Vector2 rotDir)
    {
        _playerTransform.Rotate( _player._PlayerUnit.RotationSpeed * rotDir.x * Vector3.up);
    }


    public void OnJump()
    {
        _player.HealPlayer(_player._PlayerUnit.HealPower);
    }
}
