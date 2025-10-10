using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;
using Cs2SuperPowers.Utils;

namespace Cs2SuperPowers.Powers;

public class FinalMission(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 8;
    public override string Name => "Final Mission";

    public override string Description => $"Upon death, unleash a powerful explosion that eliminates all players within a specified radius, turning your sacrifice into a final act of devastation.";

    public override char ChatColor => ChatColors.Purple;

    public override string HtmlColor => "#F5CB42";

    private const float ExplosionRadius = 500.0f;
    private const int ExplosionDamage = 999;

    protected override void OnInitialize()
    {
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }

    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!Ensure.Player(player).IsValid().Check() || !IsAssignedTo(player!))
        {
            return HookResult.Continue;
        }

        var pos = player!.PlayerPawn.Value!.AbsOrigin;
        pos!.Z += 10;

        var heProjectile = GrenadeSpawner.CreateGrenade(pos, QAngle.Zero, new Vector(0, 0, -10));

        if (heProjectile == null || !heProjectile.IsValid)
        {
            return HookResult.Continue;
        }

        heProjectile.TicksAtZeroVelocity = 100;
        heProjectile.TeamNum = player.TeamNum;
        heProjectile.Damage = ExplosionDamage;
        heProjectile.DmgRadius = (int)ExplosionRadius;
        heProjectile.DispatchSpawn();
        heProjectile.AcceptInput("InitializeSpawnFromWorld", player.PlayerPawn.Value, player.PlayerPawn.Value, "");
        heProjectile.DetonateTime = 0;

        Server.PrintToChatAll($"{ChatColors.DarkRed}► {ChatColors.Green} {player.PlayerName}: {ChatColors.Lime}explodes!!!");

        var fileNames = new[] { "radiobotfallback01", "radiobotfallback02", "radiobotfallback04" };
        Plugin!.AddTimer(0.1f, () =>
        {
            var randomFile = fileNames[new Random().Next(fileNames.Length)];
            player.ExecuteClientCommand($"play vo/agents/balkan/{randomFile}.vsnd");
        });

        return HookResult.Continue;
    }
}