using CounterStrikeSharp.API.Core;

namespace Cs2SuperPowers.Players;

public interface IPlayerHud {
    void Initialize(BasePlugin pluginRef);
    void ShowMessage(CCSPlayerController player, string message, TimeSpan? duration = null);
}