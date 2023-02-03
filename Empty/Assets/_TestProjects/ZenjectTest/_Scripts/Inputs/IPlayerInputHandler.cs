
using UnityEngine;

public interface IPlayerInputHandler
{
    void OnMove(Vector2 moveDir);
    void OnRotate(Vector2 rotDir);
    public void OnJump();

}