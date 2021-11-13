using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShipBattle
{
    class CreateMap
    {
        Form1 form1 = new Form1();

        int mapSize;
        int cellSize;
        string alphabet;

        int[,] myMap;
        int[,] enemyMap;

        Button[,] myButtons;
        Button[,] enemyButtons;

        public CreateMap(int mapSize, int cellSize, string alphabet)
        {
            this.mapSize = mapSize;
            this.cellSize = cellSize;
            this.alphabet = alphabet;
            int[,] myMap = new int[mapSize, mapSize];
            int[,] enemyMap = new int[mapSize, mapSize];

            Button[,] myButtons = new Button[mapSize, mapSize];
            Button[,] enemyButtons = new Button[mapSize, mapSize];
        }

        public object Controls { get; private set; }

        public void creatingMyMap()
        {
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
                        button.Click += new EventHandler(form1.ConfigureShips);
                    }
                    form1.Controls.Add(button);
                }
            }
        }
    }
}
