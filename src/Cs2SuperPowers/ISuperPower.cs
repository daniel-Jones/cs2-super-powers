using CounterStrikeSharp.API.Core;

namespace Cs2SuperPowers;

public interface ISuperPower
{
    int Id { get; }
    string Name { get; }
    string Description { get; }
    char ChatColor { get; }

    void Initialize(BasePlugin pluginRef);
    void AssignToPlayer(CCSPlayerController player, bool showMessages);
    void UnassignFromPlayer(CCSPlayerController player);
}