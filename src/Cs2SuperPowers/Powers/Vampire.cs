using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class Vampire(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 12;
    public override string Name => "Vampire";
    
    public override string Description => $"Drain the life force of your enemies, healing yourself for {(int)(HealModifier * 100)}% of the damage dealt with each shot.";
    
    public override char ChatColor => ChatColors.Magenta;

    public override string HtmlColor => "#ff5CD9";
    
    private const double HealModifier = 0.7;    
    
    protected override void OnInitialize()
    {
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
    }

    private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        var attacker = @event.Attacker;
        var victim = @event.Userid;

        if (
            !Ensure.Player(victim).IsValid().IsAlive().Check() || 
            !Ensure.Player(attacker).IsValid().Check() ||
            attacker == victim ||
            !IsAssignedTo(attacker!))
        {
            return HookResult.Continue;
        }
        
        var attackerPawn = attacker!.PlayerPawn.Value;
        if (attackerPawn == null)
        {
            return HookResult.Continue;
        }

        var healing = (int)(@event.DmgHealth * HealModifier);
        var newHealth = Math.Min(attackerPawn.Health + healing, attackerPawn.MaxHealth);
        attackerPawn.Health = newHealth;
        Utilities.SetStateChanged(attackerPawn, "CBaseEntity", "m_iHealth");
        
        return HookResult.Continue;
    }
}