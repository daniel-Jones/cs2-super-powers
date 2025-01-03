using CounterStrikeSharp.API.Core;

namespace Cs2SuperPowers.Players.PlayerValidations;

public class IsValid : IValidation<CCSPlayerController>
{
    public bool Validate(CCSPlayerController? player)
    {
        return player?.IsValid == true;
    }
}

public static class IsValidExtensions
{
    public static ValidationBuilder<CCSPlayerController> IsValid(this ValidationBuilder<CCSPlayerController> builder)
    {
        builder.Add(new IsValid());
        return builder;
    }
}
