using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;
using Cs2SuperPowers.Utils;

namespace Cs2SuperPowers.Powers;

public class AirStrike(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 21;
    public override string Name => "Air Strike";
    
    public override string Description => "Decoys thrown by you launch an airstrike when they explode.";
    
    public override char ChatColor => ChatColors.Red;

    public override string HtmlColor => "#FF0000";

    protected override void OnInitialize()
    {
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventDecoyDetonate>(OnDecoyDetonate);
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
                player.GiveNamedItem("weapon_decoy");
            });
        }

        return HookResult.Continue;
    }

    private HookResult OnDecoyDetonate(EventDecoyDetonate @event, GameEventInfo _)
    {
        var player = @event.Userid;
        if (!Ensure.Player(player).IsValid().Check() || !IsAssignedTo(player!))
        {
            return HookResult.Continue;
        }
        var pos = new Vector(@event.X, @event.Y, @event.Z);
        //player!.PrintToChat($"Airstrike incoming at: X={pos.X}, Y={pos.Y}, Z={pos.Z}");
        player!.GiveNamedItem("weapon_decoy");

        // Airstrike: spawn grenades above the target, spread over an area, falling down
        var random = new Random();
        int grenadeCount = 20;
        float airstrikeHeight = 800.0f; // Height above the detonation point
        float spreadRadius = 300.0f;    // How far from the center grenades can land

        for (int i = 0; i < grenadeCount; i++)
        {
            // Random offset in a circle around the target
            double angle = random.NextDouble() * 2 * Math.PI;
            double radius = Math.Sqrt(random.NextDouble()) * spreadRadius; // sqrt for uniform distribution

            float offsetX = (float)(Math.Cos(angle) * radius);
            float offsetY = (float)(Math.Sin(angle) * radius);

            // Spawn position: above the ground, with random spread
            var spawnPos = new Vector(
                pos.X + offsetX,
                pos.Y + offsetY,
                pos.Z + airstrikeHeight + (float)(random.NextDouble() * 100.0 - 50.0) // small vertical variation
            );

            // Angle: straight down, maybe with a little randomization
            float pitch = -90f + (float)(random.NextDouble() * 10.0 - 5.0); // -90 is straight down
            float yaw = (float)(random.NextDouble() * 360.0);
            var ang = new QAngle(pitch, yaw, 0);

            // Velocity: mostly downward, with a little horizontal for realism
            float downwardSpeed = (float)(random.NextDouble() * 200.0 + 800.0); // 800-1000 units/sec down
            float horizontalSpeed = (float)(random.NextDouble() * 100.0); // up to 100 units/sec sideways

            // Calculate horizontal direction
            float horizAngle = (float)(random.NextDouble() * 2 * Math.PI);
            var horizontal = new Vector(
                (float)Math.Cos(horizAngle) * horizontalSpeed,
                (float)Math.Sin(horizAngle) * horizontalSpeed,
                0
            );

            var velocity = new Vector(
                horizontal.X,
                horizontal.Y,
                -downwardSpeed
            );

            var createdGrenade = GrenadeSpawner.CreateGrenade(spawnPos, ang, velocity);
        }
        return HookResult.Continue;
    }
}


