using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers;

public class PanicTeleport(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 16;
    public override string Name => "Panic Teleport";
    
    public override string Description => $"Save a single location with the use key, then teleport back to that position whenever you press the use key again.";
    
    public override char ChatColor => ChatColors.BlueGrey;

    public override string HtmlColor => "#cdcece";
    
    private readonly List<PanicTeleportInfo> _registrations = new();
    
    protected override void OnInitialize()
    {
        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }
    
    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var player = @event.Userid;
        var registration = _registrations
            .FirstOrDefault(r => r.Player == player);
        if (registration != null)
        {
            _registrations.Remove(registration);
        }
        return HookResult.Continue;
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        foreach (var player in PlayerPowers.Instance.GetPlayersWithPower(this))
        {
            _registrations.Add(new PanicTeleportInfo(player, null));
        }

        return HookResult.Continue;
    }
    
    private void OnTick()
    {
        foreach (var registration in _registrations)
        {
            var usePressed = registration.Player.Buttons.HasFlag(PlayerButtons.Use);
            if (usePressed && !registration.WasUsePressedLastTick)
            {
                // Key was just pressed (not held)
                HandleUseKeyPress(registration);
            }
            registration.WasUsePressedLastTick = usePressed;
        }
    }
    
    private void HandleUseKeyPress(PanicTeleportInfo registration)
    {
        var player = registration.Player;
        
        if (!Ensure.Player(player).IsValid().Check() || player.PlayerPawn?.Value == null)
        {
            return;
        }
        
        if (registration.SavedLocation == null)
        {
            // Save current location
            var currentPos = player.PlayerPawn.Value.AbsOrigin;
            if (currentPos != null)
            {
                registration.SavedLocation = new Vector(currentPos.X, currentPos.Y, currentPos.Z);
                registration.SavedLocation.Z += 10;
                player.PrintToCenterAlert($"{ChatColor}Location saved! Press use key again to teleport back.");
            }
        }
        else
        {
            // Teleport to saved location
            var savedPos = registration.SavedLocation;
            player.PlayerPawn.Value.Teleport(savedPos, null, null);
        }
    }
}

public record PanicTeleportInfo(CCSPlayerController Player, Vector? SavedLocation)
{
    public Vector? SavedLocation { get; set; } = SavedLocation;
    public bool WasUsePressedLastTick { get; set; } = false;
}