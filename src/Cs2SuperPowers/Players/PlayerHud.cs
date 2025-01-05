using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Cs2SuperPowers.Players;

public class PlayerHud : IPlayerHud
{
    private readonly Dictionary<CCSPlayerController, DisposableMessage> _playerMessages = new();

    public void Initialize(BasePlugin pluginRef)
    {
        pluginRef.RegisterListener<Listeners.OnTick>(OnTick);
    }

    public void ShowMessage(CCSPlayerController player, string message, TimeSpan? duration = null)
    {
        duration ??= TimeSpan.FromSeconds(10);
        if (_playerMessages.ContainsKey(player))
        {
            _playerMessages.Remove(player);
        }
        _playerMessages.Add(player, new DisposableMessage(message, DateTime.UtcNow.Add(duration.Value)));
    }
    
    private void OnTick()
    {
        foreach (var playerMessage in _playerMessages)
        {
            if (playerMessage.Value.DisplayUntil >= DateTime.UtcNow)
            {
                playerMessage.Key.PrintToCenterHtml(playerMessage.Value.Message);
            }
            else
            {
                Server.NextFrame(() => _playerMessages.Remove(playerMessage.Key));
            }
        }
    }
}

record DisposableMessage(string Message, DateTime DisplayUntil);