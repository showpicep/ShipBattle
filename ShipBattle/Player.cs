using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShipBattle
{
    class Player
    {
        private List<Ship> ships = new List<Ship>();

        private int countOfAlives;

        public int[,] enemyMap = new int[Form1.mapSize, Form1.mapSize];// карта бота 
        public int[,] myMap = new int[Form1.mapSize, Form1.mapSize];// карта игрока 
        private Random rdn = new Random();
        public Button[,] myButtons = new Button[Form1.mapSize, Form1.mapSize];
        public Button[,] enemyButtons = new Button[Form1.mapSize, Form1.mapSize];
        //public CheckedListBox checkedListBox1 = new CheckedListBox();
        //public CheckedListBox checkedListBox2 = new CheckedListBox();

        public Dictionary<int, int> countShips = new Dictionary<int, int>();// размер, количество

        public Player(ref int[,] myMap, ref int[,] enemyMap,ref Button[,] myButtons,ref Button[,] enemyButtons)
        {
            countOfAlives = 10;
            this.myMap = myMap;
            this.enemyMap = enemyMap;
            this.enemyButtons = enemyButtons;
            this.myButtons = myButtons;

            //this.checkedListBox1 = checkedListBox1;
            //this.checkedListBox2 = checkedListBox2;

            countShips.Add(4, 1);
            countShips.Add(3, 2);
            countShips.Add(2, 3);
            countShips.Add(1, 4);
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

            for (int i = 0; i < Form1.mapSize; i++)
            {
                for (int j = 0; j < Form1.mapSize; j++)
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
                        if (k != 0 && c != 0 && k != 12 && c != 12)
                        {
                            myButtons[k, c].BackColor = Color.AntiqueWhite;
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
            return !(i < Form1.mapSize && j < Form1.mapSize && myMap[i, j] != 1);
        }

        /// <summary>
        /// Вся реализация расстоновки корабля тут 
        /// </summary>
        /// <param name="positionInCheckedListBoxOne"></param>
        /// <param name="pressedButton"></param>
        /// <param name="isHorizontal"></param>
        public void readFields(int positionInCheckedListBoxOne, Button pressedButton, bool isHorizontal)
        {
            bool flag;

            if (isHorizontal)
            {
                flag = pressedButton.Location.X / Form1.cellSize + positionInCheckedListBoxOne <= Form1.mapSize;
            }
            else
            {
                flag = pressedButton.Location.Y / Form1.cellSize + positionInCheckedListBoxOne <= Form1.mapSize;
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
                        int x = pressedButton.Location.X / Form1.cellSize + kx; // Горизонталь
                        int y = pressedButton.Location.Y / Form1.cellSize + ky; // Вертикаль
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
                        Errors.showError_1();
                    }
                }
                else
                {
                    Errors.showError_2();
                }
            }
            else
            {
                MessageBox.Show("Корабли с количеством палуб " + positionInCheckedListBoxOne + " закончились");
            }
        }

        
        /// <summary>
        /// Полуучаем кол-во живых кораблей 
        /// </summary>
        /// <returns></returns>
        public int GetIsAlive()
        {
            int isAliveNow = 0;

            foreach (Ship s in ships)
            {
                s.checkLife();
                if (s.getIsAlive())
                {
                    isAliveNow++;
                }
            }

            return isAliveNow;
        }

        public List<List<MyPoint>> GetDeads()
        {
            List<List<MyPoint>> list = new List<List<MyPoint>>();

            foreach (Ship s in ships)
            {
                if (!s.getIsAlive())
                {
                    list.Add(s.getPoints());
                }
            }

            return list;
        }

        /// <summary>
        /// Проверка на жизнь корабля
        /// </summary>
        /// <returns></returns>
        public bool checkShips()
        {
            int isAliveNow = 0;

            foreach (Ship s in ships)
            {
                s.checkLife();
                if (s.getIsAlive())
                {
                    isAliveNow++;
                }
            }
            MessageBox.Show(isAliveNow.ToString());// Выводит кол-во живых кораблей
            bool flag = isAliveNow == countOfAlives;
            countOfAlives = isAliveNow;

            return flag;
        }

        public void getHit(int y, int x)
        {
            foreach (Ship s in ships)
            {
                s.getHit(y, x);
            }

        }

        public List<Ship> getShips()
        {
            return ships;
        }

        //public void Render()
        //{
        //    List<List<MyPoint>> list = GetDeads();

        //    foreach (List<MyPoint> l in list)
        //    {
        //        MyPoint first = l[0];
        //        MyPoint last = l[l.Count - 1];

        //        first = new MyPoint(new Point(first.point.X - 1, first.point.Y - 1));
        //        last = new MyPoint(new Point(last.point.X + 1, last.point.Y + 1));

        //        for (int i = first.point.Y; i <= last.point.Y; i++)
        //        {
        //            for (int j = first.point.X; j <= last.point.X; j++)
        //            {

        //                if (i > 0 && j > 0 && i < 11 && j < 11 && myMap[i, j] != 2)
        //                {
        //                    myButtons[i, j].BackColor = Color.Aquamarine;
        //                    myButtons[i, j].Enabled = false;
        //                }

        //            }
        //        }

        //    }
        //}

    }
}
