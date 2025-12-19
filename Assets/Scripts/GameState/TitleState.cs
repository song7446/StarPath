using SongLib.Patterns.State;
using UnityEngine;

public class TitleState : IState
{
    private ShootingStarSpawner shootingStarSpawner;
    private TwinkleStarSpawner twinkleStarSpawner;
    
    public void OnEnter()
    {
        shootingStarSpawner = Object.FindAnyObjectByType<ShootingStarSpawner>();
        twinkleStarSpawner = Object.FindAnyObjectByType<TwinkleStarSpawner>();
        
        shootingStarSpawner.StartSpawning();
        twinkleStarSpawner.StartSpawning();
    }

    public void OnUpdate(float deltaTime)
    {
    }

    public void OnExit()
    {
        
    }
}
