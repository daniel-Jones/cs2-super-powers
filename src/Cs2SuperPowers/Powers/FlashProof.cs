using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class FlashProof(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 3;
    public override string Name => "Flash Proof";
    
    public override string Description => $"You cannot be blind by flashes. You will gain new flash every time your flash explodes.";
    
    public override char ChatColor => ChatColors.LightBlue;

    public override string HtmlColor => "#D6E6FF";
    

    protected override void OnInitialize()
    {
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventPlayerBlind>(OnPlayerBlinded);
        RegisterEventHandler<EventFlashbangDetonate>(OnFlashbangDetonate);
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
        var player = @event.Userid;

        if (!Ensure.Player(player).IsValid().Check() || !IsAssignedTo(player!))
        {
            return HookResult.Continue;
        }
        
        var playerPawn = player!.PlayerPawn.Value;
        playerPawn!.FlashDuration = 0.0f;

        return HookResult.Continue;
    }
    
    private HookResult OnFlashbangDetonate(EventFlashbangDetonate @event, GameEventInfo info)
    {
        var player = @event.Userid;                

        if (!Ensure.Player(player).IsValid().IsAlive().Check() || !IsAssignedTo(player!))
        {
            return HookResult.Continue;
        }
        
        player!.GiveNamedItem("weapon_flashbang");
        
        return HookResult.Continue;
    }
}