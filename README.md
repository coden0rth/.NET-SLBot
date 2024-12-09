# SecondLifeBot

**SecondLifeBot** is an automation bot for Second Life developed in C# using the LibreMetaverse framework. It features patrol point navigation, object scanning, and customizable configurations, making it an ideal tool for automating various in-world tasks.

---

## Features

- **Patrol Navigation**: The bot moves between predefined patrol points while scanning for objects with specific hover text.
- **Object Scanning**: Detects objects based on hover text and logs relevant information.
- **Messaging**: Built-in functionality to send instant messages (IMs) to a single user or all admins.
- **Configurable**: Supports configuration through a JSON file, allowing easy setup for patrol points, login credentials, search criteria, and admin management.
- **Command System**: Admins can send commands to the bot for dynamic control.
- **Event Handling**: Gracefully handles events like login, avatar loading, and disconnection.

---

## Requirements

- **.NET Version**: Requires .NET Core 6.0 or higher.
- **Dependencies**:
  - LibreMetaverse

---

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/SecondLifeBot.git
   cd SecondLifeBot
   ```

2. Install dependencies:
   - Use your favorite NuGet package manager to install the `LibreMetaverse` library.

3. Build the project using Visual Studio or the .NET CLI:
   ```bash
   dotnet build
   ```

---

## Configuration

The bot uses a JSON configuration file named `botConfig.json`. Below is an example structure:

```json
{
  "FirstName": "comfycode",
  "LastName": "Resident",
  "Password": "your-password",
  "StartRegion": "Krown",
  "StartPosition": { "X": 245, "Y": 128, "Z": 28 },
  "PatrolPoints": [
    { "X": 212, "Y": 128, "Z": 25 },
    { "X": 208, "Y": 111, "Z": 24 },
    { "X": 206, "Y": 117, "Z": 24 }
  ],
  "AdminList": [
    "1f0ee634-151d-41b6-afab-89ea0d7d0784"
  ],
  "SearchHoverText": [
    "Game Offline",
    "Score Updated"
  ]
}
```

### Fields

- **FirstName**: The first name of the bot's Second Life account.
- **LastName**: The last name of the bot's Second Life account.
- **Password**: The password for the bot's Second Life account.
- **PatrolPoints**: A list of coordinates defining the patrol path.
- **AdminList**: A list of UUIDs allowed to issue commands to the bot.
- **SearchHoverText**: A list of strings to match against object hover text during scans.

Place this file in the `Config/` directory.

---

## Usage

1. Run the bot using the compiled executable:
   ```bash
   dotnet run
   ```

2. The bot will:
   - Log in using the credentials from `botConfig.json`.
   - Begin patrolling the defined points.
   - Listen for commands from the admin list.

---

## Commands

The bot will soon support commands via messages.

---

## BotManager Documentation

### Methods

#### `void Teleport(string regionName, Vector3 position)`
- Teleports the bot to a specified region and position.
- Logs the status of the teleport request.

#### `void SendIM(UUID targetUUID, string message)`
- Sends an instant message to a specific UUID.
- Logs the target UUID and message content.

#### `void SendIMToAdmins(string message)`
- Sends an instant message to all admins defined in the configuration.
- Logs the message content and delivery status.

---

## Project Structure

- `Program.cs`: The entry point of the application.
- `BotManager.cs`: Handles bot-specific actions like teleportation and messaging.
- `Movement.cs`: Manages bot movement and obstacle avoidance.
- `ObjectScanner.cs`: Handles object scanning during patrol.
- `Commands.cs`: Processes commands sent by admins.
- `BotConfiguration.cs`: Defines the structure for the configuration file.
- `Loader.cs`: Handles loading and parsing of the configuration.

---

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a feature branch.
3. Commit your changes.
4. Open a pull request.

---

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.

---

## Acknowledgments

Special thanks to the LibreMetaverse community for their framework and support!
