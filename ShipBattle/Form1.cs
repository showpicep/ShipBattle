using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        private List<Ship> ships;

        public int[,] myMap = new int[mapSize, mapSize];
        public int[,] enemyMap = new int[mapSize, mapSize];

        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public static bool isPlaying = false;

        public CheckedListBox checkedListBox1 = new CheckedListBox();
        public CheckedListBox checkedListBox2 = new CheckedListBox();
        Dictionary<int, int> countShips = new Dictionary<int, int>();

        private Bot bot;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Морской бой";
            Init();
        }

        public void Init()
        {
            ships = new List<Ship>();

            isPlaying = false;
            CreateMaps();
            bot = new Bot(ref enemyMap, ref myMap, enemyButtons, myButtons);
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
                        button.Click += new EventHandler(PlayerShoot);// CHANGE!
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

            countShips.Add(4, 1);
            countShips.Add(3, 2);
            countShips.Add(2, 3);
            countShips.Add(1, 4);
        }

        /// <summary>
        /// Можно ли начать игру 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Start(object sender, EventArgs e)
        {
            int sum = 0;
            foreach (int sumOfShips in countShips.Values)
            {
                sum += sumOfShips;
            }
            if (sum==0)
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
        /// Проверка на возможность поставить корабль в окресности корабля или на сам корабль
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns> 
        public bool check(List<Point> l)
        {
            bool isChecked = true;

            foreach (Point p in l)
            {
                isChecked = isChecked && !(myMap[p.Y, p.X] == 1 || myMap[p.Y, p.X] == -1);
            }
            return isChecked;
        }

        /// <summary>
        /// Расстановка кораблей 
        /// </summary>
        /// <param name="l"></param>
        public void addShip(List<Point> l)
        {
            Ship ship = new Ship();

            foreach (Point p in l)
            {
                MyPoint newPoint = new MyPoint(p);
                ship.addPoint(newPoint);

                myMap[p.Y, p.X] = 1;
                myButtons[p.Y, p.X].BackColor = Color.Red;
            }

            ships.Add(ship);

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (myMap[i, j] == 1)
                    {
                        fillFields(i, j);
                    }
                }
            }
        }

        /// <summary>
        /// Заполнение окрестности корабля в радиусе 1
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public void fillFields(int i, int j)
        {

            for (int k = i - 1; k <= i + 1; k++)
            {
                for (int c = j - 1; c <= j + 1; c++)
                    if (!cantFill(k, c))
                    {
                        myMap[k, c] = -1;
                        if (k!=0 && c!= 0 && k!=12 && c!=12)
                        {
                            myButtons[k, c].BackColor = Color.Aquamarine;
                        }
                    }
            }
        }

        /// <summary>
        /// Проверка для заполнения окрестности вокруг корабля
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public bool cantFill(int i, int j)
        {
            return !(i < mapSize && j < mapSize && myMap[i, j] != 1);
        }

        private void readFields(int positionInCheckedListBoxOne, Button pressedButton, bool isHorizontal)
        {
            bool flag;

            if (isHorizontal)
            {
                flag = pressedButton.Location.X / cellSize + positionInCheckedListBoxOne <= mapSize;
            }
            else
            {
                flag = pressedButton.Location.Y / cellSize + positionInCheckedListBoxOne <= mapSize;
            }

            if (countShips[positionInCheckedListBoxOne] > 0)
            {
                if (flag)
                {
                    List<Point> l = new List<Point>();
                    int k = 0;
                    int kx = 0;
                    int ky = 0;
                    while (k < positionInCheckedListBoxOne)
                    {
                        int x = pressedButton.Location.X / cellSize + kx; // Горизонталь
                        int y = pressedButton.Location.Y / cellSize + ky; // Вертикаль
                        l.Add(new Point(x, y));

                        if (isHorizontal)
                        {
                            kx++;
                        }
                        else
                        {
                            ky++;
                        }
                        k++;
                    }

                    if (check(l))
                    {
                        addShip(l);
                        countShips[positionInCheckedListBoxOne]--;
                    }
                    else
                    {
                        showError_1();
                    }
                }
                else
                {
                    showError_2();
                }
            }
            else
            {
                MessageBox.Show("Корабли с количеством палуб " + positionInCheckedListBoxOne + " закончились");
            }
        }
        public void ConfigureShips(object sender, EventArgs e)
        {
            int positionInCheckedListBoxOne = Convert.ToInt32(checkedListBox1.SelectedItem);
            string positionInCheckedListBoxTwo = Convert.ToString(checkedListBox2.SelectedItem);
            Button pressedButton = sender as Button;

            if (positionInCheckedListBoxOne == 0 || positionInCheckedListBoxTwo == "")
            {
                if (positionInCheckedListBoxOne == 0 && positionInCheckedListBoxTwo == "")
                {
                    showError_5();
                }

                else if (positionInCheckedListBoxOne == 0)
                {
                    showError_3();
                }

                else if (positionInCheckedListBoxTwo == "")
                {
                    showError_4();
                }
            }
            else
            {
                readFields(positionInCheckedListBoxOne, pressedButton, positionInCheckedListBoxTwo == "Горизонтально");
            }

        }

        public void PlayerShoot(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;
            bool playerTurn = Shoot(pressedButton);

            if (!playerTurn)
            {
                bot.Shoot();
            }

        }

        public bool Shoot(Button pressedButton)
        {
            bool hit = false;
            if (isPlaying)
            {
                int delta = 0;
                if (pressedButton.Location.X > 350)
                    delta = 350;

                int Y = pressedButton.Location.Y / cellSize; // индекс по у 
                int X = (pressedButton.Location.X - delta) / cellSize;

                if (enemyMap[Y, X] == 1)
                {
                    bot.getHit(Y, X);
                    pressedButton.BackColor = Color.Red; // попадание
                    pressedButton.Text = "X";
                    enemyButtons[Y, X].Enabled = false; //чтобы нельзя было повторно нажать на кнопку
                    enemyMap[Y, X] = 2;
                    hit = true;
                }
                if (enemyMap[Y, X] != 1 && enemyMap[Y, X] != -2 && enemyMap[Y, X] != 2) // если попал не в корабли И не на уже нажатое место и не на подбитую часть корабля
                {
                    pressedButton.BackColor = Color.Black; // miss
                    enemyMap[Y, X] = -2;
                    enemyButtons[Y, X].Enabled = false; // чтобы нельзя было повторно нажать на кнопку 
                    hit = false;
                }
            }
            else
                MessageBox.Show("Нужно начать игру");

            if (hit)
            {
                bool flag = bot.checkShips(); // true == осталось по-прежнему
                MessageBox.Show(flag.ToString());
                if (!flag)
                {
                    Render();
                    // вызвать метод, проверяющий конец игры
                }
            }

            return hit;
        }

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

        private void showError_1()
        {
            MessageBox.Show("ЧИТАЙ ПРАВИЛААА1");
        }

        private void showError_2()
        {
            MessageBox.Show("ЧИТАЙ ПРАВИЛААА2");
        }

        private void showError_3()
        {
            MessageBox.Show("Выберите количество палуб у корабля");
        }

        private void showError_4()
        {
            MessageBox.Show("Выберите положение коробля");
        }

        private void showError_5()
        {
            MessageBox.Show("Вы не выбрали корабль");
        }

        
    }
}