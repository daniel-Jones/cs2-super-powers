using CounterStrikeSharp.API.Core;

namespace Cs2SuperPowers.Players.PlayerValidations;

public class IsAlive : IValidation<CCSPlayerController>
{
    public bool Validate(CCSPlayerController? player)
    {
        return player?.PlayerPawn.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE;
    }
}

public static class IsAliveExtensions
{
    public static ValidationBuilder<CCSPlayerController> IsAlive(this ValidationBuilder<CCSPlayerController> builder)
    {
        builder.Add(new IsAlive());
        return builder;
    }
}