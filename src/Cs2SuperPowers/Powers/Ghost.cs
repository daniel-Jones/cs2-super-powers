using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class Ghost : BasePower
{
    public override int Id => 7;
    public override string Name => "The Ghost";
    
    public override string Description => $"You are invisible to other players - but will show up for few seconds after you will take/give damage.";
    
    public override char ChatColor => ChatColors.White;
    
    private readonly Random _random = new Random();
    
    public override string HtmlColor => "#FFFFFF";
    protected override void OnInitialize()
    {
        RegisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
    }

    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        foreach (var timer in Plugin!.Timers)
        {
            timer.Kill();
        }
        Plugin.Timers.Clear();
        
        foreach (var player in PlayerPowers.Instance.GetPlayersWithPower(this))
        {
            Server.NextFrame(() =>
            {
                SetPlayerVisibility(player, true);
            });
        }
        
        return HookResult.Continue;
    }

    private HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
    {
        var validPlayersWithPower = PlayerPowers.Instance
            .GetPlayersWithPower(this)
            .Where(player => Ensure.Player(player).IsValid().Check());
        
        foreach (var player in validPlayersWithPower)
        {
            Server.NextFrame(() =>
            {
                SetPlayerVisibility(player, false);
            });
        }

        return HookResult.Continue;
    }

    private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        var attacker = @event.Attacker;
        var victim = @event.Userid;

        if (!Ensure.Player(victim).IsValid().IsAlive().Check() || 
            !Ensure.Player(attacker).IsValid().Check())
        {
            return HookResult.Continue;
        }

        if (IsAssignedTo(victim!))
        {
            SetPlayerVisibility(victim!, true);
            Plugin!.AddTimer(_random.Next(1, 5), () =>
            {
                SetPlayerVisibility(victim!, false);
            });
        }
        else if (IsAssignedTo(attacker!))
        {
            SetPlayerVisibility(attacker!, true);
            Plugin!.AddTimer(_random.Next(1, 5), () =>
            {
                SetPlayerVisibility(attacker!, false);
            });
        }
        
        return HookResult.Continue;
    }
    
    private void SetPlayerVisibility(CCSPlayerController player, bool visible)
    {
        var playerPawn = player.PlayerPawn.Value;
        if (playerPawn != null)
        {
            var color = visible 
                ? Color.FromArgb(255, 255, 255, 255) 
                : Color.FromArgb(0, 255, 255, 255);
            var shadowStrength = visible ? 1.0f : 0.0f;

            playerPawn.Render = color;
            playerPawn.ShadowStrength = shadowStrength;
            Utilities.SetStateChanged(playerPawn, "CBaseModelEntity", "m_clrRender");

            var activeWeapon = playerPawn.WeaponServices?.ActiveWeapon.Value;
            if (activeWeapon != null && activeWeapon.IsValid)
            {
                activeWeapon.Render = color;
                activeWeapon.ShadowStrength = shadowStrength;
                Utilities.SetStateChanged(activeWeapon, "CBaseModelEntity", "m_clrRender");
            }

            var myWeapons = playerPawn.WeaponServices?.MyWeapons;
            if (myWeapons != null)
            {
                foreach (var gun in myWeapons)
                {
                    var weapon = gun.Value;
                    if (weapon != null)
                    {
                        weapon.Render = color;
                        weapon.ShadowStrength = shadowStrength;
                        Utilities.SetStateChanged(weapon, "CBaseModelEntity", "m_clrRender");
                    }
                }
            }
        }
    }
}