using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LC_DataGenerator
{
    public partial class MainForm : Form
    {
        List<String> randomStringData = new List<string> { "的","一","是","不","了","在","人","有","我","他","这","个","们","中","来","上","大","为","和","国","地","到","以","说","时","要","就","出","会","可","也","你","对","生","能","而","子","那","得","于","着","下","自","之","年","过","发","后","作","里","用","道","行","所","然","家","种","事","成","方","多","经","么","去","法","学","如","都","同","现","当","没","动","面","起","看","定","天","分","还","进","好","小","部","其","些","主","样","理","心","她","本","前","开","但","因","只","从","想","实" };

        List<String> randomStringEnData = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        Random random = new Random();

        public MainForm()
        {
            InitializeComponent();
        }

        private void randomStringListButton_Click(object sender, EventArgs e)
        {
            this.outRichTextBox.Text = "";
            for (var i = 0; i < 500; i++)
            {
                var randomData = "";
                var randomDataCount = random.Next(0, 4);
                for (int j = 0; j < randomDataCount; j++)
                {
                    //randomData += randomStringData[random.Next(0, randomStringData.Count)] + " ";
                    var charCount = random.Next(2, 5);
                    for(var k = 0; k < charCount; k++)
                    {
                        randomData += randomStringData[random.Next(0, randomStringData.Count)];
                    }
                    randomData += " ";
                }
                this.outRichTextBox.AppendText(randomData.Trim() + "\r\n");
            }
        }

        private void randomYMDButton_Click(object sender, EventArgs e)
        {
            this.outRichTextBox.Text = "";
            for (var i = 0; i < 500; i++)
            {
                this.outRichTextBox.AppendText(
                    (new DateTime(
                        random.Next(1999, 2024),
                        random.Next(1, 13),
                        random.Next(1, 28),
                        random.Next(1, 24),
                        random.Next(1, 60),
                        random.Next(1, 60)).ToUniversalTime().ToString("yyyy年MM月dd日")
                    + "\r\n"));
            }
        }

        private void randomSentenceButton_Click(object sender, EventArgs e)
        {
            this.outRichTextBox.Text = "";
            for (var i = 0; i < 500; i++)
            {
                var randomData = "";
                var randomDataCount = random.Next(10, 50);
                for (int j = 0; j < randomDataCount; j++)
                {
                    randomData += randomStringData[random.Next(0, randomStringData.Count)];
                }
                this.outRichTextBox.AppendText(randomData.Replace(" ", "") + "。\r\n");
            }
        }

        private void randomENFilenameButton_Click(object sender, EventArgs e)
        {
            this.outRichTextBox.Text = "";
            for (var i = 0; i < 500; i++)
            {
                var randomData = "";
                var randomDataCount = random.Next(10, 50);
                for (int j = 0; j < randomDataCount; j++)
                {
                    randomData += randomStringEnData[random.Next(0, randomStringEnData.Count)];
                }
                this.outRichTextBox.AppendText(randomData.Replace(" ", "") + "\r\n");
            }
        }

        private void randomZHFilenameButton_Click(object sender, EventArgs e)
        {
            this.outRichTextBox.Text = "";
            for (var i = 0; i < 500; i++)
            {
                var randomData = "";
                var randomDataCount = random.Next(10, 50);
                for (int j = 0; j < randomDataCount; j++)
                {
                    randomData += randomStringData[random.Next(0, randomStringData.Count)];
                }
                this.outRichTextBox.AppendText(randomData.Replace(" ", "") + "\r\n");
            }
        }

        private void randomDoubleButton_Click(object sender, EventArgs e)
        {
            this.outRichTextBox.Text = "";
            for (var i = 0; i < 500; i++)
            {
                this.outRichTextBox.AppendText($"{(random.NextDouble() + 0.01) * 1024}\r\n");
            }
        }

        private void randomIntButton_Click(object sender, EventArgs e)
        {
            this.outRichTextBox.Text = "";
            for (var i = 0; i < 500; i++)
            {
                this.outRichTextBox.AppendText($"{random.Next(1000)}\r\n");
            }
        }

        private void randomFileSizeButton_Click(object sender, EventArgs e)
        {
            this.outRichTextBox.Text = "";
            var unit = new List<string>() { "KB", "MB", "GB" };
            for (var i = 0; i < 500; i++)
            {
                this.outRichTextBox.AppendText(
                    ((random.NextDouble() + 0.01) * 1024).ToString("0.0") + 
                    unit[random.Next(0,3)] + 
                    "\r\n");
            }
        }

        private void randomHMSButton_Click(object sender, EventArgs e)
        {
            this.outRichTextBox.Text = "";
            for (var i = 0; i < 500; i++)
            {
                this.outRichTextBox.AppendText($"{random.Next(90)}:{random.Next(60)}:{random.Next(60)}\r\n");
            }
        }
    }
}
