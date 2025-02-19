using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NGT_DemoApp;

internal class PrinterCommunication
{
    private static void Main()
    {
        const int printerPort = 4001;

        var slotNumber = GetUserInput("Slot Number", 10);
        var printerIp = GetUserInput("Printer IP", "192.168.173.50");
        var formatFileName = GetUserInput("Format File Name", "Doc001.txt");

        try
        {
            var formatFilePath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), formatFileName);

            if (File.Exists(formatFilePath))
            {
                // Select slot
                Console.WriteLine("Set slot to " + slotNumber);
                var selectSlotCommand = "X36" + slotNumber.ToString("000");
                SendCommandToPrinter(printerIp, printerPort, CreateHeader(selectSlotCommand));

                // Send format
                Console.WriteLine("Load format " + formatFileName);
                var formatContent = File.ReadAllText(formatFilePath, Encoding.GetEncoding(1252));
                SendCommandToPrinter(printerIp, printerPort, formatContent, 1000);

                // Read and set variables
                var variables = ReadPrinterVariables(printerIp, printerPort);
                SetPrinterVariables(printerIp, printerPort, variables);

                // Read Error
                var error = SendCommandToPrinter(printerIp, printerPort, CreateHeader("R99"));
                error = error.Length >= 2 ? error.Substring(error.Length - 2) : error; // last 2 characters
                Console.WriteLine("Error: " + error);
            }
            else
            {
                Console.WriteLine("Error: Format file not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    private static Dictionary<string, string> ReadPrinterVariables(string printerIp, int printerPort)
    {
        var variables = new Dictionary<string, string>();
        var response = SendCommandToPrinter(printerIp, printerPort, CreateHeader("R83VI"));

        if (response.Contains("ZOK"))
        {
            var items = response.Substring(response.IndexOf("ZOK") + 3).Split('\v');

            foreach (var item in items)
                if (!string.IsNullOrWhiteSpace(item))
                {
                    var valueResponse = SendCommandToPrinter(printerIp, printerPort, CreateHeader("R84" + item + '\v'));
                    var value = valueResponse.Substring(valueResponse.IndexOf("ZOK") + 3).Split('\v')[0];
                    variables.Add(item, value);
                }
        }

        return variables;
    }

    private static void SetPrinterVariables(string printerIp, int printerPort, Dictionary<string, string> variables)
    {
        foreach (var variable in variables)
        {
            var userValue = GetUserInput($"Enter value for {variable.Key}", variable.Value);
            var command = '\x1B' + "X56" + variable.Key + '\v' + userValue + '\v';
            SendCommandToPrinter(printerIp, printerPort, command);
        }
    }

    private static string GetUserInput(string prompt, string defaultValue)
    {
        Console.Write($"{prompt} ({defaultValue}): ");
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }

    private static int GetUserInput(string prompt, int defaultValue)
    {
        Console.Write($"{prompt} ({defaultValue}): ");
        var input = Console.ReadLine();
        return int.TryParse(input, out var result) ? result : defaultValue;
    }

    private static string SendCommandToPrinter(string printerIp, int printerPort, string command, int sleep = 0)
    {
        var response = "";
        using var client = new TcpClient(printerIp, printerPort);
        using var stream = client.GetStream();

        var commandBytes = Encoding.GetEncoding(1252).GetBytes(command);
        stream.Write(commandBytes, 0, commandBytes.Length);

        // Read response
        Thread.Sleep(sleep);
        var responseBuffer = new byte[1024];
        var bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
        response = Encoding.GetEncoding(1252).GetString(responseBuffer, 0, bytesRead);
        //Console.WriteLine("Response: " + response);

        return response;
    }

    private static string CreateHeader(string command)
    {
        var escapeCharacter = '\x1B'; // Escape character
        return "~1" + (command.Length + 1).ToString("000000") + escapeCharacter + command;
    }
}