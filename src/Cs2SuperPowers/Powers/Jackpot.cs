using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class Jackpot : BasePower
{
    public override int Id => 9;
    public override string Name => "Jackpot";
    
    public override string Description => $"Begin each round with a random cash bonus, giving you an unpredictable economic advantage over your enemies.";
    
    public override char ChatColor => ChatColors.Yellow;

    public override string HtmlColor => "#D4AF37";  
    
    protected override void OnInitialize()
    {
        RegisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
    }

    private HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo _)
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
                var moneyBonus = random.Next(5000, 15000);
                var moneyServices = player?.InGameMoneyServices;
                if (moneyServices == null) return;

                moneyServices.Account += moneyBonus;
                Utilities.SetStateChanged(player!
                    , "CCSPlayerController", "m_pInGameMoneyServices");
            });
        }

        return HookResult.Continue;
    }
}