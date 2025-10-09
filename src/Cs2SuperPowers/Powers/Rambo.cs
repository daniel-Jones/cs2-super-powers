using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class Rambo(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 5;
    public override string Name => "John Rambo";
    
    public override string Description => $"You start with additional random health, negev, kevlar and hand grenades.";
    
    public override char ChatColor => ChatColors.Green;

    public override string HtmlColor => "#009905";

    protected override void OnInitialize()
    {
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo _)
    {
        var random = new Random();
        foreach (var player in PlayerPowers.Instance.GetPlayersWithPower(this))
        {
            if (!Ensure.Player(player).IsValid().Check())
            {
                continue;
            }
                    
            Server.NextFrame(() =>
            {
                var newHealth = 100 + random.Next(150, 400);
                
                player.GiveNamedItem("weapon_negev");
                player.GiveNamedItem("item_assaultsuit");
                player.GiveNamedItem("weapon_hegrenade");

                player.SetNewHealth(newHealth);
            });
        }

        return HookResult.Continue;
    }
}