using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweaper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Velkommen til Minesweeper!"); // fortæller spillet det skal starte
            MinesweeperGame game = new MinesweeperGame();
            game.Run();
        }
    }

    internal class MinesweeperGame
    {
        private const int gridSize = 10; // Ændre dette for at ændre størrelsen på brættet
        private const int bombCount = 7; // Ændre dette for at ændre antallet af bomber

        private bool[,] isBomb = new bool[gridSize, gridSize]; // Et 2D-array til at gemme information om bomber
        private bool[,] isRevealed = new bool[gridSize, gridSize]; // Et 2D-array til at gemme information om afslørede celler
        private bool[,] isFlagged = new bool[gridSize, gridSize]; // Et 2D-array til at gemme information om flag

        public void Run()
        {
            InitializeGame(); // Initialiser spillet ved at placere bomber tilfældigt
            PrintGrid(); // Udskriv spillebrættet

            while (true)
            {
                Console.WriteLine("Vælg handling:");
                Console.WriteLine("1. Indtast koordinater");
                Console.WriteLine("2. Placer flag");
                Console.Write("Valg (1/2): ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Indtast række (0-" + (gridSize - 1) + "): ");
                    int row = int.Parse(Console.ReadLine());

                    Console.Write("Indtast kolonne (0-" + (gridSize - 1) + "): ");
                    int col = int.Parse(Console.ReadLine());

                    if (row < 0 || row >= gridSize || col < 0 || col >= gridSize)
                    {
                        Console.WriteLine("Ugyldige koordinater. Prøv igen.");
                        continue;
                    }

                    if (isFlagged[row, col])
                    {
                        Console.WriteLine("Denne celle er flagget. Fjern flaget for at afsløre.");
                        continue;
                    }

                    if (isBomb[row, col])
                    {
                        Console.WriteLine("Boom! Du tabte.");
                        RevealAllBombs(); // Vis alle bomber
                        PrintGrid(); // Udskriv brættet med alle bomber synlige
                        break;
                    }

                    isRevealed[row, col] = true;
                    if (CheckWinCondition())
                    {
                        Console.WriteLine("Tillykke! Du har vundet spillet.");
                        PrintGrid(); // Udskriv brættet med alle celler afsløret
                        break;
                    }

                    PrintGrid(); // Udskriv opdateret spillebræt
                }
                else if (choice == "2")
                {
                    Console.Write("Indtast række (0-" + (gridSize - 1) + ") for flag: ");
                    int row = int.Parse(Console.ReadLine());

                    Console.Write("Indtast kolonne (0-" + (gridSize - 1) + ") for flag: ");
                    int col = int.Parse(Console.ReadLine());

                    if (row < 0 || row >= gridSize || col < 0 || col >= gridSize)
                    {
                        Console.WriteLine("Ugyldige koordinater. Prøv igen.");
                        continue;
                    }

                    if (isRevealed[row, col])
                    {
                        Console.WriteLine("Denne celle er allerede afsløret. Du kan ikke placere et flag her.");
                        continue;
                    }

                    isFlagged[row, col] = !isFlagged[row, col]; // Toggle flag
                    PrintGrid(); // Udskriv opdateret spillebræt
                }
                else
                {
                    Console.WriteLine("Ugyldigt valg. Prøv igen.");
                }
            }

            Console.WriteLine("Spillet er slut. Vil du starte en ny runde? (Ja/Nej)"); // Ja laver et nyt spil, nej lukker spillet ned
            string playAgain = Console.ReadLine();

            if (playAgain.Equals("Ja", StringComparison.OrdinalIgnoreCase))
            {
                ResetGame(); // Start en ny runde
            }
            else
            {
                Console.WriteLine("Farvel!");
            }
        }

        private void InitializeGame()
        {
            // Placer bomber tilfældigt
            Random rand = new Random();
            for (int i = 0; i < bombCount; i++)
            {
                int row, col;
                do
                {
                    row = rand.Next(0, gridSize);
                    col = rand.Next(0, gridSize);
                } while (isBomb[row, col]);
                isBomb[row, col] = true;
            }
        }

        private void PrintGrid()
        {
            Console.Clear();
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    if (isRevealed[row, col])
                    {
                        if (isBomb[row, col])
                        {
                            Console.Write("* "); // Bomber vises som "*"
                        }
                        else
                        {
                            int bombCount = CountSurroundingBombs(row, col);
                            Console.Write(bombCount + " ");
                        }
                    }
                    else
                    {
                        if (isFlagged[row, col])
                        {
                            Console.Write("F "); // Flag vises som "F"
                        }
                        else
                        {
                            Console.Write(". "); // Ikke-afslørede celler vises som ". "
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        private int CountSurroundingBombs(int row, int col)
        {
            int bombCount = 0;

            // Tjek de omkringliggende celler for bomber
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;

                    // Undgå at gå uden for brættets grænser
                    if (newRow >= 0 && newRow < gridSize && newCol >= 0 && newCol < gridSize)
                    {
                        if (isBomb[newRow, newCol])
                        {
                            bombCount++;
                        }
                    }
                }
            }

            return bombCount;
        }

        private void RevealAllBombs()
        {
            // Vis alle bomberne
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    if (isBomb[row, col])
                        isRevealed[row, col] = true;
                }
            }
        }

        private bool CheckWinCondition()
        {
            // Tjek om spillet er vundet
            int unrevealedCount = 0;
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    if (!isRevealed[row, col] && !isBomb[row, col])
                        unrevealedCount++;
                }
            }

            return unrevealedCount == 0;
        }

        private void ResetGame()
        {
            // Nulstil spillet ved at fjerne bomber, afslørede celler og flag
            Array.Clear(isBomb, 0, isBomb.Length);
            Array.Clear(isRevealed, 0, isRevealed.Length);
            Array.Clear(isFlagged, 0, isFlagged.Length);
            Run(); // Start en ny runde
        }
    }
}
