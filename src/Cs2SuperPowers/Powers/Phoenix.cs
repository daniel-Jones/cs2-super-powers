using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class Phoenix(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 2;
    public override string Name => "Phoenix Rising";
    
    public override string Description => $"You will be reborn after your first death like Phoenix from the ashes.";
    
    public override char ChatColor => ChatColors.DarkRed;

    public override string HtmlColor => "#ff5C0A";

    private HashSet<CCSPlayerController> _phoenixUses = new HashSet<CCSPlayerController>();

    protected override void OnInitialize()
    {
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }

    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo _)
    {
        var player = @event.Userid;
        if (!Ensure.Player(player).IsValid().Check() || !IsAssignedTo(player!))
        {
            return HookResult.Continue;
        }

        if (_phoenixUses.Contains(player!))
        {
            return HookResult.Continue;
        }

        Server.NextFrame(() =>
        {            
            var newHealth = 120;
            player!.SetNewHealth(newHealth);
            player!.Respawn();
            _phoenixUses.Add(player);

            player.PrintToChat($"{ChatColors.DarkRed}Phoenix {ChatColors.Green}rises.");
        });

        return HookResult.Continue;
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo _)
    {
        _phoenixUses = new HashSet<CCSPlayerController>();
        foreach (var player in Utilities.GetPlayers())
        {
            if (!Ensure.Player(player).IsValid().Check() || !IsAssignedTo(player!))
            {
                return HookResult.Continue;
            }

            Server.NextFrame(() =>
            {
                player.SetNewHealth(50);
            });
        }

        return HookResult.Continue;
    }
}