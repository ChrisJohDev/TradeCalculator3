using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Data4Mat
{
	namespace TradeCalculator
	{
		public partial class NewCurrencyPairForm : Form
		{
			private cr d;
			private bool ok = false;
			private CurrencyDataProvider curr = new CurrencyDataProvider();
			private int currIndx;
			private string dataFile = global::Data4Mat.TradeCalculator.Properties.Resources.CurrenciesDataFile;

			/// <summary>
			/// Constructor NewCurrencyPairForm
			/// </summary>
			public NewCurrencyPairForm()
			{
				InitializeComponent();
				Initialize();
			}

			private void Initialize()
			{
				d = new cr();
				currIndx = curr.Currencies.Length;
				this.box_indx.Text = currIndx.ToString();
				if (currIndx > 1)
				{
					LoadSelectCurr();
				}
				else
				{
					this.input_selectCurrPair.Items.Add("New");
				}
				this.input_selectCurrPair.SelectedIndex = 0;
			}

			private void LoadSelectCurr()
			{
				this.input_selectCurrPair.Items.Add("New");
				for (int i = 1; i < curr.Currencies.Length; i++)
				{
					this.input_selectCurrPair.Items.Add(curr.Currencies.GetCurrency(i).CurrencyPair);
				}
			}

			private void OK_btn_Click(object sender, EventArgs e)
			{
				LoadData();
				ok = true;
				this.Close();
			}

			private void LoadData()
			{
				if (this.box_pair.Text != "")
				{
					d.CurrencyPair = this.box_pair.Text;
				}
				else
				{
					Error("Enter value in Currency Pair box!");
					this.box_pair.Focus();
				}
				if (this.box_quote.Text != "")
				{
					d.QuoteCurrency = this.box_quote.Text;
				}
				else
				{
					Error("Enter quote currency!");
					this.box_quote.Focus();
				}
				if (this.box_size.Text != "")
				{
					try
					{
						d.LotSize = int.Parse(this.box_size.Text);
					}
					catch (Exception e)
					{
						Error(e.Message + "\r\nEnter an integer value for Lot size!");
						this.box_size.Focus();
					}
				}
				else
				{
					Error("Enter Lot Size!");
					this.box_size.Focus();
				}

				if (this.box_pipvalue.Text != "")
				{
					try
					{
						d.TickValue = int.Parse(this.box_pipvalue.Text);
					}
					catch (Exception e)
					{
						Error(e.Message + "\r\nEnter an integer value for PIP value!");
						this.box_size.Focus();
					}
				}
				else
				{
					Error("Enter PIP value!");
					this.box_size.Focus();
				}

				if (this.box_spread.Text != "")
				{
					try
					{
						d.Spread = int.Parse(this.box_spread.Text);
					}
					catch (Exception e)
					{
						Error(e.Message + "\r\nEnter an integer value for Spread!");
						this.box_spread.Focus();
					}
				}
				else
				{
					Error("Enter Spread value!");
					this.box_spread.Focus();
				}

				if (this.box_decimals.Text != "")
				{
					try
					{
						d.Decimals = int.Parse(this.box_decimals.Text);
					}
					catch (Exception e)
					{
						Error(e.Message + "\r\nEnter an integer value for Decimals!");
						this.box_decimals.Focus();
					}
				}
				else
				{
					Error("Enter Decimals value!");
					this.box_decimals.Focus();
				}

				if (this.box_prefix.Text != "")
				{
					d.CurrencyPrefix = this.box_prefix.Text;
				}
				else
				{
					Error("Enter the quote currency's prefix!");
					this.box_prefix.Focus();
				}

				this.curr.Currencies.Add(d);
				WriteData(d);

			}

			private void WriteData(cr c)
			{
				using (StreamWriter sw = File.AppendText(dataFile))
				{
					string x = this.currIndx.ToString() + "," +
								c.CurrencyPair + "," +
								c.QuoteCurrency + "," +
								c.LotSize.ToString() + "," +
								c.TickValue.ToString() + "," +
								c.Spread.ToString() + "," +
								c.Decimals.ToString() + "," +
								c.CurrencyPrefix
								;
					//sw.WriteLine("");
					sw.WriteLine(x);
				}
			}

			private void Cancel_btn_Click(object sender, EventArgs e)
			{
				this.Close();
			}

			private void Error(string message)
			{
				MessageBox.Show(message, "Error in New Currency Pair!");
			}

			/// <summary>
			/// 
			/// </summary>
			public cr Inputs
			{
				get
				{
					if (this.ok) return d;
					else return null;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public bool Canceled
			{
				get { return !this.ok; }
			}
		}
	}
}
