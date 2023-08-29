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

        private void openResourceButton_Click(object sender, EventArgs e)
        {

        }
    }
}
