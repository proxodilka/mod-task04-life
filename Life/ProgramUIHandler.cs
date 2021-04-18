using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace cli_life
{
    partial class Program
    { 
        protected static void pauseHandler(object sender, ConsoleCancelEventArgs args)
        {
            state = States.Pause;
            args.Cancel = true;
        }

        static void HandleMainMenu()
        {
            string[] options = new string[] { "Start new game", "Load game state", "Exit" };
            Console.Clear();

            for (int i=0; i < options.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }

            int choise = Convert.ToInt32(Console.ReadLine());
            switch (choise)
            {
                case 1:
                {
                    Console.Clear();
                    Console.WriteLine("Enter filename with game settings (keep empty for default settings):");
                    string filepath = Console.ReadLine();
                    if (filepath == "")
                    {
                        filepath = "settings.json";
                    }
                    try
                    {
                        board = BoardSerializer.DeserializeFromJson(filepath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR: {e.Message}");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        return;
                    }
                    break;
                }
                case 2:
                {
                    Console.Clear();
                    Console.WriteLine("Enter filename with game state (keep empty to get back to the menu):");
                    string filepath = Console.ReadLine();
                    if (filepath == "")
                    {
                        return;
                    }
                    try
                    {
                        board = BoardSerializer.Deserialize(filepath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR: {e.Message}");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        return;
                    }
                    break;
                }
                case 3:
                {
                    System.Environment.Exit(0);
                    break;
                }
            }
            state = States.Pause;
        }

        static void HandlePause()
        {
            Render(new string[] { "The game is on pause...\n", "1. Save state", "2. Resume", "3. Exit to menu" }, true);
            int choise = 0;
            try
            {
                choise = Convert.ToInt32(Console.ReadLine());
            }
            catch { }
            
            switch (choise)
            {
                case 1:
                {
                    Render(new string[] { "Enter filename" }, true);
                    string filepath = Console.ReadLine();
                    try
                    {
                        BoardSerializer.Serialize(board, filepath);
                    }
                    catch(Exception e) 
                    {
                        Console.WriteLine($"ERROR: {e.Message}");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                    }
                    Console.WriteLine("The state has been successfully written to the file!\nPress any key to continue...");
                    Console.ReadKey();
                    break;
                }
                case 3:
                {
                    Console.Clear();
                    state = States.Menu;
                    break;
                }
                case 2:
                default:
                {
                    state = States.Run;
                    Render(null, true);
                    break;
                }
            }
        }
    }
}
