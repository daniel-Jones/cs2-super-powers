using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;
using Cs2SuperPowers.Utils;

namespace Cs2SuperPowers.Powers;

public class NotSoDecoy(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 24;
    public override string Name => "Not So Decoy";
    
    public override string Description => "Decoys thrown by you spawn 20 grenades when it explodes.";
    
    public override char ChatColor => ChatColors.Red;

    public override string HtmlColor => "#FF00000";

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
        //player!.PrintToChat($"Decoy detonated at position: X={pos.X}, Y={pos.Y}, Z={pos.Z}");
        player!.GiveNamedItem("weapon_decoy");

   

    // Spawn 20 grenades in a loop with random angles and velocities
        var random = new Random();
        for (int i = 0; i < 20; i++)
        {
            // Random pitch between -45 and 45 degrees, yaw between 0 and 360
            float pitch = (float)(random.NextDouble() * 90.0 - 45.0);
            float yaw = (float)(random.NextDouble() * 360.0);
            var ang = new QAngle(pitch, yaw, 0);

            // Random speed between 400 and 900 units
            float speed = (float)(random.NextDouble() * 500.0 + 400.0);

            // Convert angles to a forward vector
            float pitchRad = pitch * MathF.PI / 180f;
            float yawRad = yaw * MathF.PI / 180f;
            var forward = new Vector(
                MathF.Cos(yawRad) * MathF.Cos(pitchRad),
                MathF.Sin(yawRad) * MathF.Cos(pitchRad),
                -MathF.Sin(pitchRad)
            );

            // Add some random upward velocity for the "jump"
            float upward = (float)(random.NextDouble() * 200.0 + 200.0);

            var velocity = new Vector(
                forward.X * speed,
                forward.Y * speed,
                forward.Z * speed + upward
            );

            var createdGrenade = GrenadeSpawner.CreateGrenadeWithOwner(pos, ang, velocity, player!);
        }
        return HookResult.Continue;
    }
}


