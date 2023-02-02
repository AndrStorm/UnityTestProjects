using UnityEngine;
using Zenject;

public class PlayerInputServiceInstaller : MonoInstaller
{
    [SerializeField] private PlayerInputService _playerInputService;
    
    public override void InstallBindings()
    {
        
        /*Container.Bind<IPlayerInputService>().To<PlayerInputService>().
            FromComponentInNewPrefab(_playerInputService).AsSingle();*/
    }
}