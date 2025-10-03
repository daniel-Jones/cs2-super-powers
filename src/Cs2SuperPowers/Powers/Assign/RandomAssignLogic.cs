using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Cs2SuperPowers.Players;
using Cs2SuperPowers.Players.PlayerValidations;

namespace Cs2SuperPowers.Powers.Assign;

public class RandomAssignLogic : IPowerAssignLogic
{
    public void Initialize(BasePlugin pluginRef, IReadOnlyCollection<ISuperPower> powers)
    {
        pluginRef.RegisterEventHandler<EventPlayerDisconnect>((@event, _) =>
        {
            var player = @event.Userid;

            if (!Ensure.Player(player).IsValid().Check()) return HookResult.Continue;
            PlayerPowers.Instance.RemovePower(player!);

            return HookResult.Continue;
        });


        pluginRef.RegisterEventHandler<EventRoundStart>((_, _) =>
        {
            var gameRules = Utilities
                .FindAllEntitiesByDesignerName<CBaseEntity>("cs_gamerules")
                .Single()
                .As<CCSGameRulesProxy>()
                .GameRules!;
            
            if (gameRules.WarmupPeriod)
            {
                return HookResult.Continue;
            }
            
            var random = new Random();            
            foreach (var player in Utilities.GetPlayers())
            {
                var randomPower = powers.OrderBy(_ => random.Next()).First();
                PlayerPowers.Instance.AddPower(player!, randomPower);
            }
            
            foreach (var player in Utilities.GetPlayers())
            {
                var team = player.Team;                

                player.PrintToChat($"Your {ChatColors.Blue}Team {ChatColors.Default}powers are:");
                PlayerPowers.Instance
                    .GetAssignedPowers()
                    .Where(x => x.Player.Team == team)
                    .Select(x => {
                        player.PrintToChat($"{ChatColors.DarkRed} ► {x.Player.PlayerName} {ChatColors.Default}: {x.Power.ChatColor}{x.Power.Name}");                        
                        return x;
                    })
                    .ToList();
            }

            return HookResult.Continue;
        }, HookMode.Pre);

        pluginRef.RegisterEventHandler<EventPlayerDeath>((@event, _) =>
        {
            var victim = @event.Userid;
            var attacker = @event.Attacker;

            if (victim == null || attacker == null || victim == attacker) return HookResult.Continue;

            if (!PlayerPowers.Instance.TryGetValue(attacker, out var attackerPower))
            {
                return HookResult.Continue;
            }

            victim.PrintToChat($"You have been killed by {ChatColors.DarkRed}{attacker.PlayerName}{ChatColors.Lime}");
            victim.PrintToChat($"His/her super power was: {attackerPower!.ChatColor}{attackerPower.Name}");

            return HookResult.Continue;
        });
    }
}
