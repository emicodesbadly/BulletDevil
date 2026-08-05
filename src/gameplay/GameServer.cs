using System;
using BulletDevil.Rendering;

namespace BulletDevil.Gameplay;

public sealed class GameServer
{
    // Lazy singleton implementation (NOT THREAD-SAFE!!!)
	private static readonly Lazy<GameServer> instance = new(() => new GameServer());
	public static GameServer Instance => instance.Value;

    private GameServer()
    {

    }

    public void UpdateBullets()
    {
        foreach (Bullet bullet in RenderingServer.Instance.bullets.Values)
        {
            bullet.Update();
        }
    }
}