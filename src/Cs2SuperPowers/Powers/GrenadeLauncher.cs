using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;
using Cs2SuperPowers.Utils;

namespace Cs2SuperPowers.Powers;

public class GrenadeLauncher(IPlayerHud playerHud) : BasePower(playerHud)
{
    public override int Id => 11;
    public override string Name => "Grenade Launcher";
    
    public override string Description => $"Every shot from your pistol launches a grenade.";
    
    public override char ChatColor => ChatColors.Red;

    public override string HtmlColor => "#FF0000";

    protected override void OnInitialize()
    {
        RegisterEventHandler<EventWeaponFire>(OnWeaponFire);
    }

private HookResult OnWeaponFire(EventWeaponFire @event, GameEventInfo info)
{
    var controller = @event.Userid;
    if (!Ensure.Player(controller).IsValid().Check() || !IsAssignedTo(controller!))
        return HookResult.Continue;
    
    // only pistols
    var weapon = controller!.PlayerPawn.Value?.WeaponServices?.ActiveWeapon.Value;
    if (weapon == null)
        return HookResult.Continue;

    // List of pistol weapon names (classnames)
    var pistolClassNames = new[]
    {
        "weapon_glock", "weapon_usp_silencer", "weapon_hkp2000", "weapon_p250",
        "weapon_fiveseven", "weapon_tec9", "weapon_cz75a", "weapon_deagle",
        "weapon_revolver", "weapon_elite", "weapon_negev"
    };

    if (!pistolClassNames.Contains(weapon.DesignerName))
        return HookResult.Continue;

    var pawn = controller!.PlayerPawn.Value;
    if (pawn == null || !pawn.IsValid)
        return HookResult.Continue;

    var pos = pawn.AbsOrigin;
    if (pos == null) return HookResult.Continue;
    pos.Z += 50f;

    // Forward vector from the player's view angles
    var ang = pawn.V_angle;
    float yaw = ang.Y * MathF.PI / 180f;
    float pitch = ang.X * MathF.PI / 180f;
    var forward = new Vector(
        MathF.Cos(yaw) * MathF.Cos(pitch),
        MathF.Sin(yaw) * MathF.Cos(pitch),
        -MathF.Sin(pitch)
    );

    // Spawn slightly in front of the player along their facing direction (horizontal only)
    const float spawnAhead = 32f;
    pos = new Vector(
        pos.X + forward.X * spawnAhead,
        pos.Y + forward.Y * spawnAhead,
        pos.Z
    );

    // Give it some speed and an upward arc
    const float speed = 800f;
    var velocity = new Vector(
        forward.X * speed,
        forward.Y * speed,
        forward.Z * speed + 200f
    );

    var createdGrenade = GrenadeSpawner.CreateGrenade(pos, ang, velocity, pawn);
    
    return HookResult.Continue;
}

}