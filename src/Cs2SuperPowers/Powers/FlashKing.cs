using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class FlashKing(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 17;
    public override string Name => "Flash King";
    
    public override string Description => $"With this superpower, your flashbangs blind enemies for three times as long.";
    
    public override char ChatColor => ChatColors.LightBlue;

    public override string HtmlColor => "#D6E6FF";
    

    protected override void OnInitialize()
    {
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventPlayerBlind>(OnPlayerBlinded);
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo _)
    {
        foreach (var player in Utilities.GetPlayers())
        {
            if (!Ensure.Player(player).IsValid().IsAlive().Check() || !IsAssignedTo(player!))
            {
                continue;
            }

            Server.NextFrame(() =>
            {
                player.GiveNamedItem("weapon_flashbang");
                player.GiveNamedItem("weapon_flashbang");
            });
        }

        return HookResult.Continue;
    }
    
    private HookResult OnPlayerBlinded(EventPlayerBlind @event, GameEventInfo info)
    {
        var victim = @event.Userid;
        var attacker = @event.Attacker;

        if (!Ensure.Player(victim).IsValid().Check() || !IsAssignedTo(attacker!))
        {
            return HookResult.Continue;
        }
        
        var playerPawn = victim!.PlayerPawn.Value;

        if (attacker != victim && attacker!.TeamNum != playerPawn!.TeamNum)
        {
            playerPawn!.FlashDuration = playerPawn!.FlashDuration * 3;    
        }

        return HookResult.Continue;
    }
}