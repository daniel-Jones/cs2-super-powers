using System.Text;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;

namespace Cs2SuperPowers.Powers;

public abstract class BasePower : ISuperPower
{
    private readonly HashSet<ulong> _playerIds = new();

    protected BasePlugin? Plugin { get; private set; }
    
    public abstract int Id { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract char ChatColor { get; }
    public abstract string HtmlColor { get; }

    public void Initialize(BasePlugin pluginRef)
    {
        Plugin = pluginRef;
        OnInitialize();
    }
    
    protected abstract void OnInitialize();

    public virtual void AssignToPlayer(CCSPlayerController player)
    {
        _playerIds.Add(player.SteamID);
        player.PrintToChat($"You have been granted the power of {ChatColor}{Name}");
        player.PrintToChat(Description);

        var centerMessageBuilder = new StringBuilder();
        centerMessageBuilder.Append("<font class='fontSize-l' class='fontWeight-Bold' color='#FFFFFF'>Your super power:</font> <br>");
        centerMessageBuilder.Append($"<font class='fontSize-l' class='fontWeight-Bold' color='{HtmlColor}'>{Name}</font> <br>");
        Server.NextFrame(() => player.PrintToCenterHtml(centerMessageBuilder.ToString(), 60 * 60 * 1000));
    }

    public virtual void UnassignFromPlayer(CCSPlayerController player)
    {
        _playerIds.Remove(player.SteamID);
    }
    
    protected void RegisterEventHandler<T>(BasePlugin.GameEventHandler<T> handler, HookMode hookMode = HookMode.Post) 
        where T : GameEvent
    {
        Plugin!.RegisterEventHandler(handler, hookMode);
    }

    protected bool IsAssignedTo(CCSPlayerController player)
    {
        return _playerIds.Contains(player.SteamID);
    }
}