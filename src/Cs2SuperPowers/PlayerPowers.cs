using CounterStrikeSharp.API.Core;
using Cs2SuperPowers.Powers;

namespace Cs2SuperPowers;

public class PlayerPowers
{
    private static PlayerPowers? _instance;
    public static PlayerPowers Instance => _instance ??= new PlayerPowers();
    
    private readonly IDictionary<CCSPlayerController, ISuperPower> _superPowers 
        = new Dictionary<CCSPlayerController, ISuperPower>();

    public void RemovePower(CCSPlayerController player)
    {
        if (!_superPowers.TryGetValue(player, out var superPower)) return;
        
        superPower.UnassignFromPlayer(player);
        _superPowers.Remove(player);
    }
    
    public void AddPower(CCSPlayerController player, ISuperPower superPower)
    {
        if (_superPowers.TryGetValue(player, out var existingSuperPower))
        {
            existingSuperPower.UnassignFromPlayer(player);
            _superPowers.Remove(player);
        };
        
        superPower.AssignToPlayer(player);
        _superPowers.Add(player, superPower);
    }

    public bool TryGetValue(CCSPlayerController attacker, out ISuperPower? power) 
        => _superPowers.TryGetValue(attacker, out power);

    public IEnumerable<(CCSPlayerController Player, ISuperPower Power)> GetAssignedPowers() 
        => _superPowers.Select(x => (x.Key, x.Value));

    public IEnumerable<CCSPlayerController> GetPlayersWithPower(ISuperPower power)
        => GetAssignedPowers().Where(x => x.Power.Id == power.Id).Select(x => x.Player);
}