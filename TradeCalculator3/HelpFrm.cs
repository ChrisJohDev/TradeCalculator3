using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Data4Mat
{
	namespace TradeCalculator
	{
		/// <summary>
		/// 
		/// </summary>
		public partial class HelpFrm : Form
		{
			/// <summary>
			/// Constructor Helpfrm
			/// </summary>
			public HelpFrm()
			{
				InitializeComponent();
			}

			private void exitToolStripMenuItem_Click(object sender, EventArgs e)
			{
				this.Close();
			}

			private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
			{
				bool x = this.TopMost;
				this.TopMost = false;
				AboutBox1 box = new AboutBox1();
				box.ShowDialog(this);
				this.TopMost = x;
			}

			private void ascendingTriangleToolStripMenuItem_Click(object sender, EventArgs e)
			{

			}
		}
	}
}