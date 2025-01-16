using Match3.Data;
using Match3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Match3
{
    public partial class MainPage : ContentPage
    {
        private readonly AppDbContext _context;
        private readonly string _username;

        const int GridSize = 5;
        const int TileTypes = 5;
        int[,] board;
        int score = 0;
        (int row, int col)? firstTile = null;
        bool isProcessing = false; 
        Random random = new Random();

        public MainPage(string username)
        {
            InitializeComponent();
            InitializeGame(); 
            _username = username;
            _context = new AppDbContext(new DbContextOptions<AppDbContext>());

        }

         
        void InitializeGame()
        {
            board = new int[GridSize, GridSize];

             
            do
            {
                for (int row = 0; row < GridSize; row++)
                {
                    for (int col = 0; col < GridSize; col++)
                    {
                        board[row, col] = random.Next(1, TileTypes + 1);  
                    }
                }
            } while (!HasPlayableCombinations());   

             
            GameGrid.Children.Clear();
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    var button = new Button
                    {
                        //Text = board[row, col].ToString(),
                        ImageSource = GetTileImageSource(board[row, col]),
                        CommandParameter = (row, col)
                    };

                    button.Clicked += OnTileClicked;

                     
                    GameGrid.Children.Add(button);
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                }
            }

            DisplayScore();
        }

         
        ImageSource GetTileImageSource(int tileType)
        {
            return tileType switch
            {
                1 => "redtile.png",    
                2 => "bluetile.png",   
                3 => "greentile.png",  
                4 => "yellowtile.png",  
                5 => "purpletile.png",  
                _ => "dotnet_bot.png"  
            };
        }

         
        async void OnTileClicked(object sender, EventArgs e)
        {
            if (isProcessing) return;  
            isProcessing = true;

            var button = sender as Button;
            var (row, col) = ((int, int))button.CommandParameter;

            // If the first tile is not selected, set the first tile clicked
            if (firstTile == null)
            {
                firstTile = (row, col);
                //button.BackgroundColor = Colors.Gray;
            }
            else
            {
                var (firstRow, firstCol) = firstTile.Value;

                // If the clicked tile is adjacent to the first tile, swap them
                if (Math.Abs(row - firstRow) + Math.Abs(col - firstCol) == 1)
                {
                    
                    SwapTiles(firstRow, firstCol, row, col);

                     
                    if (CheckForMatches())
                    {
                         
                        RemoveMatches();
                        DropNewTiles();
                    }
                    else
                    {
                        
                        SwapTiles(firstRow, firstCol, row, col);
                    }
                }

                 
                firstTile = null;
            }

            
            DisplayScore();
            isProcessing = false;  
        }

         
        void SwapTiles(int row1, int col1, int row2, int col2)
        {
            var temp = board[row1, col1];
            board[row1, col1] = board[row2, col2];
            board[row2, col2] = temp;

             
            var button1 = GetButtonAt(row1, col1);
            var button2 = GetButtonAt(row2, col2);

            //button1.Text = board[row1, col1].ToString();
            button1.ImageSource = GetTileImageSource(board[row1, col1]);

            //button2.Text = board[row2, col2].ToString();
            button2.ImageSource = GetTileImageSource(board[row2, col2]);
        }

         
        Button GetButtonAt(int row, int col)
        {
            return GameGrid.Children
                .Cast<Button>()
                .FirstOrDefault(b => Grid.GetRow(b) == row && Grid.GetColumn(b) == col);
        }

        
        bool CheckForMatches()
        {
            var matchedTiles = new HashSet<(int, int)>();

            
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize - 2; col++)
                {
                    if (board[row, col] == board[row, col + 1] && board[row, col + 1] == board[row, col + 2] && board[row, col] != 0)
                    {
                        matchedTiles.Add((row, col));
                        matchedTiles.Add((row, col + 1));
                        matchedTiles.Add((row, col + 2));
                    }
                }
            }

             
            for (int col = 0; col < GridSize; col++)
            {
                for (int row = 0; row < GridSize - 2; row++)
                {
                    if (board[row, col] == board[row + 1, col] && board[row + 1, col] == board[row + 2, col] && board[row, col] != 0)
                    {
                        matchedTiles.Add((row, col));
                        matchedTiles.Add((row + 1, col));
                        matchedTiles.Add((row + 2, col));
                    }
                }
            }

             
            return matchedTiles.Count > 0;
        }

        
        void RemoveMatches()
        {
            var matchedTiles = new HashSet<(int, int)>();

             
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize - 2; col++)
                {
                    if (board[row, col] == board[row, col + 1] && board[row, col + 1] == board[row, col + 2] && board[row, col] != 0)
                    {
                        matchedTiles.Add((row, col));
                        matchedTiles.Add((row, col + 1));
                        matchedTiles.Add((row, col + 2));
                    }
                }
            }

             
            for (int col = 0; col < GridSize; col++)
            {
                for (int row = 0; row < GridSize - 2; row++)
                {
                    if (board[row, col] == board[row + 1, col] && board[row + 1, col] == board[row + 2, col] && board[row, col] != 0)
                    {
                        matchedTiles.Add((row, col));
                        matchedTiles.Add((row + 1, col));
                        matchedTiles.Add((row + 2, col));
                    }
                }
            }

             
            foreach (var (row, col) in matchedTiles)
            {
                board[row, col] = 0;  

                var button = GetButtonAt(row, col);
                button.Text = "";  
                //button.BackgroundColor = Colors.Gray;
            }

             
            score += matchedTiles.Count;
        }

         
        void DropNewTiles()
        {
            bool tilesDropped = false;

             
            for (int col = 0; col < GridSize; col++)
            {
                for (int row = GridSize - 1; row >= 0; row--)
                {
                    if (board[row, col] == 0)   
                    {
                        for (int r = row - 1; r >= 0; r--)
                        {
                            if (board[r, col] != 0)
                            {
                                board[row, col] = board[r, col];
                                board[r, col] = 0;

                                
                                var button = GetButtonAt(r, col);
                                //button.Text = board[row, col].ToString();
                                button.ImageSource = GetTileImageSource(board[row, col]);

                                var newButton = GetButtonAt(row, col);
                                //newButton.Text = board[row, col].ToString();
                                newButton.ImageSource = GetTileImageSource(board[row, col]);
                                tilesDropped = true;
                                break;
                            }
                        }
                    }
                }
            }

            
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (board[row, col] == 0)
                    {
                        board[row, col] = random.Next(1, TileTypes + 1);  
                        var button = GetButtonAt(row, col);
                        //button.Text = board[row, col].ToString();
                        button.ImageSource = GetTileImageSource(board[row, col]);
                    }
                }
            }

            
            if (CheckForMatches())
            {
                RemoveMatches();
                DropNewTiles();
            }
        }

         
        bool HasPlayableCombinations()
        {
             
            return !CheckForMatches();
        }

        async void OnSurrender(object sender, EventArgs e)
        {
            var playerHighestScore = await _context.HighScores.FirstOrDefaultAsync(p => p.Player.Username == _username);

            if (playerHighestScore == null)
            {
                var highScore = new HighScore
                {
                    Player = new Player { Username = _username },
                    Score = score,
                };

               await _context.HighScores.AddAsync(highScore);
               await _context.SaveChangesAsync();
            }

            if(playerHighestScore != null && score > playerHighestScore.Score)
            {
                playerHighestScore.Score = score;
                _context.HighScores.Update(playerHighestScore);
                await _context.SaveChangesAsync();
            }

            bool choice = await DisplayAlert("Game Over!",$"Your score is: {score}","Reset Game","Back to main menu");
            if(choice)
            {
                ResetGame();
            }
            else
            {
                await Navigation.PopToRootAsync();
            }
        }
         
        void ResetGame()
        {
            InitializeGame();
            score = 0;
            DisplayScore();
        }

         
        void DisplayScore()
        {
            ScoreLabel.Text = $"Score: {score}";
        }
    }
}
