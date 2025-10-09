using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;

namespace Cs2SuperPowers.Powers;

public class BlindingLight(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 22;
    public override string Name => "Blinding Light";
    
    public override string Description => $"Inflict a blinding flash of light that disorients enemies. Press E (Use) to activate the power.";
    
    public override char ChatColor => ChatColors.BlueGrey;

    public override string HtmlColor => "#cdcece";

    private readonly List<BlindingLightInfo> _registrations = new();

    private const int SecondsBetweenActivations = 15;
    
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
            _registrations.Add(new BlindingLightInfo(player, DateTime.MinValue));
        }

        return HookResult.Continue;
    }

    private void OnTick()
    {
        foreach (var registration in _registrations)
        {
            var usePressed = registration.Player.Buttons.HasFlag(PlayerButtons.Use);
            var canUsePower = (DateTime.UtcNow - registration.LastUse).TotalSeconds > SecondsBetweenActivations;
            if (usePressed && canUsePower)
            {
                ActivatePower(registration);
            }
            else if (!canUsePower && usePressed)
            {
                var secondsBeforeNextUse =
                    SecondsBetweenActivations - (int)(DateTime.UtcNow - registration.LastUse).TotalSeconds;
                registration.Player
                    .PrintToCenterAlert($"You need to wait {ChatColors.DarkRed}{secondsBeforeNextUse} {ChatColors.Default}seconds before using the power.");
            }
        }
    }

    private void ActivatePower(BlindingLightInfo registration)
    {
        var heProjectile = Utilities.CreateEntityByName<CFlashbangProjectile>("flashbang_projectile");
        if (heProjectile == null || !heProjectile.IsValid)
        {
            return;
        }

        var player = registration.Player;
        var pos = player.PlayerPawn.Value!.AbsOrigin;
        pos!.Z += 10;

        heProjectile.TicksAtZeroVelocity = 100;
        heProjectile.TeamNum = player.TeamNum;
        heProjectile.Teleport(pos, null, new Vector(0, 0, -10));
        heProjectile.DispatchSpawn();
        heProjectile.AcceptInput("InitializeSpawnFromWorld", player.PlayerPawn.Value, player.PlayerPawn.Value);
        heProjectile.DetonateTime = 0;
        
        registration.UpdateUse();
    }
}

public record BlindingLightInfo(CCSPlayerController Player, DateTime LastUse)
{
    public DateTime LastUse { get; private set; } = LastUse;
    
    public void UpdateUse()
    {
        LastUse = DateTime.UtcNow;
    }
}