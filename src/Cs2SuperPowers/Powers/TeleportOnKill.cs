using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class TeleportOnKill(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 21;
    public override string Name => "Teleport On Kill";
    
    public override string Description => $"When you kill an enemy, you teleport to their location";
    
    public override char ChatColor => ChatColors.Red;

    public override string HtmlColor => "#FF0000";

    protected override void OnInitialize()
    {
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }

    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var attacker = @event.Attacker;
        var victim = @event.Userid;

        if (
            !Ensure.Player(victim).IsValid().Check() || 
            !Ensure.Player(attacker).IsValid().Check() ||
            attacker == victim ||
            !IsAssignedTo(attacker!))
        {
            return HookResult.Continue;
        }
        
        if (victim!.PlayerPawn?.Value != null && attacker!.PlayerPawn?.Value != null)
        {
            var victimPos = victim.PlayerPawn.Value.AbsOrigin;
            // raise Z a bit to avoid getting stuck in the ground
            if (victimPos != null)
            {
                victimPos.Z += 10;
                attacker.PlayerPawn.Value.Teleport(victimPos, null, null);
            }
        }

        return HookResult.Continue;
    }
}