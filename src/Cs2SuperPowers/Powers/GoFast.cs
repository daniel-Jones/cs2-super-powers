using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class GoFast(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 18;
    public override string Name => "Go Fast";
    
    public override string Description => $"Your movement speed is increased by 50%";
    
    public override char ChatColor => ChatColors.Green;

    public override string HtmlColor => "#009905";

    protected override void OnInitialize()
    {
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo _)
    {
        foreach (var player in PlayerPowers.Instance.GetPlayersWithPower(this))
        {
            if (!Ensure.Player(player).IsValid().Check())
            {
                continue;
            }
                    
            Server.NextFrame(() =>
            {
                player.SetNewSpeed(player.GetSpeed() * 1.5f);
            });
        }

        return HookResult.Continue;
    }
}