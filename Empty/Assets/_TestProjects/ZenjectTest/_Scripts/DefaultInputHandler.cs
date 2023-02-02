using UnityEngine;

public class DefaultInputHandler : IPlayerInputHandler
{
    private readonly Transform _playerTransform;
    private readonly CharacterController _controller;
    private readonly Player _player;

    public DefaultInputHandler(Player player)
    {
        _player = player;
        _playerTransform = _player.transform;
        _controller = _player.PlayerController;
    }
    

    public void OnMove(Vector2 moveDir)
    {
        Vector3 moveVecotor = _playerTransform.right * moveDir.x + _playerTransform.forward * moveDir.y;
        
        _controller.Move(_player.MoveSpeed * Time.deltaTime * moveVecotor);
        //_playerTransform.Translate(moveDir,Space.World);
    }
    

    public void OnRotate(Vector2 rotDir)
    {
        _playerTransform.Rotate( _player.RotationSpeed * rotDir.x * Vector3.up);
    }
}
