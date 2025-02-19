# NGT-DemoApp

## Overview
NGT-DemoApp is a simple console application for communicating with a networked Diagraph NGT printer. It allows users to:
- Select a print slot.
- Load and send a print format file.
- Read and set printer variables.
- Retrieve error codes from the printer.

The application interacts with the printer over TCP/IP using predefined commands.

## Features
- Connect to a printer using an IP address and port.
- Send print format files to the printer.
- Read and update printer variables dynamically.
- Retrieve error messages from the printer.

## Requirements
- .NET 6.0 or later
- Windows operating system (for file path compatibility)
- A networked printer supporting the required command set

## Installation
1. Clone the repository:
   ```sh
   git clone https://github.com/diagraph-de/NGT-DemoApp.git
   ```
2. Open the project in Visual Studio or any .NET-compatible IDE.
3. Restore dependencies (if applicable):
   ```sh
   dotnet restore
   ```
4. Build the project:
   ```sh
   dotnet build
   ```

## Usage
Run the application from a terminal or command prompt:
```sh
   dotnet run
```
The program will prompt for user inputs such as:
- Slot Number (default: 10)
- Printer IP Address (default: 192.168.173.50)
- Format File Name (default: Doc001.txt)

### Example Workflow
1. Set the slot number.
2. Load and send a print format file from the desktop.
3. Read and set printer variables.
4. Retrieve and display any printer error codes.

## Configuration
Modify the `Main` method in `PrinterCommunication.cs` if you need to change default values or add additional commands.

## Code Structure
- `SendCommandToPrinter`: Sends a formatted command to the printer.
- `ReadPrinterVariables`: Retrieves the list of available printer variables.
- `SetPrinterVariables`: Allows the user to update variable values.
- `CreateHeader`: Generates the necessary command headers.
- `GetUserInput`: Handles user input with default values.

## Troubleshooting
- **Error: Format file not found**: Ensure that the file exists on the desktop.
- **Connection issues**: Verify that the printer is accessible via the configured IP and port.
- **Unexpected responses**: The printer's command set may differ; check printer documentation.

## License
This project is licensed under the MIT License.

## Contact
For support, please contact the Diagraph team or submit an issue in the repository.

