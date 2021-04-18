using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }

    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;
        protected int AliveCells;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        [JsonConstructor]
        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        public Board(bool[,] board)
        {
            CellSize = 1;
            AliveCells = 0;
            Cells = new Cell[board.GetLength(0), board.GetLength(1)];
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Cells[x, y] = new Cell() { IsAlive = board[x, y] };
                    AliveCells += Convert.ToInt32(board[x, y]);
                }
            }
            ConnectNeighbors();
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            AliveCells = 0;
            foreach (var cell in Cells)
            {
                cell.IsAlive = rand.NextDouble() < liveDensity;
                AliveCells += Convert.ToInt32(cell.IsAlive);
            }
                
        }

        public void Advance()
        {
            AliveCells = 0;
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
            {
                cell.Advance();
                AliveCells += Convert.ToInt32(cell.IsAlive);
            }
                
        }
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }

        public string FiledToString()
        {
            string result = "";
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    var cell = Cells[col, row];
                    if (cell.IsAlive)
                    {
                        result += "*";
                    }
                    else
                    {
                        result += " ";
                    }
                }
                result += "\n";
            }
            return result;
        }

        public string Representation()
        {
            string result = FiledToString();
            result += $"\nShape: ({Rows}, {Columns}) | Alive cells: {AliveCells}   \n";
            return result;
        }
    }
    partial class Program
    {
        enum States
        {
            Run,
            Pause,
            Menu
        }

        static string settingsPath;
        static Board board;
        static States state = States.Menu;

        static void Render(string[] messages = null, bool clear = false)
        {
            if (clear)
            {
                Console.Clear();
            }
            Console.SetCursorPosition(0, 0);
            Console.Write(board.Representation());
            if (messages != null)
            {
                Console.Write("\n");
                foreach (var message in messages)
                {
                    Console.WriteLine(message);
                }
            }
        }

        static void Main(string[] args)
        {
            settingsPath = "settings.json";
            if (args.Length == 2)
            {
                settingsPath = args[1];
            }
            Console.CancelKeyPress += new ConsoleCancelEventHandler(pauseHandler);
            while (true)
            {
                switch (state)
                {
                    case States.Run:
                    {
                        Render(new string[] { "Press CTRL+C to pause" });
                        board.Advance();
                        Thread.Sleep(100);
                        break;
                    }
                    case States.Pause:
                    {
                        HandlePause();
                        break;
                    }
                    case States.Menu:
                    {
                        HandleMainMenu();
                        break;
                    }
                }
            }
        }
    }
}