using CounterStrikeSharp.API.Core;

namespace Cs2SuperPowers.Powers.Assign;

public interface IPowerAssignLogic
{
    void Initialize(BasePlugin pluginRef, IReadOnlyCollection<ISuperPower> powers);
}