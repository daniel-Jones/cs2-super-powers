using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class PowerThief : BasePower
{
    public override int Id => 15;
    public override string Name => "Power Thief";
    
    public override string Description => $"With a 25% chance on hit, you can permanently strip your enemies of their current superpower.";
    
    public override char ChatColor => ChatColors.LightYellow;

    public override string HtmlColor => "#f8e1ab";
    
    private readonly Random _random = new();
    
    protected override void OnInitialize()
    {
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
    }

    private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
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
        
        if (_random.NextDouble() <= 0.25)
        {
            StealSuperpower(victim!, attacker!);
        }
        return HookResult.Continue;
    }

    private void StealSuperpower(CCSPlayerController victim, CCSPlayerController attacker)
    {
        if (!PlayerPowers.Instance.TryGetValue(victim, out var power)) return;
        
        PlayerPowers.Instance.RemovePower(victim);
        victim.SetMaxHealth(100);
        attacker.PrintToChat($"You have stolen {ChatColors.Red}{victim.PlayerName} {ChatColors.Default}superpower {power!.ChatColor}{power.Name}.");
        victim.PrintToChat($"Your superpower {power.ChatColor}{power.Name} {ChatColors.Red}was stolen! You're a regular guy/gal now.");
    }
}