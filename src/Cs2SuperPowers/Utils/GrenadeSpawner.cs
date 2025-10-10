using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;

namespace Cs2SuperPowers.Utils;

public static class GrenadeSpawner
{
    public static MemoryFunctionWithReturn<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, int, CHEGrenadeProjectile>
        CHEGrenadeProjectile_CreateFunc = new(
        "55 4C 89 C1 48 89 E5 41 57 49 89 FF 41 56 49 89 D6"
    );

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
                grenade.TeamNum = owner.TeamNum;

                if (owner.PlayerPawn?.Value != null)
                {
                    //owner.PrintToChat($"[cs2sp/debug] {owner.PlayerName} spawned a grenade at decoy position.");
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
