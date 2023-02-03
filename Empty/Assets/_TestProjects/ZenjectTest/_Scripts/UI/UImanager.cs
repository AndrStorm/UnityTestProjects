using UnityEngine;
using Zenject;

public class UImanager : MonoBehaviour
{
    [Inject] private readonly SignalBus _signalBus;

    
    private void Start()
    {
        _signalBus.Subscribe<PlayerHealedSignal>(OnPlayerHealed);
    }


    private void OnPlayerHealed(PlayerHealedSignal args)
    {
        ShowHealth(args.amount);
    }

    private void ShowHealth(int health)
    {
        Debug.Log("health is changed by " + health);
    }
}
