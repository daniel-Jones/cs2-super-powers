# CS2 Super Powers

CS2 Super Powers is a plugin for Counter-Strike 2 that grants players unique superpowers. Each power provides different abilities that can be used during the game to gain an advantage over opponents.

## Features

- **Rise From the Dead**: 35% chance to revive after death with 70 health.
- **Phoenix Rising**: Reborn after the first death with 120 health.
- Various other powers like FlashProof, IronHead, Rambo, ArmBreaker, Ghost, FinalMission, Jackpot, IAmChicken, OneShot, and Vampire.

## Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/cs2-super-powers.git
    ```
2. Navigate to the project directory:
    ```sh
    cd cs2-super-powers
    ```
3. Build the project using your preferred .NET build tool:
    ```sh
    dotnet build
    ```
4. Before installing the plugin you need to install [MetaMod](https://www.sourcemm.net/downloads.php/?branch=master) and [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) into your server.
5. Copy the compiled plugin to ``game/csgo/addons/counterstrikesharp/cs2-super-powers``

## Configuration

You can configure the plugin by modifying the `Cs2SuperPowersPluginDependencyInjection.cs` file to add or remove superpowers.

## Development

To contribute to the project, follow these steps:

1. Fork the repository.
2. Create a new branch:
    ```sh
    git checkout -b feature-branch
    ```
3. Make your changes.
4. Commit your changes:
    ```sh
    git commit -m "Add new feature"
    ```
5. Push to the branch:
    ```sh
    git push origin feature-branch
    ```
6. Create a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For any questions or suggestions, please open an issue on GitHub or contact the project maintainer.
