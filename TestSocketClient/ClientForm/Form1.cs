using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ClientForm
{
    public partial class Form1 : Form
    {
        string fileContent = string.Empty;
        string filePath = string.Empty;
        Stream fileStream = null;
        Socket client;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Connect_Click(object sender, EventArgs e)
        {
            var ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(ipep);

            MessageBox.Show("Connected");
        }

        private void Open_Click(object sender, EventArgs e)
        {
            fileContent = string.Empty;
            filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = ".\\";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    fileStream = openFileDialog.OpenFile();
                }
            }

            pictureBox1.Image = Bitmap.FromFile(filePath);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void Send_Click(object sender, EventArgs e)
        {
            //this.Send.Enabled = false;

            Byte[] _data = ImageToByteArray(pictureBox1.Image);
            client.Send(BitConverter.GetBytes(_data.Length));
            client.Send(_data);
        }
        public byte[] ImageToByteArray(System.Drawing.Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
    }
}
