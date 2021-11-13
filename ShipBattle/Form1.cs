﻿using System;
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
        ErorrsForDebug erorrs = new ErorrsForDebug();

        public const int mapSize = 11;
        static int cellSize = 30;
        static string alphabet = "АБВГДЕЖЗИК";

        //CreateMap createMap = new CreateMap(mapSize, cellSize, alphabet);

        public int[,] myMap = new int[mapSize, mapSize];    
        public int[,] enemyMap = new int[mapSize, mapSize];

        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public bool isPlaying = false;

        public CheckedListBox checkedListBox1 = new CheckedListBox();
        public CheckedListBox checkedListBox2 = new CheckedListBox();
        Dictionary<int, int> countShips = new Dictionary<int, int>();


        public Form1()
        {
            InitializeComponent();
            this.Text = "Морской бой";
            Init();
        }

        public void Init()
        {
            isPlaying = false;
            CreateMaps();
        }

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

        public void Start(object sender, EventArgs e)
        {
            isPlaying = true;
        }


        private bool check(List<Point> l)
        {
            bool isChecked = true;

            foreach (Point p in l)
            {
                isChecked = isChecked && !(myMap[p.Y, p.X] == 1 || myMap[p.Y, p.X] == -1);
            }

            return isChecked;
        }

        private void addShip(List<Point> l)
        {
            foreach (Point p in l)
            {
                myMap[p.Y, p.X] = 1;
                myButtons[p.Y, p.X].BackColor = Color.Red;
            }

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

        private void fillFields(int i, int j)
        {

            for (int k = i - 1; k <= i + 1; k++)
            {
                for (int c = j - 1; c <= j + 1; c++)
                    if (!cantFill(k, c))
                    {
                        myMap[k, c] = -1;
                        //myButtons[k, c].BackColor = Color.Blue;
                    }
            }
        }

        private bool cantFill(int i, int j)
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
                        erorrs.showErorr_1();
                    }
                }
                else
                {
                    erorrs.showErorr_2();
                }
            }
            else
            {
                MessageBox.Show("Выбери корабль");
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
                    erorrs.showErorr_5();
                }

                else if (positionInCheckedListBoxOne == 0)
                {
                    erorrs.showErorr_3();
                }

                else if (positionInCheckedListBoxTwo == "")
                {
                    erorrs.showErorr_4();
                }
            }
            else
            {
                readFields(positionInCheckedListBoxOne, pressedButton, positionInCheckedListBoxTwo == "Горизонтально");
            }

        }
    }
}