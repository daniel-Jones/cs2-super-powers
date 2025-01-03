using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class ArmBreaker : BasePower
{
    public override int Id => 6;
    public override string Name => "Arms Breaker";
    
    public override string Description => $"You have 25% chance to disarm victim on hit.";
    
    public override char ChatColor => ChatColors.LightBlue;
    
    private readonly Random _random = new Random();

    public override string HtmlColor => "#D6E6FF";
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
        
        if (_random.NextDouble() <= 0.25)
        {
            var weaponServices = victim!.PlayerPawn?.Value?.WeaponServices;
            if (weaponServices?.MyWeapons == null) return HookResult.Continue;

            foreach (var weapon in weaponServices.MyWeapons)
            {
                var weaponName = weapon.Value?.DesignerName;
                if (weaponName != null && !weaponName.Contains("weapon_knife") && !weaponName.Contains("weapon_c4"))
                {
                    victim.DropActiveWeapon();
                }
            }
        }
        return HookResult.Continue;
    }
}