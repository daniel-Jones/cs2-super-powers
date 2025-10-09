using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;

namespace Cs2SuperPowers.Utils;

/// <summary>
/// Shared utility class for spawning grenades across different powers.
/// Contains the common memory function and basic grenade creation logic.
/// </summary>
public static class GrenadeSpawner
{
    /// <summary>
    /// Memory function for creating CHE grenade projectiles.
    /// This is the core function used by all grenade-spawning powers.
    /// </summary>
    public static MemoryFunctionWithReturn<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, int, CHEGrenadeProjectile>
        CHEGrenadeProjectile_CreateFunc = new(
        "55 4C 89 C1 48 89 E5 41 57 49 89 FF 41 56 49 89 D6"
    );

    /// <summary>
    /// Creates a grenade at the specified position with the given angle and velocity.
    /// This is the basic grenade creation function used by all powers.
    /// </summary>
    /// <param name="position">The spawn position of the grenade</param>
    /// <param name="angle">The spawn angle of the grenade</param>
    /// <param name="velocity">The initial velocity of the grenade</param>
    /// <returns>The created grenade projectile, or null if creation failed</returns>
    public static CBaseCSGrenadeProjectile? CreateGrenade(CounterStrikeSharp.API.Modules.Utils.Vector position, CounterStrikeSharp.API.Modules.Utils.QAngle angle, CounterStrikeSharp.API.Modules.Utils.Vector velocity)
    {
        try
        {
            return CHEGrenadeProjectile_CreateFunc.Invoke(
                position.Handle,
                angle.Handle,
                velocity.Handle,
                velocity.Handle,
                IntPtr.Zero,
                44
            );
        }
        catch
        {
            return null;
        }
    }

    public static CBaseCSGrenadeProjectile? CreateGrenadeWithOwner(CounterStrikeSharp.API.Modules.Utils.Vector position, CounterStrikeSharp.API.Modules.Utils.QAngle angle, CounterStrikeSharp.API.Modules.Utils.Vector velocity, CCSPlayerController owner)
    {
        try
        {
            var grenade = CHEGrenadeProjectile_CreateFunc.Invoke(
                position.Handle,
                angle.Handle,
                velocity.Handle,
                velocity.Handle,
                IntPtr.Zero,
                44
            );

            if (grenade != null && grenade.IsValid)
            {
                // Set the team number to match the owner for proper kill attribution
                grenade.TeamNum = owner.TeamNum;
                
                // Set the owner/instigator for kill attribution
                // This ensures that kills from this grenade are attributed to the owner
                if (owner.PlayerPawn?.Value != null)
                {
                    grenade.AcceptInput("InitializeSpawnFromWorld", owner.PlayerPawn.Value, owner.PlayerPawn.Value, "");
                }
            }

            return grenade;
        }
        catch
        {
            return null;
        }
    }
}
