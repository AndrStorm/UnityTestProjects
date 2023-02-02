using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private Player _player;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private bool _isFromNewInstance;
    public override void InstallBindings()
    {
        if (_isFromNewInstance)
        {
            var playerInstance = Container.InstantiatePrefabForComponent<Player>(
                _player, _spawnPosition.position, quaternion.identity, null);

            Container.Bind<Player>().FromInstance(playerInstance).AsSingle();
        }
        else
        {
            Container.Bind<Player>().FromInstance(_player).AsSingle(); 
        }
       
        
        
    }
}