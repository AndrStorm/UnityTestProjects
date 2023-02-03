using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private PlayerUnit _playerUnit;
    [SerializeField] private PlayerUnit _playerUnitPrefab;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private bool _isFromNewInstance;
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<PlayerHealedSignal>();
        
        if (_isFromNewInstance)
        {
            var playerInstance = Container.InstantiatePrefabForComponent<PlayerUnit>(
                _playerUnitPrefab, _spawnPosition.position, quaternion.identity, null);

            Container.Bind<PlayerUnit>().FromInstance(playerInstance).AsSingle();
            //Container.QueueForInject(playerInstance);
        }
        else
        {
            Container.Bind<PlayerUnit>().FromInstance(_playerUnit).AsSingle();
        }
        
        Container.BindInterfacesAndSelfTo<Player>().AsSingle();

    }
}