using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class ThrowRocks(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 20;
    public override string Name => "Throw Rocks";
    
    public override string Description => "Spawn with infinite decoys. A direct hit with a decoy instantly kills the victim.";
    
    public override char ChatColor => ChatColors.Grey;

    public override string HtmlColor => "#AAAAAA";

    protected override void OnInitialize()
    {
        RegisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
        RegisterEventHandler<EventDecoyStarted>(OnDecoyThrown);
        RegisterEventHandler<EventDecoyDetonate>(OnDecoyDetonate);
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
    }

    private HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo _)
    {
        foreach (var player in Utilities.GetPlayers())
        {
            if (!Ensure.Player(player).IsValid().IsAlive().Check() || !IsAssignedTo(player!))
            {
                continue;
            }

            Server.NextFrame(() =>
            {
                // Ensure player starts with at least one decoy
                player.GiveNamedItem("weapon_decoy");
                player.GiveNamedItem("weapon_decoy");
            });
        }

        return HookResult.Continue;
    }

    private HookResult OnDecoyThrown(EventDecoyStarted @event, GameEventInfo _)
    {
        var player = @event.Userid;
        if (!Ensure.Player(player).IsValid().Check() || !IsAssignedTo(player!))
        {
            return HookResult.Continue;
        }

        // Replenish immediately to simulate infinite decoys
        player!.GiveNamedItem("weapon_decoy");
        return HookResult.Continue;
    }

    private HookResult OnDecoyDetonate(EventDecoyDetonate @event, GameEventInfo _)
    {
        var player = @event.Userid;
        if (!Ensure.Player(player).IsValid().Check() || !IsAssignedTo(player!))
        {
            return HookResult.Continue;
        }

        // Safety: also replenish on detonation
        player!.GiveNamedItem("weapon_decoy");
        return HookResult.Continue;
    }

    private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        var attacker = @event.Attacker;
        var victim = @event.Userid;

        if (
            !Ensure.Player(victim).IsValid().IsAlive().Check() || 
            !Ensure.Player(attacker).IsValid().Check() ||
            attacker == victim ||
            !IsAssignedTo(attacker!))
        {
            return HookResult.Continue;
        }

        // If the damage came from a decoy impact, instantly kill
        // Many CS2 event payloads include the weapon name; if present and equals "decoy", execute the effect.
        try
        {
            var weaponNameProp = @event.GetType().GetProperty("Weapon");
            var weaponName = weaponNameProp?.GetValue(@event) as string;
            if (string.Equals(weaponName, "decoy", StringComparison.OrdinalIgnoreCase))
            {
                var pawn = victim!.PlayerPawn.Value;
                if (pawn != null)
                {
                    pawn.Health = 0;
                    Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
                    return HookResult.Continue;
                }
            }
        }
        catch
        {
            // Ignore reflection errors; just continue normally
        }

        return HookResult.Continue;
    }
}


