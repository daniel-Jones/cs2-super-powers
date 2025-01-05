using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class Twister(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 14;
    public override string Name => "Twister";
    
    public override string Description => $"Inflict disorienting spins upon your enemies with a 25% chance on each hit, disrupting their aim and movement.";
    
    public override char ChatColor => ChatColors.Silver;

    public override string HtmlColor => "#00FF00";
    
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
            RotateEnemy(victim!);
        }
        return HookResult.Continue;
    }

    private void RotateEnemy(CCSPlayerController player)
    {
        var currentPosition = player.PlayerPawn.Value!.AbsOrigin;
        var currentAngles = player.PlayerPawn.Value.EyeAngles;

        var newAngles = new QAngle(
            currentAngles.X,
            currentAngles.Y + 180,
            currentAngles.Z
        );

        player.PlayerPawn.Value.Teleport(currentPosition, newAngles, new Vector(0, 0, 0));
    }
}