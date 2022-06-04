using System.Collections;
using System.Text;

class Player
{
    public int PlayerId { get; }
    public string Token { get; }
    public int Streak { get; set; }
    public List<ValueTuple<int, int>> History { get; }
    public bool HasWon { get; set; }
    public int NumMoves => History.Count;

    public Player(int playerId, string token)
    {
        if (playerId < 1)
        {
            throw new ArgumentException($"PlayerId must be integer > 0");
        }

        History = new List<(int, int)>();
        PlayerId = playerId;
        Token = token;
        HasWon = false;
    }
    
}

class TicTacToe
{
    public int Size { get; }
    public int WinCondition { get; }
    public string[,] State { get; }
    private Dictionary<int, Player> _playerMap = new Dictionary<int, Player>();
    private int _currentPlayer;
    private int _moveCounter;
    private readonly int _maximumAllowableMoves;

    public TicTacToe(int size, int winCondition)
    {
        if (size <= 0)
        {
            throw new ArgumentException($"Game grid size must be > 0");
        }

        if (winCondition <= 1)
        {
            throw new ArgumentException($"Winning sequence length must be > 1");
        }
        Size = size;
        WinCondition = winCondition;
        State = new string[Size,Size];
        _playerMap[1] = new Player(1, "X");
        _playerMap[2] = new Player(2, "#");
        _currentPlayer = 1;
        _moveCounter = 0;
        _maximumAllowableMoves = Size * Size;
    }

    private void DrawBoard()
    {
        Console.WriteLine();
        StringBuilder rowState;
        for (int row = 0; row < Size; row++)
        {
            rowState = new StringBuilder();
            Console.WriteLine(string.Concat(Enumerable.Repeat("     |", Size - 1)) + "     ");
            for (int col = 0; col < Size; col++)
            {
                rowState.Append($"  {State[row, col]}  ");
                if (col != Size - 1)
                {
                    rowState.Append("|");
                }
            }

            Console.WriteLine(rowState);
            Console.WriteLine(string.Concat(Enumerable.Repeat("     |", Size - 1)) + "     ");
            if (row != Size - 1)
            {
                Console.WriteLine(string.Concat(Enumerable.Repeat("------", Size)));
            }
        }
    }

    private int GameEnds()
    {
        foreach (var entry in _playerMap)
        {
            if (entry.Value.Streak >= WinCondition)
            {
                Console.WriteLine($"Player {entry.Value.PlayerId} has won the game!");
                _playerMap[entry.Key].HasWon = true;
                return entry.Key;
            }
        }
        if (_moveCounter >= _maximumAllowableMoves)
        {
            Console.WriteLine("It's a draw");
            return 0;
        }
        
        return -1; // Game continues
    }

    private bool isNeighbor(ValueTuple<int, int> move1, ValueTuple<int, int> move2)
    {
               // Same row, adjacent column
        return (move1.Item1 == move2.Item1 && Math.Abs(move1.Item2 - move2.Item2) == 1) ||
               // Same column, adjacent rows
               (move1.Item2 == move2.Item2 && Math.Abs(move1.Item1 - move2.Item1) == 1) ||
               // Diagonal neighbors
               (Math.Abs(move1.Item1 - move2.Item1) == 1 && Math.Abs(move1.Item2 - move2.Item2) == 1);
    }

    private void UpdateHistory(int player, ValueTuple<int, int> move)
    {
        if (_playerMap[player].History.Count == 0)
        {
            _playerMap[player].Streak = 1;
        }
        foreach (var previousMove in _playerMap[player].History) 
        {
            if (isNeighbor(move, previousMove))
            {
                _playerMap[player].Streak++;
                Console.WriteLine($"Streak of {player} extended by 1. Now {_playerMap[player].Streak}");
                break;
            }
        }
        _playerMap[player].History.Add(move);
        _moveCounter++;
    }

    private ValueTuple<int, int> GetPlayerMove(int player)
    {
        Console.WriteLine($"Player {player} - Choose a field to place your token");
        ConsoleKeyInfo key = Console.ReadKey();
        try
        {
            int gridNum = int.Parse(key.KeyChar.ToString());
            if (gridNum > Size * Size)
            {
                throw new IndexOutOfRangeException();
            }
            
            int gridRow = (int)(gridNum / Size);
            int gridCol = gridRow == 0? gridNum: gridNum - gridRow * Size;
            
            if (_playerMap.Values.Any(p => p.Token == State[gridRow, gridCol]))
            {
                throw new InvalidDataException();
            }

            var playerMove = new ValueTuple<int, int>(gridRow, gridCol);
            UpdateHistory(player, playerMove);
            return playerMove;
        }
        catch (Exception)
        {
            Console.WriteLine($"Invalid input: {key.KeyChar}");
            return GetPlayerMove(player);
        }
    }

    public void InitializeGame()
    {
        int counter = 0;
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                State[i, j] = counter.ToString();
                counter++;
            }
        }
        DrawBoard();
        _currentPlayer = 1;
    }

    private void SwitchPlayer()
    {
        _currentPlayer = _currentPlayer == 1 ? 2 : 1;
    }
    
    public void Start()
    {
        InitializeGame();
        while (GameEnds() < 0)
        {
            var (playerRow, playerCol) = GetPlayerMove(_currentPlayer);
            State[playerRow, playerCol] = _playerMap[_currentPlayer].Token;
            SwitchPlayer();
            DrawBoard();
        }
    }
    
    public static void Main(string[] args)
    {
        TicTacToe game = new TicTacToe(3, 3);
        game.Start();
    }
}