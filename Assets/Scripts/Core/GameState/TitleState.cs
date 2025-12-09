using SongLib.Patterns.State;
using UnityEngine;

public class TitleState : IState
{
    private ShootingStarSpawner shootingStarSpawner;
    
    public void OnEnter()
    {
        shootingStarSpawner = Object.FindAnyObjectByType<ShootingStarSpawner>();
        shootingStarSpawner.StartSpawning();
    }

    public void OnUpdate(float deltaTime)
    {
    }

    public void OnExit()
    {
        
    }
}
