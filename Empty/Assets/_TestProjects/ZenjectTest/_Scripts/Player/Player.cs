using UnityEngine;
using Zenject;


public class Player : IInitializable
{
    public readonly PlayerUnit _PlayerUnit;
    
    private SignalBus _signalBus;

    [Inject]
    public Player(PlayerUnit playerUnit, SignalBus signalBus)
    {
        _PlayerUnit = playerUnit;
        _signalBus = signalBus;
    }
    
    public void Initialize()
    {
        Debug.Log("Press Space to Heal");
    }
    
    
    public void HealPlayer(int healAmount)
    {
        _signalBus?.Fire(new PlayerHealedSignal(){amount = healAmount});
        Debug.Log("Event Player health Incrised by " + healAmount);
    }


    
}
