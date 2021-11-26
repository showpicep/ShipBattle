using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShipBattle
{
    public partial class Form1 : Form
    {

        public const int mapSize = 11;
        public const int cellSize = 30;
        public string alphabet = "АБВГДЕЖЗИК";

        //private List<Ship> ships;

        public int[,] myMap = new int[mapSize, mapSize];
        public int[,] enemyMap = new int[mapSize, mapSize];

        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public static bool isPlaying = false;

        public CheckedListBox checkedListBox1 = new CheckedListBox();
        public CheckedListBox checkedListBox2 = new CheckedListBox();
        public ComboBox chooseBot = new ComboBox();
        PictureBox temp = new PictureBox();

        Dictionary<int, int> countShips = new Dictionary<int, int>();
        AnimationForHit anim = new AnimationForHit();

        private Bot bot;
        private Player player;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Морской бой";
            Init();
        }

        public void Init()
        {
            //ships = new List<Ship>();

            isPlaying = false;
            CreateMaps();
            player = new Player(ref myMap, ref enemyMap, ref myButtons, ref enemyButtons);
            bot = new Bot(ref enemyMap, ref myMap, ref enemyButtons, ref myButtons, ref player);
            bot.ConfigureShips();
        }



        /// <summary>
        /// Создание карты
        /// </summary>
        public void CreateMaps()
        {
            this.Width = mapSize * 2 * cellSize + 100;
            this.Height = (mapSize + 3) * cellSize + 100;

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    myMap[i, j] = 0;

                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;

                    if (j == 0 || i == 0)
                    {
                        button.BackColor = Color.Gray;
                        if (i == 0 && j > 0)
                            button.Text = alphabet[j - 1].ToString();
                        if (j == 0 && i > 0)
                            button.Text = i.ToString();
                    }
                    else
                    {
                        myButtons[i, j] = button;
                        button.Click += new EventHandler(ConfigureShips);
                    }
                    this.Controls.Add(button);
                }
            }

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    enemyMap[i, j] = 0;

                    Button button = new Button();
                    button.Location = new Point(350 + j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;

                    if (j == 0 || i == 0)
                    {
                        button.BackColor = Color.Gray;
                        if (i == 0 && j > 0)
                            button.Text = alphabet[j - 1].ToString();
                        if (j == 0 && i > 0)
                            button.Text = i.ToString();
                    }
                    else
                    {
                        enemyButtons[i, j] = button;
                        button.Click += new EventHandler(PlayerShoot);
                    }
                    this.Controls.Add(button);
                }
            }

            Label map1 = new Label();
            map1.Text = "Карта игрока";
            map1.Location = new Point(mapSize * cellSize / 2, mapSize * cellSize + 10);
            this.Controls.Add(map1);

            Label map2 = new Label();
            map2.Text = "Карта противника";
            map2.Location = new Point(350 + mapSize * cellSize / 2, mapSize * cellSize + 10);
            this.Controls.Add(map2);

            Button startButton = new Button();
            startButton.Text = "Начать";
            startButton.Click += new EventHandler(Start);
            startButton.Location = new Point(0, mapSize * cellSize + 20);
            this.Controls.Add(startButton);

            string[] words1 = { "4", "3", "2", "1" };
            checkedListBox1.Location = new Point(100, mapSize * cellSize + 50);
            for (int i = 0; i < words1.Length; i++)
                checkedListBox1.Items.Add(words1[i]);
            checkedListBox1.SelectionMode = SelectionMode.One;
            this.Controls.Add(checkedListBox1);

            string[] words2 = { "Вертикально", "Горизонтально" };
            checkedListBox2.Location = new Point(250, mapSize * cellSize + 50);
            for (int i = 0; i < words2.Length; i++)
                checkedListBox2.Items.Add(words2[i]);
            checkedListBox2.SelectionMode = SelectionMode.One;
            this.Controls.Add(checkedListBox2);



            string[] words3 = { "Easy", "Hard" };
            chooseBot.Location = new Point(400, mapSize * cellSize + 50);
            for (int i = 0; i < words3.Length; i++)
                chooseBot.Items.Add(words3[i]);
            chooseBot.SelectedItem = "Easy";
            this.Controls.Add(chooseBot);

            
            temp.Location = new Point(400, mapSize * cellSize + 100);
            temp.Size = new System.Drawing.Size(30, 30);
            temp.BackColor = Color.Black;
            this.Controls.Add(temp);
        }

        /// <summary>
        /// Можно ли начать игру 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Start(object sender, EventArgs e)
        {
            int sum = 0;
            foreach (int sumOfShips in player.countShips.Values)
            {
                sum += sumOfShips;
            }
            if (sum == 0)
            {
                isPlaying = true;
            }
            else
            {
                MessageBox.Show("Вы не расставили все корабли");
                isPlaying = false;
            }
        }

        /// <summary>
        /// Вызываем метод расстоновки кораблей параллельно проверяя на некоторые ошибки 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ConfigureShips(object sender, EventArgs e)
        {
            int positionInCheckedListBoxOne = Convert.ToInt32(checkedListBox1.SelectedItem);
            string positionInCheckedListBoxTwo = Convert.ToString(checkedListBox2.SelectedItem);
            Button pressedButton = sender as Button;
            if (!isPlaying)
            {
                if (positionInCheckedListBoxOne == 0 || positionInCheckedListBoxTwo == "")
                {
                    if (positionInCheckedListBoxOne == 0 && positionInCheckedListBoxTwo == "")
                    {
                        Errors.showError_5();
                    }

                    else if (positionInCheckedListBoxOne == 0)
                    {
                        Errors.showError_3();
                    }

                    else if (positionInCheckedListBoxTwo == "")
                    {
                        Errors.showError_4();
                    }
                }
                else
                {
                    player.readFields(positionInCheckedListBoxOne, pressedButton, positionInCheckedListBoxTwo == "Горизонтально");
                }
            }

        }

        /// <summary>
        /// Здесь происходит смена очереди стрльбы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PlayerShoot(object sender, EventArgs e)
        {
            string chosenBot = Convert.ToString(chooseBot.SelectedItem);
            Button pressedButton = sender as Button;
            bool playerTurn = Shoot(pressedButton);

            if (GameContinion())
            {
                if (!playerTurn)
                {
                    if (chosenBot == "Easy")
                        bot.Shoot(false);
                    if (chosenBot == "Hard") 
                        bot.Shoot(true);
                }
            }


        }

        public bool GameContinion()
        {
            if (bot.GetIsAlive() == 0)
            {
                SoundForGame.PlayerWin();
                MessageBox.Show("ПОБЕДА");
                isPlaying = false;
            }
            if (player.GetIsAlive() == 0)
            {
                SoundForGame.BotWin();
                MessageBox.Show("Победа бота(");
                isPlaying = false;
            }
            return isPlaying;
        }

        private async void Print(Button pressedButton)
        {
            foreach (Image img in anim.frame.ToList())
            {
                pressedButton.Image = img;

                await Task.Delay(100);
            }
        }

        public bool Shoot(Button pressedButton)
        {
            
            bool hit = false;
            if (isPlaying)
            {
                anim.GettingImages();
                int delta = 0;
                if (pressedButton.Location.X > 350)
                    delta = 350;

                int Y = pressedButton.Location.Y / cellSize; // индекс по у 
                int X = (pressedButton.Location.X - delta) / cellSize;

                if (enemyMap[Y, X] == 1)
                {
                    bot.getHit(Y, X);
                    Print(enemyButtons[Y,X]);
                    //pressedButton.BackColor = Color.Red; // попадание
                    //pressedButton.Text = "X";
                    enemyButtons[Y, X].Enabled = false; //чтобы нельзя было повторно нажать на кнопку
                    enemyMap[Y, X] = 2;
                    hit = true;
                }
                if (enemyMap[Y, X] != 1 && enemyMap[Y, X] != -2 && enemyMap[Y, X] != 2) // если попал не в корабли И не на уже нажатое место и не на подбитую часть корабля
                {
                    pressedButton.BackColor = Color.Aquamarine; // miss
                    enemyMap[Y, X] = -2;
                    enemyButtons[Y, X].Enabled = false; // чтобы нельзя было повторно нажать на кнопку 
                    hit = false;
                }
            }
            else
                MessageBox.Show("Нужно начать игру");

            if (hit)
            {
                bool bflag = bot.checkShips(); // true == осталось по-прежнему

                //MessageBox.Show(player.GetIsAlive().ToString());
                if (!bflag)
                {
                    if (bot.GetIsAlive() == 9)
                    {
                        SoundForGame.FB();
                        Render();
                    }
                    else if (bot.GetIsAlive() < 9)

                    {
                        SoundForGame.LakadMatatag();
                        Render();
                    }
                }
            }
            if (!hit)
            {

                bool pFlag = player.checkShips();
                if (!pFlag)
                {
                    if (player.GetIsAlive() == 9)
                    {
                        SoundForGame.Vilat();
                        bot.Render();
                    }
                    else if (player.GetIsAlive() < 9)
                    {
                        bot.Render();
                    }
                }
            }
            return hit;
        }       


        /// <summary>
        /// Отрисовка окрестности убитых кораблей на мапе бота 
        /// </summary>
        private void Render()
        {
            List<List<MyPoint>> list = bot.GetDeads();

            foreach (List<MyPoint> l in list)
            {
                MyPoint first = l[0];
                MyPoint last = l[l.Count - 1];

                first = new MyPoint(new Point(first.point.X - 1, first.point.Y - 1));
                last = new MyPoint(new Point(last.point.X + 1, last.point.Y + 1));

                for (int i = first.point.Y; i <= last.point.Y; i++)
                {
                    for (int j = first.point.X; j <= last.point.X; j++)
                    {

                        if (i > 0 && j > 0 && i < 11 && j < 11 && enemyMap[i, j] != 2)
                        {
                            enemyButtons[i, j].BackColor = Color.Aquamarine;
                            enemyButtons[i, j].Enabled = false;
                        }
                        
                    }
                }

            }
        }




        private void Form1_Load(object sender, EventArgs e)
        {

        }
        
    }
}