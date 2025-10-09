using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class OneShot(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 23;
    public override string Name => "One Shot";
    
    public override string Description => $"Instantly eliminate any opponent with a single hit, embodying the ultimate force of lethal precision.";
    
    public override char ChatColor => ChatColors.Magenta;

    public override string HtmlColor => "#ff5CD9";

    protected override void OnInitialize()
    {
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(OnTakeDamage, HookMode.Pre);
    }
    
    private HookResult OnTakeDamage(DynamicHook h)
    {
        var param = h.GetParam<CEntityInstance>(0);
        var param2 = h.GetParam<CTakeDamageInfo>(1);

        var attackerPawn = new CCSPlayerPawn(param2.Attacker.Value!.Handle);
        var victimPawn = new CCSPlayerPawn(param.Handle);

        if (attackerPawn.Controller.Value == null ||
            victimPawn.Controller.Value == null)
        {
            return HookResult.Continue;
        }

        var attacker = attackerPawn.Controller.Value.As<CCSPlayerController>();

        if (Ensure.Player(attacker).IsValid().IsAlive().Check() && IsAssignedTo(attacker))
        {
            param2.Damage = 1000f;
        }
        
        return HookResult.Continue;
    }
}