using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mmdo_coursework
{
    public partial class SystemSolutionForm : Form
    {
        private MainScreen _originalMainScreen;
        public SystemSolutionForm(MainScreen mainScreen)
        {
            InitializeComponent();
            _originalMainScreen = mainScreen;
        }

        private void back_Button_Click(object sender, EventArgs e)
        {
            _originalMainScreen.Show();
            this.Close();
        }

        private void SystemSolutionForm_Load(object sender, EventArgs e)
        {

        }
    }
}
