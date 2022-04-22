using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SnakeWpf
{
    public enum SnakeDirection
    {
        Left,
        Up,
        Right,
        Down
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private const int InitialLength = 3;
        private const int InitialInterval = 300;
        private const double SpeedIncreaseFactor = 1.1;
        private const int ApplesToIncreaseSpeed = 5;

        private readonly Random random = new Random(Environment.TickCount);
        private readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.DataBind);
        private PointCollection snake = new PointCollection();
        private Point appleCoordinates = new Point(-100, -100);
        private int applesCollected;
        private int level;
        private bool gameOver;

        public MainViewModel()
        {
            timer.Interval = TimeSpan.FromMilliseconds(InitialInterval);
            timer.Tick += (s, e) => MoveSnake();

            NewGame = new Command(StartNewGame);
            MoveLeft = new Command(() => FutureDirections.Enqueue(SnakeDirection.Left), () => timer.IsEnabled);
            MoveUp = new Command(() => FutureDirections.Enqueue(SnakeDirection.Up), () => timer.IsEnabled);
            MoveRight = new Command(() => FutureDirections.Enqueue(SnakeDirection.Right), () => timer.IsEnabled);
            MoveDown = new Command(() => FutureDirections.Enqueue(SnakeDirection.Down), () => timer.IsEnabled);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand NewGame { get; }

        public ICommand MoveLeft { get; }

        public ICommand MoveUp { get; }

        public ICommand MoveRight { get; }

        public ICommand MoveDown { get; }

        public int GridWidth { get; } = 20;

        public int GridHeight { get; } = 20;

        public double SnakeWidth { get; } = 0.8;

        public Point AppleCoordinates
        {
            get => appleCoordinates;
            set
            {
                appleCoordinates = value;
                OnPropertyChanged();
            }
        }

        public PointCollection Snake
        {
            get => snake;
            set
            {
                snake = value;
                OnPropertyChanged();
            }
        }

        public int ApplesCollected
        {
            get => applesCollected;
            set
            {
                applesCollected = value;
                OnPropertyChanged();
            }
        }

        public int Level
        {
            get => level;
            set
            {
                level = value;
                OnPropertyChanged();
            }
        }

        public bool GameOver
        {
            get => gameOver;
            set
            {
                gameOver = value;
                OnPropertyChanged();
            }
        }

        public SnakeDirection CurrentDirection { get; set; } = SnakeDirection.Up;

        public Queue<SnakeDirection> FutureDirections { get; } = new Queue<SnakeDirection>();

        private void MoveSnake()
        {
            while (FutureDirections.TryDequeue(out SnakeDirection direction))
            {
                var difference = Math.Abs(CurrentDirection - direction);

                if (difference == 1 || difference == 3)
                {
                    CurrentDirection = direction;
                }

                break;
            }


            var copy = new PointCollection(Snake);
            var top = copy[0];
            Point insertPoint;

            switch (CurrentDirection)
            {
                case SnakeDirection.Left:
                    {
                        insertPoint = new Point(top.X - 1, top.Y);
                        break;
                    }
                case SnakeDirection.Up:
                    {
                        insertPoint = new Point(top.X, top.Y - 1);
                        break;
                    }
                case SnakeDirection.Right:
                    {
                        insertPoint = new Point(top.X + 1, top.Y);
                        break;
                    }
                case SnakeDirection.Down:
                    {
                        insertPoint = new Point(top.X, top.Y + 1);
                        break;
                    }
            }

            if (insertPoint.X < 0 || insertPoint.X > GridWidth || insertPoint.Y < 0 || insertPoint.Y > GridHeight ||
                copy.Take(copy.Count - 1).Any(x => x == insertPoint))
            {
                timer.Stop();
                GameOver = true;
            }
            else
            {
                copy.Insert(0, insertPoint);

                if (AppleCoordinates == insertPoint)
                {
                    Snake = copy;
                    ApplesCollected++;
                    CreateNewApple();
                    TryIncreaseSpeed();
                }
                else
                {
                    copy.RemoveAt(copy.Count - 1);
                    Snake = copy;
                }
            }
        }

        private void StartNewGame()
        {
            timer.Stop();

            GameOver = false;
            ApplesCollected = 0;
            Level = 1;
            FutureDirections.Clear();
            CurrentDirection = SnakeDirection.Up;

            var snake = new PointCollection();

            int x = GridWidth / 2;
            int y = GridHeight / 2;

            for (int i = 0; i < InitialLength; i++)
            {
                snake.Add(new Point(x, i + y));
            }

            Snake = snake;
            CreateNewApple();

            timer.Start();
        }

        private void CreateNewApple()
        {
            while (true)
            {
                var x = random.Next(0, GridWidth);
                var y = random.Next(0, GridHeight);

                var point = new Point(x, y);

                if (!Snake.Any(x => x == point))
                {
                    AppleCoordinates = point;
                    break;
                }
            }
        }

        private void TryIncreaseSpeed()
        {
            if (ApplesCollected % ApplesToIncreaseSpeed == 0)
            {
                timer.Interval = TimeSpan.FromMilliseconds(timer.Interval.TotalMilliseconds / SpeedIncreaseFactor);
                Level++;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
