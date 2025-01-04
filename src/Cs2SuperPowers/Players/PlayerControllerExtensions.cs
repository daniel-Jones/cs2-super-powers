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

        player.Health = Math.Max(maxHealth, player.Health);
        pawn.Health = Math.Max(maxHealth, pawn.Health);;
            
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
        return player;
    }
}