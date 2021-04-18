using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace cli_life
{
    class JsonSerializer
    {
        public static T ReadJson<T>(string filepath)
        {
            string json = (new StreamReader(filepath)).ReadToEnd();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    class BoardSerializer
    {
        public static void Serialize(Board board, string filepath)
        {
            string content = board.FieldToString();
            StreamWriter sw = new StreamWriter(filepath);
            try
            {
                sw.Write(content);
            }
            finally
            {
                sw.Close();
            }
        }

        public static Board DeserializeFromJson(string filepath)
        {
            if (!File.Exists(filepath))
            {
                Console.WriteLine("WARNING: File with settings is missing, settings will be set to default.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return new Board(
                    width: 50,
                    height: 20,
                    cellSize: 1,
                    liveDensity: 0.5);
            }
            return JsonSerializer.ReadJson<Board>(filepath);
        }

        public static Board Deserialize(string filepath)
        {
            StreamReader sr = new StreamReader(filepath);
            var board = new List<List<bool>>() { new List<bool>() };

            int i = 0;
            while (!sr.EndOfStream)
            {
                char cell = Convert.ToChar(sr.Read());
                if (cell == '\n')
                {
                    if (!sr.EndOfStream)
                    {
                        board.Add(new List<bool>());
                        i++;
                    }
                }
                else if (cell == ' ' || cell == '\r')
                {
                    board[i].Add(false);
                }
                else
                {
                    board[i].Add(true);
                }  
            }
            sr.Close();

            bool[,] compatible_board = new bool[board[0].Count, board.Count];
            for (i=0; i < board.Count; i++)
            {
                for (int j=0; j<board[0].Count; j++)
                {
                    compatible_board[j, i] = board[i][j];
                }
            }
            return new Board(compatible_board);
        }
    }
}
