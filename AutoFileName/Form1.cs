using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace AutoFileName
{
    public partial class Form1 : Form
    {
        public class FileNameInfo
        {
            public string id { get; set; }
            public string objectid { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (var file = new StreamReader("example.txt"))
            {
                var line = file.ReadLine();
                while (line != null)
                {
                    if (line.Split(' ').Count() > 1)
                    {
                        var newFile = new FileNameInfo()
                        {
                            id = line.Split(' ')[0],
                            objectid = line.Split(' ')[1]
                        };
                        var files = Directory.GetFiles("video").Where(t=>Path.GetFileName(t).StartsWith(newFile.id)).ToList();
                        if (files.Count() == 1)
                        {
                            new FileInfo(files[0]).MoveTo($"video_out/{newFile.objectid}.mp4");
                        }
                    };

                }
                line = file.ReadLine();
            }
        }
    }
}
