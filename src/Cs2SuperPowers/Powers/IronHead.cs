using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class IronHead : BasePower
{
    public override int Id => 4;
    public override string Name => "Iron Head";
    
    public override string Description => $"You cannot be killed by head shots.";
    
    public override char ChatColor => ChatColors.LightPurple;

    public override string HtmlColor => "#8B4513";

    protected override void OnInitialize()
    {
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
    }

    private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        var attacker = @event.Attacker;
        var victim = @event.Userid;
        if (@event.Hitgroup != (int)HitGroup_t.HITGROUP_HEAD)
        {
            return HookResult.Continue;
        }

        if (
            !Ensure.Player(victim).IsValid().Check() || 
            !Ensure.Player(attacker).IsValid().Check() ||
            attacker == victim ||
            !IsAssignedTo(victim!))
        {
            return HookResult.Continue;
        }
        
        var playerPawn = victim!.PlayerPawn.Value;
        var newHealth = playerPawn!.Health + @event.DmgHealth;

        if (newHealth > 100)
            newHealth = 100;

        playerPawn.Health = newHealth;
        return HookResult.Stop;
    }
}