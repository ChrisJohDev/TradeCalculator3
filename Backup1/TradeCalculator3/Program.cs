using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Data4Mat
{
	namespace TradeCalculator
	{
		static class Program
		{
			/// <summary>
			/// The main entry point for the application.
			/// </summary>
			[STAThread]
			static void Main()
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new trdcalc());
			}
		}
	}
}