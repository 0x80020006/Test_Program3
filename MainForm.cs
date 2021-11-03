using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Test_Program3
{
    class MainForm : Form
    {
        public int fNumber;
        public string fPath;
        MenuStrip menuStrip;
        OpenFileDialog ofd;
        List<string> filesList;
        PictureBox[] pbList;

        static readonly int MS_WIDTH = 500;
        static readonly int MS_HEIGHT = 470;
        static readonly int CS_WIDTH = 800;
        static readonly int CS_HEIGHT = 770;

        public MainForm()
        {
            Load += new EventHandler(MainForm_Load);
            menuStrip = new MenuStrip();
            Controls.Add(menuStrip);            
            MinimumSize = new Size(MS_WIDTH, MS_HEIGHT);
            Size = new Size(CS_WIDTH, CS_HEIGHT);
            DoubleBuffered = true;
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            ToolStripMenuItem menuFile = new ToolStripMenuItem();
            menuFile.Text = "ファイル(&F)";
            menuStrip.Items.Add(menuFile);

            ToolStripMenuItem menuFileOpen = new ToolStripMenuItem();

            menuFileOpen.Text = "開く(&O)";
            menuFileOpen.Click += new EventHandler(Open_Click);
            menuFile.DropDownItems.Add(menuFileOpen);
            ToolStripMenuItem menuFileEnd = new ToolStripMenuItem();

            menuFileEnd.Text = "終了(&X)";
            menuFileEnd.Click += new EventHandler(Close_Click);
            menuFile.DropDownItems.Add(menuFileEnd);
        }

        void Open_Click(object sender, EventArgs e)
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "Image File(*.bmp,*.jpg,*.png)|*.bmp;*.jpg;*.png|Bitmap(*.bmp)|*.bmp|Jpeg(*.jpg)|*.jpg|PNG(*.png)|*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine($"読み込みファイル:{ofd.FileName}");
                string folderPath = Path.GetDirectoryName(ofd.FileName);
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath).Where(str => str.EndsWith(".bmp") || str.EndsWith(".jpg") || str.EndsWith(".png"));
                filesList = files.ToList();
                var sortQuery = filesList.OrderBy(s => s.Length);
                filesList = sortQuery.ToList();
                //fNumber = filesList.IndexOf(ofd.FileName);
                Console.WriteLine($"{filesList.Count}");
                fNumber = filesList.IndexOf(ofd.FileName);

                pbList = new PictureBox[filesList.Count];
                for(int i = 0; i < pbList.Length; i++)
                {
                    fPath = filesList[i];
                    Console.WriteLine($"{fPath}");
                                        
                    pbList[i] = new PictureBox();
                    pbList[i].BackColor = Color.Blue;
                    pbList[i].Location = new Point(ClientSize.Width / 2 - pbList[i].Width / 2 + (pbList[i].Width + 10) * (i - fNumber), ClientSize.Height / 2 - pbList[i].Height / 2);
                    Controls.Add(pbList[i]);
                    Bitmap b = new Bitmap(pbList[i].Width, pbList[i].Height);
                    Graphics g = Graphics.FromImage(b);
                    Image img = Image.FromFile(fPath);

                    float rWidth = Width / img.Width;
                    float rHeight = Height / img.Height;
                    float r = Math.Min(rWidth, rHeight);

                    g.DrawImage(img, 0, 0);
                    img.Dispose();
                    g.Dispose();
                    pbList[i].Image = b;
                }
            }
        }

        void Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
