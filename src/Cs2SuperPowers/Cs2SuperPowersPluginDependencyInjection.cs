using CounterStrikeSharp.API.Core;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Powers;
using Cs2SuperPowers.Powers.Assign;
using Microsoft.Extensions.DependencyInjection;

namespace Cs2SuperPowers;

public class Cs2SuperPowersPluginDependencyInjection : IPluginServiceCollection<Cs2SuperPowersPlugin>
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<ISuperPower, RiseFromTheDead>();
        services.AddScoped<ISuperPower, FlashProof>();
        services.AddScoped<ISuperPower, IronHead>();
        services.AddScoped<ISuperPower, Phoenix>();
        services.AddScoped<ISuperPower, Rambo>();
        services.AddScoped<ISuperPower, ArmBreaker>();
        services.AddScoped<ISuperPower, Ghost>();
        services.AddScoped<ISuperPower, FinalMission>();
        services.AddScoped<ISuperPower, Jackpot>();
        services.AddScoped<ISuperPower, IAmChicken>();
        services.AddScoped<ISuperPower, OneShot>();
        services.AddScoped<ISuperPower, Vampire>();
        services.AddScoped<ISuperPower, Twister>();
        services.AddScoped<ISuperPower, PowerThief>();
        
        services.AddScoped<IPowerAssignLogic, RandomAssignLogic>();
        services.AddScoped<IPlayerHud, PlayerHud>();
    }
}