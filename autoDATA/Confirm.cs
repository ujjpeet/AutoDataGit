﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace autoDATA
{
    public partial class Confirm : Form
    {
        public Confirm(string question)
        {
            InitializeComponent();
            lbConfirmQuestion.Text = question;
        }
    }
}
