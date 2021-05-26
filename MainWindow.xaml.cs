using Eatsnake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Eatsnake
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //color
        private readonly Brush _lightColor = new SolidColorBrush(Color.FromRgb(248, 248, 255));
        private readonly Brush _weightColor = new SolidColorBrush(Color.FromRgb(211, 211, 211));
        private readonly Brush _snakeColor = new SolidColorBrush(Color.FromRgb(255, 140, 0));

        
        public int apple_X;
        public int apple_Y;
        private int score = 0;
        private bool isPause = false;
        public Direction CurrentDirection;
        public Level CurrentLevel = Level.Low;
        public List<RectangleElementModel> Snake { get; set; } = new List<RectangleElementModel>();
        public RectangleElementModel[][] Rectangles { get; set; } = new RectangleElementModel[20][];
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var i in Enumerable.Range(0, 20))
            {
                var temp = new RectangleElementModel[20];
                foreach (var j in Enumerable.Range(0, 20))
                {
                    var color = _lightColor;
                    if (i % 2 == 0 && j % 2 == 0)
                    {
                        color = _weightColor;
                    }

                    if (i % 2 != 0 && j % 2 != 0)
                    {
                        color = _weightColor;
                    }

                    var rect = new Rectangle()
                    {
                        Fill = color,
                        Width = 20,
                        Height = 20,
                    };
                    Canvas.SetLeft(rect, i * 20);
                    Canvas.SetTop(rect, j * 20);
                    container.Children.Add(rect);
                    temp[j] = new RectangleElementModel()
                    {
                        X = i,
                        Y = j,
                        Rectangle = rect,
                        LastBrush = rect.Fill
                    };
                }
                Rectangles[i] = temp;
            }
           
            //蛇头黄色
            Rectangles[(int)GenerateHead().X][(int)GenerateHead().Y].Rectangle.Fill = _snakeColor;
            Snake.Add(Rectangles[(int)GenerateHead().X][(int)GenerateHead().Y]);

            //随机生成苹果到某个位置
            RandomGenerateApple();

            this.count.Text = $"分数:{score}";
            this.level.Text = $"难度:{this.CurrentLevel}";

            //开始
            RefreshSnake();
        }

        private void RandomGenerateApple()
        {
            //随机生成某个位置 i，j
            var random = new Random();
            while (true)
            {
                apple_X = random.Next(20);
                apple_Y = random.Next(20);
                if (Snake.FirstOrDefault(o => o.X == apple_X && o.Y == apple_Y) != null)
                    continue;
                Canvas.SetLeft(apple, 20 * apple_X);
                Canvas.SetTop(apple, 20 * apple_Y);
                break;
            }
        }

        private Point GenerateHead()
        {
            var random = new Random();
            var i = random.Next(20);
            var j = random.Next(20);

            return new Point() { X = i, Y = j };
        }

        private async void RefreshSnake()
        {
            while (true)
            {
                try
                {
                    await Task.Delay((int)CurrentLevel);
                    if (this.CurrentDirection == Direction.Default || this.isPause)
                        continue;

                    switch (CurrentDirection)
                    {
                        case Direction.Top:
                            UpdateSnake(0, -1);
                            break;
                        case Direction.Bottom:
                            UpdateSnake(0, 1);
                            break;
                        case Direction.Left:
                            UpdateSnake(-1, 0);
                            break;
                        case Direction.Right:
                            UpdateSnake(1, 0);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }
            }
        }

        private void UpdateSnake(int update_x, int update_y)
        {
            var last = Snake.LastOrDefault();
            last.Rectangle.Fill = last.LastBrush;
            for (var i = Snake.Count - 1; i > 0; i--)
            {
                Snake[i] = Snake[i - 1];
            }

            if (Snake[0].X + update_x >= 20 ||
                Snake[0].X + update_x < 0 ||
                Snake[0].Y + update_y >= 20 ||
                Snake[0].Y + update_y < 0 ||
                Snake.FirstOrDefault(o => o.X == Snake[0].X + update_x && o.Y == Snake[0].Y + update_y) != null
                )
            {
                //game over
                throw new Exception("游戏结束");
            }

            //get apple
            if (Snake[0].X + update_x == this.apple_X && Snake[0].Y + update_y == this.apple_Y)
            {
                this.score += 10000 / (int)this.CurrentLevel;
                this.count.Text = $"分数:{score}";
                this.level.Text = $"难度:{this.CurrentLevel}";
                if (this.score >= 300)
                {
                    this.CurrentLevel = Level.High;
                }
                else if (this.score >= 100) 
                {
                    this.CurrentLevel = Level.Mid;
                }

                last.Rectangle.Fill = _snakeColor;
                Snake.Add(last);
                RandomGenerateApple();
            }
            Snake[0] = Rectangles[Snake[0].X + update_x][Snake[0].Y + update_y];
            Snake[0].Rectangle.Fill = _snakeColor;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Up) 
            {
                if (this.CurrentDirection == Direction.Bottom)
                    return;

                this.CurrentDirection = Direction.Top;
            }

            else if (e.Key == System.Windows.Input.Key.Down)
            {
                if (this.CurrentDirection == Direction.Top)
                    return;
                this.CurrentDirection = Direction.Bottom;
            }

            else if (e.Key == System.Windows.Input.Key.Left)
            {
                if (this.CurrentDirection == Direction.Right)
                    return;
                this.CurrentDirection = Direction.Left;
            }

            else if (e.Key == System.Windows.Input.Key.Right)
            {
                if (this.CurrentDirection == Direction.Left)
                    return;
                this.CurrentDirection = Direction.Right;
            }

            else if (e.Key == System.Windows.Input.Key.Space)
            {
                this.isPause = !this.isPause;
            }
        }
    }

    public enum Direction
    {
        Default,
        Left,
        Top,
        Right,
        Bottom
    }

    public enum Level
    {
        Low = 500,
        Mid = 250,
        High = 150,
    }
}
