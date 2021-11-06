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
        public int fNumberP;
        public int fNumberM;
        public string fPath;
        MenuStrip menuStrip;
        OpenFileDialog ofd;
        List<string> filesList;
        PictureBox[] pbList;
        bool LoadFlag;

        static readonly int MS_WIDTH  = 500;
        static readonly int MS_HEIGHT = 470;
        static readonly int CS_WIDTH  = 800;
        static readonly int CS_HEIGHT = 770;
        static readonly int CM_WIDTH  = 1920;
        static readonly int CM_HEIGHT = 1080;
        static readonly int IMG_BETWEEN = 10;
        static readonly int BAR_HEIGHT = SystemInformation.CaptionHeight + SystemInformation.MenuHeight;
        static readonly int FILE_RANGE = 5;

        public MainForm()
        {
            Load += new EventHandler(MainForm_Load);
            menuStrip = new MenuStrip();
            Controls.Add(menuStrip);
            MinimumSize = new Size(MS_WIDTH, MS_HEIGHT);
            Size = new Size(CS_WIDTH, CS_HEIGHT);
            MouseWheel += new MouseEventHandler(MouseWheelControl);
            DoubleBuffered = true;
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            SizeChanged += Window_SizeChanged;

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
            //コントロールが存在している場合にコントロールを削除する処理を追加すること

            ofd = new OpenFileDialog();
            ofd.Filter = "Image File(*.bmp,*.jpg,*.png)|*.bmp;*.jpg;*.png|Bitmap(*.bmp)|*.bmp|Jpeg(*.jpg)|*.jpg|PNG(*.png)|*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (LoadFlag == true)
                {
                    //PictureBoxをRemoveする処理を入れる
                    for(int i = 0; i < filesList.Count - 1; i++)
                    {
                        if (this.Contains(pbList[i]) == true)
                        {
                            Controls.Remove(pbList[i]);
                        }
                    }
                    filesList.Clear();
                }
                Console.WriteLine($"読み込みファイル:{ofd.FileName}");
                string folderPath = Path.GetDirectoryName(ofd.FileName);
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath).Where(str => str.EndsWith(".bmp") || str.EndsWith(".jpg") || str.EndsWith(".png"));
                filesList = files.ToList();
                var sortQuery = filesList.OrderBy(s => s.Length);
                filesList = sortQuery.ToList();
                Console.WriteLine($"{filesList.Count}");
                fNumber = filesList.IndexOf(ofd.FileName);
                FileRange_Math();
                pbList = new PictureBox[filesList.Count];
                Img_Load();
                LoadFlag = true;

                
            }
        }

        void Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void MouseWheelControl(object sender, MouseEventArgs e)
        {
            if (LoadFlag != true)
            {

            }
            else
            {

                if (e.Delta > 0 && fNumber > 0)
                {
                    fNumber -= 1;
                    FileRange_Math();
                }
                else if (e.Delta < 0 && fNumber < filesList.Count - 1)
                {
                    fNumber += 1;
                    FileRange_Math();
                }

                Img_Load();
                PBList_Location();

            }

        }

        private void Window_SizeChanged(object sender, EventArgs e)
        {
            if (LoadFlag != true)
            {

            }
            else
            {
                PBList_Location();
            }
            Console.WriteLine($"{Width},{Height}");
        }

        void FileRange_Math()
        {
            fNumberP = fNumber + FILE_RANGE;
            fNumberM = fNumber - FILE_RANGE;
            if (fNumberM < 0)
            {
                fNumberP -= fNumberM;
            }
            else if (fNumberP > filesList.Count - 1)
            {
                fNumberM -= fNumberP - filesList.Count - 1;
            }
            Console.WriteLine($"{fNumberM}<{fNumber}<{fNumberP}");
        }

        void PBList_Location()
        {
            for (int i = 0; i < pbList.Length; i++)
            {
                if (pbList[i] != null)
                {
                    pbList[i].Location = new Point(ClientSize.Width / 2 - pbList[i].Width / 2 + (pbList[i].Width + IMG_BETWEEN) * (i - fNumber), SystemInformation.MenuHeight + (ClientSize.Height - SystemInformation.MenuHeight) / 2 - pbList[i].Height / 2);
                    Invalidate();
                }
            }
        }

        void Img_Load()
        {
            for (int i = fNumberM; i < fNumberP; i++)
            {
                if (i >= 0 && i < filesList.Count)
                {

                    if (this.Contains(pbList[i]) == false)
                    {

                        fPath = filesList[i];
                        Console.WriteLine($"{fPath}");

                        pbList[i] = new PictureBox();
                        pbList[i].BackColor = Color.Blue;
                        Controls.Add(pbList[i]);
                        Image img = Image.FromFile(fPath);

                        float iW = img.Width;
                        float iH = img.Height;
                        Console.WriteLine($"{iW},{iH}");
                        float cW = Width;
                        float cH = Height - BAR_HEIGHT;
                        Console.WriteLine($"{cW},{cH}");


                        float rW = cW / iW;
                        float rH = cH / iH;
                        float r = Math.Min(rW, rH);
                        Console.WriteLine($"{r}");
                        int mW = (int)(img.Width * r);
                        int mH = (int)(img.Height * r);

                        pbList[i].Width = mW;
                        pbList[i].Height = mH;
                        pbList[i].Location = new Point(ClientSize.Width / 2 - pbList[i].Width / 2 + (pbList[i].Width + IMG_BETWEEN) * (i - fNumber), SystemInformation.MenuHeight + (ClientSize.Height - SystemInformation.MenuHeight) / 2 - pbList[i].Height / 2);

                        Bitmap b = new Bitmap(mW, mH);
                        Graphics g = Graphics.FromImage(b);

                        g.DrawImage(img, 0, 0, mW, mH);

                        img.Dispose();
                        g.Dispose();

                        pbList[i].Image = b;
                    }
                }
            }
        }
    }
}
