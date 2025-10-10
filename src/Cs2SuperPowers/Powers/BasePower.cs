using System.Text;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;
using Cs2SuperPowers.Players;

namespace Cs2SuperPowers.Powers;

public abstract class BasePower(IPlayerHud playerHud) : ISuperPower
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

    public virtual void AssignToPlayer(CCSPlayerController player, bool showMessages)
    {
        _playerIds.Add(player.SteamID);
        
        if (showMessages)
        {
            player.PrintToChat($"You have been granted the power of {ChatColor}{Name}");
            player.PrintToChat(Description);

            var centerMessageBuilder = new StringBuilder();
            centerMessageBuilder.Append("<font class='fontSize-l' class='fontWeight-Bold' color='#FFFFFF'>Your superpower:</font> <br>");
            centerMessageBuilder.Append($"<font class='fontSize-l' class='fontWeight-Bold' color='{HtmlColor}'>{Name}</font> <br>");
            centerMessageBuilder.Append($"<font class='fontSize-s' class='fontWeight-Normal' color='#FFFFFF'>{Description}</font> <br>");
            playerHud.ShowMessage(player, centerMessageBuilder.ToString(), TimeSpan.FromSeconds(10));
        }
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
    
    protected void RegisterListener<T>(T handler) where T : Delegate 
    {
        Plugin!.RegisterListener(handler);
    }

    protected bool IsAssignedTo(CCSPlayerController player)
    {
        return _playerIds.Contains(player.SteamID);
    }
}