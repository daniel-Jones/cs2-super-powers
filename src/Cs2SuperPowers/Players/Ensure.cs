using CounterStrikeSharp.API.Core;

namespace Cs2SuperPowers.Players;

public static class Ensure
{
    public static ValidationBuilder<CCSPlayerController> Player(CCSPlayerController? player)
    {
        return new ValidationBuilder<CCSPlayerController>(player);
    }
}

public class ValidationBuilder<TEntity>(TEntity? entity)
{
    private readonly Queue<IValidation<TEntity>> _validations = new();

    public void Add(IValidation<TEntity> validation)
    {
        _validations.Enqueue(validation);
    }

    public bool Check()
    {
        return _validations.All(v => v.Validate(entity));
    }
}