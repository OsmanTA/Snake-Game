using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace snakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int rows = 15, columns = 15;
        private readonly Image[,] gridImages;
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            {GridValue.Empty, Images.Empty },
            {GridValue.Snake, Images.Body },
            {GridValue.Food, Images.Food }
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            {Direction.Up, 0 },
            {Direction.Right, 90 },
            {Direction.Down, 180 },
            {Direction.Left, 270 }
        };

        private gameState gameStatus;
        private bool gameRunning;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameStatus = new gameState(rows, columns);
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameStatus = new gameState(rows, columns);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameStatus.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameStatus.ChangeDirections(Direction.Left);
                    break;
                case Key.Right:
                    gameStatus.ChangeDirections(Direction.Right);
                    break;
                case Key.Up:
                    gameStatus.ChangeDirections(Direction.Up);
                    break;
                case Key.Down:
                    gameStatus.ChangeDirections(Direction.Down);
                    break;
                case Key.A:
                    gameStatus.ChangeDirections(Direction.Left);
                    break;
                case Key.D:
                    gameStatus.ChangeDirections(Direction.Right);
                    break;
                case Key.W:
                    gameStatus.ChangeDirections(Direction.Up);
                    break;
                case Key.S:
                    gameStatus.ChangeDirections(Direction.Down);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while(!gameStatus.GameOver)
            {
                await Task.Delay(100);
                gameStatus.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, columns];
            GameGrid.Rows= rows;
            GameGrid.Columns= columns;
            GameGrid.Width = GameGrid.Height * (columns / (double)rows);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        // makes center of image the rotation point
                        RenderTransformOrigin = new Point(.5, .5)
                    };
                images[i, j] = image;
                GameGrid.Children.Add(image);
                }
            }
            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"Score {gameStatus.Score}";
        }

        private void DrawGrid()
        {
            for (int i=0; i < rows; i++) 
            {
                for (int j = 0; j < columns; j++)
                {
                    GridValue gridVal = gameStatus.Grid[i, j];
                    gridImages[i, j].Source = gridValToImage[gridVal];
                    // Makes it so only the head image rotates
                    gridImages[i, j].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameStatus.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Column];
            image.Source = Images.Head;
            // grabs the rotation amount
            int rotation = dirToRotation[gameStatus.Dir];
            // rotates image
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameStatus.SnakePositions());
            
            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Column].Source = source;
                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(500);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Press Any Key To Start";
        }
    }
}
