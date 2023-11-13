namespace WinFormsApp1
{
    public partial class LoadMapForm : Form
    {
        private int[,] array2D;
        public LoadMapForm()
        {
            (Height, Width) = (400, 400);
            (FormBorderStyle, MaximizeBox) = (FormBorderStyle.FixedDialog, false);
            AddControls();
        }

        private void AddControls()
        {
            Label label = new Label()
            {
                Text = "Выберите карту для загрузки",
                Font = new Font("Vernada", 12),
                AutoSize = true
            };

            Controls.Add(label);
            CenterControl(label);

            Button loadMap = new Button()
            {
                Text = "Обзор",
                Font = new Font("Vernada", 12),
                AutoSize = true
            };
            Controls.Add(loadMap);
            CenterControl(loadMap, label);

            Label labelInfo = new Label()
            {
                Font = new Font("Vernada", 12),
                AutoSize = true
            };

            Controls.Add(label);
            CenterControl(labelInfo, loadMap);

            Button startGame = new Button()
            {
                Text = "Начать игру",
                Font = new Font("Vernada", 16),
                AutoSize = true,
                Enabled = false
            };
            Controls.Add(startGame);
            CenterControl(startGame, labelInfo, 5);

            loadMap.Click += (sender, e) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "txt files (*.txt)|*.txt"
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(openFileDialog.FileName);
                        array2D = new int[lines.Length, lines[0].Split(',').Length];
                        for (int i = 0; i < lines.Length; i++)
                        {
                            string[] values = lines[i].Split(',');
                            for (int j = 0; j < values.Length; j++)
                            {
                                array2D[i, j] = int.Parse(values[j].Trim());
                            }
                        }
                        labelInfo.Text = $"Название карты: {Path.GetFileName(openFileDialog.FileName)}";
                        CenterControl(labelInfo, loadMap);
                        startGame.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            };

            startGame.Click += (sender, e) =>
            {
                GameForm gameForm = new GameForm(array2D);
                gameForm.ShowDialog();
            };
        }
        private void CenterControl(Control? currentControl, Control? lastControl = null, int koeff = 1)
        {
            int centerX = (this.ClientSize.Width - currentControl.Width) / 2;
            int centerY = lastControl != null ? lastControl.Bottom : currentControl.Height;
            if (lastControl != null)
            {
                currentControl.Location = new Point(centerX, centerY + lastControl.Height * koeff);
            }
            else
            {
                currentControl.Location = new Point(centerX, centerY);
            }
        }
    }
    public partial class GameForm : Form
    {
        private readonly int CellSize;
        private readonly int MazeSize;

        private int playerX = 1;
        private int playerY = 0;

        private int[,] array2D;
        public GameForm(int[,] array2D)
        {
            InitializeComponent();
            this.array2D = array2D;
            CellSize = array2D.GetLength(0) * 2;
            MazeSize = array2D.GetLength(1);
            InitializeMaze();
        }

        private void InitializeMaze()
        {
            this.ClientSize = new Size(MazeSize * CellSize, MazeSize * CellSize);
            this.Text = "Labirint Game";
            this.Paint += GameForm_Paint;
            this.KeyDown += GameForm_KeyDown;
        }
        private void GameForm_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int i = 0; i < array2D.GetLength(0); i++)
            {
                for (int j = 0; j < array2D.GetLength(1); j++)
                {
                    Brush brush = array2D[i, j] == 1 ? Brushes.Black : Brushes.White;
                    g.FillRectangle(brush, j * CellSize, i * CellSize, CellSize, CellSize);
                }
            }
            g.FillRectangle(Brushes.Red, playerX * CellSize, playerY * CellSize, CellSize, CellSize);
        }
        private void GameForm_KeyDown(object? sender, KeyEventArgs e)
        {
            int newX = playerX;
            int newY = playerY;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    {
                        newY = Math.Max(0, playerY - 1);
                        break;
                    }
                case Keys.Down:
                    {
                        newY = Math.Min(MazeSize - 1, playerY + 1);
                        break;
                    }
                case Keys.Left:
                    {
                        newX = Math.Max(0, playerX - 1);
                        break;
                    }
                case Keys.Right:
                    {
                        newX = Math.Min(MazeSize - 1, playerX + 1);
                        break;
                    }
            }

            if (array2D[newY, newX] == 0)
            {
                playerX = newX;
                playerY = newY;
                Invalidate();
            }
            if (playerX == array2D.GetLength(1) - 1 || playerY == array2D.GetLength(0) - 1)
            {
                MessageBox.Show("Лабиринт пройден!");
                this.Close();
            }
        }
    }
}