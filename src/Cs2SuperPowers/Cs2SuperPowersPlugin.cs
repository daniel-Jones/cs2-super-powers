using CounterStrikeSharp.API.Core;
using Cs2SuperPowers.Powers.Assign;
using Microsoft.Extensions.Logging;

namespace Cs2SuperPowers;

public class Cs2SuperPowersPlugin(IEnumerable<ISuperPower> superPowers, IPowerAssignLogic powerAssignLogic) : BasePlugin
{
    public override string ModuleName => "Cs2SuperPowersPlugin";
    public override string ModuleVersion => "0.1.0";
    public override string ModuleAuthor => "Latoch";
    public override string ModuleDescription => "This CS2 server plugin unleashes the divine within! Experience a chaotic and exhilarating gameplay where players wield incredible superpowers.";

    private readonly ISuperPower[] _superPowers = superPowers.ToArray();

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("Loading {ModuleName} with HotReload = {HotReload}", ModuleName, hotReload);
        base.Load(hotReload);
        foreach (var superPower in _superPowers)
        {
            Logger.LogInformation("Initializing SuperPower {Name} with Id = {Id}", superPower.Name, superPower.Id);
            superPower.Initialize(this);
            Logger.LogInformation("Finished Initializing SuperPower {Name}", superPower.Name);
        }
        powerAssignLogic.Initialize(this, _superPowers);
        Logger.LogInformation("Finished loading {ModuleName} with HotReload = {HotReload}", ModuleName, hotReload);
    }
    
    
}