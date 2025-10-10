using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;

namespace Cs2SuperPowers.Utils;

public static class GrenadeSpawner
{
    static MemoryFunctionWithReturn<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, int, CHEGrenadeProjectile>
        CHEGrenadeProjectile_CreateFunc = new(
        "55 4C 89 C1 48 89 E5 41 57 49 89 FF 41 56 49 89 D6"
    );

    public static CBaseCSGrenadeProjectile? CreateGrenade(CounterStrikeSharp.API.Modules.Utils.Vector position, CounterStrikeSharp.API.Modules.Utils.QAngle angle, CounterStrikeSharp.API.Modules.Utils.Vector velocity, CCSPlayerPawn? owner = null)
    {
        try
        {
            return CHEGrenadeProjectile_CreateFunc.Invoke(
                position.Handle,
                angle.Handle,
                velocity.Handle,
                velocity.Handle,
                owner == null ? IntPtr.Zero : owner.Handle,
                44
            );
        }
        catch
        {
            return null;
        }
    }
}
