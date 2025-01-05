using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class IAmChicken(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 10;
    public override string Name => "I Am Chicken!";
    
    public override string Description => $"You become Chicken! You're harder to hit but have 30% of HP.";
    
    public override char ChatColor => ChatColors.Orange;

    public override string HtmlColor => "#FF8B42";

    protected override void OnInitialize()
    {
        RegisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
    }

    private HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
    {
        var random = new Random();
        foreach (var player in PlayerPowers.Instance.GetPlayersWithPower(this))
        {
            if (!Ensure.Player(player).IsValid().Check())
            {
                continue;
            }
            
            var playerPawn = player.PlayerPawn?.Value;
            if (playerPawn == null)
            {
                continue;
            }
            
            Server.NextFrame(() =>
            {
                SetPlayerModel(player, "models/chicken/chicken.vmdl");
                playerPawn!.CBodyComponent!.SceneNode!.GetSkeletonInstance().MaterialGroup.Value =
                    (uint)random.Next(1, 4);
                
                playerPawn.MaxHealth = 30;
                playerPawn.Health = 30;
                Utilities.SetStateChanged(player, "CCSPlayerPawn", "m_iMaxHealth");                
            });
        }

        return HookResult.Continue;
    }
    
    private void SetPlayerModel(CCSPlayerController player, string model)
    {
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null) return;

        Server.NextFrame(() =>
        {
            pawn.SetModel(model);

            var originalRender = pawn.Render;
            pawn.Render = Color.FromArgb(255, originalRender.R, originalRender.G, originalRender.B);
        });
    }
}