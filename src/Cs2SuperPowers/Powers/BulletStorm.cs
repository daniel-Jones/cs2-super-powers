using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class BulletStorm(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 13;
    public override string Name => "Bullet Storm";
    
    public override string Description => $"Unleash an unending torrent of bullets, emptying your magazines without fear of depletion";
    
    public override char ChatColor => ChatColors.Olive;

    public override string HtmlColor => "#0000FF";
    
    protected override void OnInitialize()
    {
        RegisterEventHandler<EventWeaponFire>(OnWeaponFire);
        RegisterEventHandler<EventWeaponReload>(OnWeaponReload);
    }

    private HookResult OnWeaponFire(EventWeaponFire @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Ensure.Player(player).IsValid().Check() || !IsAssignedTo(player!))
        {
            return HookResult.Continue;
        }
        
        ApplyInfiniteAmmo(player!);
        return HookResult.Continue;
    }
    
    private HookResult OnWeaponReload(EventWeaponReload @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Ensure.Player(player).IsValid().Check() || !IsAssignedTo(player!))
        {
            return HookResult.Continue;
        }
        
        ApplyInfiniteAmmo(player!);
        return HookResult.Continue;
    }
    
    private static void ApplyInfiniteAmmo(CCSPlayerController player)
    {
        var activeWeaponHandle = player.PlayerPawn.Value?.WeaponServices?.ActiveWeapon;
        if (activeWeaponHandle?.Value != null)
        {
            activeWeaponHandle.Value.Clip1 = 100;
        }
    }
}