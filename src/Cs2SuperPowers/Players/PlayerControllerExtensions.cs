using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Cs2SuperPowers.Players;

public static class PlayerControllerExtensions
{
    public static CCSPlayerController SetNewHealth(this CCSPlayerController player, int health)
    {
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return player;

        player.MaxHealth = health;
        pawn.MaxHealth = health;

        player.Health = health;
        pawn.Health = health;
            
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
        return player;
    }
    
    public static CCSPlayerController SetMaxHealth(this CCSPlayerController player, int maxHealth)
    {
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return player;

        player.MaxHealth = maxHealth;
        pawn.MaxHealth = maxHealth;

        player.Health = Math.Min(maxHealth, player.Health);
        pawn.Health = Math.Min(maxHealth, pawn.Health);;
            
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
        return player;
    }

    public static float GetSpeed(this CCSPlayerController player)
    {
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return 1.0f;
        return pawn.VelocityModifier;
    }

    public static CCSPlayerController SetNewSpeed(this CCSPlayerController player, float speed)
    {
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return player;

        pawn.VelocityModifier = speed;
        Utilities.SetStateChanged(pawn, "CCSPlayer_MovementServices", "m_flVelocityModifier");
        return player;
    }
}