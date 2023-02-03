using Zenject;

public class PlayerInputInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<PlayerInputService>().AsSingle();
        Container.Bind<PlayerInputHandlerProvider>().AsSingle();
        Container.Bind<IPlayerInputHandler>().To<DefaultInputHandler>().AsSingle();
    }
}