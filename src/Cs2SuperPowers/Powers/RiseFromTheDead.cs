using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class RiseFromTheDead(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 1;
    public override string Name => "Rise From the Dead";
    
    public override string Description => $"You have {(int)(ReviveChance * 100)}% chance to rise from the dead.";
    
    public override char ChatColor => ChatColors.Red;

    public override string HtmlColor => "#ff5C0A";

    private const double ReviveChance = 0.35;    
    
    private readonly Random _random = new();
    
    protected override void OnInitialize()
    {
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }
    
    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo _)
    {
        var player = @event.Userid;
        if (!Ensure.Player(player).IsValid().Check() || !IsAssignedTo(player!))
        {
            return HookResult.Continue;
        }

        if (_random.NextDouble() <= ReviveChance)
        {
            Server.NextFrame(() =>
            {
                player!.MaxHealth = 70;
                player.Health = 70;
                player.Respawn();
                player.PrintToChat($"You have been raised from {ChatColors.DarkRed}the dead.");
            });
        }

        return HookResult.Continue;
    }
}
