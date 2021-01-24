using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Data4Mat
{
	namespace TradeCalculator
	{
		public partial class trdcalc : Form
		{
			private const string fileVersion = "1.0"; // ~.tc1
			private double dbl_enter, dbl_sl, dbl_exit1, dbl_exit2, dbl_exit3;
			private double dbl_height, dbl_input1, dbl_input2, dbl_input3, dbl_input4, dbl_equity;
			private double dbl_pipLoss, dbl_pipExit1, dbl_pipExit2, dbl_pipExit3, dbl_pipSubTot, dbl_pipTotal;
			private double dbl_amountLoss, dbl_amountExit1, dbl_amountExit2, dbl_amountExit3, dbl_amountSubTot, dbl_amountTotal;
			private double orderSize = 1.0;
			private bool b_stopLoss, b_exit1, b_riskRewPips, b_tradeDirectionUp, b_riskRewardOn;
			const double perc10 = 0.1;
			const double perc25 = 0.25;
			// default values for comboboxes
			private int defaultIndx_boxCurrency = 0;
			private int defaultIndx_direction = 1;
			private int defaultIndx_OrderSize = 5;
			private int defaultIndx_riskRewardRatio = 6;
			private int default_labelTxtBoxHorrOffset = 6;	// Default horizontal distance between label and text box
			private int i_tradeParam2, i_tradeParam3, i_tradeParam1;
			private int i_riskRewardRatioIndx;
			private string str_appPath, str_dataPath, str_userPath;
			double piplevel;								// factor to convert to/from pips
			string numFormat;								// the number format to use
			string currFormat;								// currency format
			cr curr = new cr();								// local currency object
			private ResourceManager ResMan;
			private CurrencyDataProvider currency;
			private string default_equity_str = global::Data4Mat.TradeCalculator.Properties.Settings.Default.Equity;
			private TradeMethod tradeType = TradeMethod.None;
			private RiskRewardRatioItems rrItems = new RiskRewardRatioItems();


			/// <summary>
			/// 
			/// </summary>
			public trdcalc()
			{
				InitializeComponent();
				ResMan = new ResourceManager(typeof(string));
				currency = new CurrencyDataProvider();
				Initialize();
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnClosing(CancelEventArgs e)
			{
				global::Data4Mat.TradeCalculator.Properties.Settings.Default.Equity = this.input_equityData.Value.ToString();
				global::Data4Mat.TradeCalculator.Properties.Settings.Default.Save();
				base.OnClosing(e);
			}
			/// <summary>
			/// Initializes the form to its default layout and default content.
			/// </summary>
			private void Initialize()
			{
				LoadCurrencies();
				this.input_Data1.Focus();
				this.input_directionData.SelectedIndex = 1;
				if (this.input_OrderSizeData.Items.Count > 5)
				{
					this.input_OrderSizeData.SelectedIndex = 5;
				}
				else
				{
					this.input_OrderSizeData.SelectedIndex = 0;
				}
				this.input_currencyData.SelectedIndex = 0;
				string tmp = global::Data4Mat.TradeCalculator.Properties.Settings.Default.Equity;
				if (tmp != "" && tmp != "0") this.input_equityData.SetText(tmp);
				this.b_exit1 = false;
				this.b_riskRewPips = true;
				this.b_stopLoss = false;
				this.statusBox_spreadExit1.Text = "Off";
				this.statusBox_spreadSL.Text = "Off";

				this.input_exit1Chk.Checked = false;
				this.input_stopLossChk.Checked = false;
				this.input_riskRewPips.Checked = true;
				this.input_riskRewAmount.Checked = false;
				this.input_riskRewardOn.Checked = false;
				this.Group_RiskReward.Enabled = false;

				// Load values to the input_riskRewRatioData comboBox
				for (int i = 0; i < this.rrItems.Length; i++)
				{
					this.input_riskRewRatioData.Items.Add(this.rrItems[i].Ratio);
				}
				this.input_riskRewRatioData.SelectedIndex = 6;

				this.statusBox_riskRewardType.Text = "PIPs";
				this.statusBox_riskRewardRatio.Text = this.input_riskRewRatioData.SelectedItem.ToString();
				this.statusBox_riskRewardType.Enabled = false;
				this.statusBox_riskRewardRatio.Enabled = false;

				this.str_appPath = Application.StartupPath;
				this.str_dataPath = this.str_appPath + @"\Data";
				this.str_userPath = this.str_appPath + @"\User Files";

				this.saveTC1File.InitialDirectory = this.str_userPath;
			}

			// Event handlers for menu items
			#region Menu Event Handlers

			private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
			{
				bool x = this.TopMost;
				this.TopMost = false;
				AboutBox1 xd = new AboutBox1();
				xd.ShowDialog();
				this.TopMost = x;
			}
			private void exitToolStripMenuItem_Click(object sender, EventArgs e)
			{
				Application.Exit();
			}
			private void slingShotToolStripMenuItem_Click(object sender, EventArgs e)
			{
				ResetAllData(sender, e);
				SetupSlingShot();
			}
			private void ascendingToolStripMenuItem_Click(object sender, EventArgs e)
			{
				ResetAllData(sender, e);
				SetupAscendingTriangleBreakout();
			}
			private void descendingToolStripMenuItem_Click(object sender, EventArgs e)
			{
				ResetAllData(sender, e);
				SetupDescendingTriangleBreakout();
			}
			private void flagpoleToolStripMenuItem_Click(object sender, EventArgs e)
			{
				ResetAllData(sender, e);
				SetupFlagPole();
			}
			/// <summary>
			/// Toggles the setting of keeping the application window
			/// on top of other windows on the the screen.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
			{
				if (this.TopMost == true)
				{
					this.TopMost = false;
				}
				else
				{
					this.TopMost = true;
				}
			}
			/// <summary>
			/// Runs the new currency interface.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void addCurrencyToolStripMenuItem_Click(object sender, EventArgs e)
			{
				bool x = this.TopMost;
				this.TopMost = false;
				NewCurrencyPairForm frm = new NewCurrencyPairForm();
				frm.FormClosing += new FormClosingEventHandler(frm_FormClosing);
				frm.ShowDialog(this);
				this.TopMost = x;
			}
			private void saveTradeToolStripMenuItem_Click(object sender, EventArgs e)
			{
				throw new NotImplementedException();
			}
			private void EMA10TradeSetupToolStripMenuItem_Click(object sender, EventArgs e)
			{
				ResetAllData(sender, e);
				SetupEMA10TradeSetup();
			}
			private void EMA10TradeMaintenanceToolStripMenuItem_Click(object sender, EventArgs e)
			{
				ResetAllData(sender, e);
				SetupEMA10TradeMaintenance();
			}
			private void symmetricalTriangleToolStripMenuItem_Click(object sender, EventArgs e)
			{
				ResetAllData(sender, e);
				SetupSymmTriangleBreakout();
			}
			private void symmetricalTriangle2LegToolStripMenuItem_Click(object sender, EventArgs e)
			{
				ResetAllData(sender, e);
				SetupSymmTriangle2LegBreakout();
			}
			private void horizontalRangeToolStripMenuItem_Click(object sender, EventArgs e)
			{
				ResetAllData(sender, e);
				SetupHorizontalRangeBreakout();
			}
			private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
			{
				bool x = this.TopMost;
				this.TopMost = false;
				HelpFrm box = new HelpFrm();
				box.Show(this);
				this.TopMost = x;
			}
			private void saveToolStripMenuItem_Click(object sender, EventArgs e)
			{
				string path = Application.ExecutablePath;
				//path += @"\Trade Files";
				string[] tmp = path.Split('\\');
				path = "";
				for (int i = 0; i < tmp.Length - 1; i++)
				{
					path += tmp[i] + @"\";
				}
				path += @"Trade Files\";
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				this.saveTC1File.InitialDirectory = path;
				if (this.saveTC1File.ShowDialog(this) == DialogResult.OK)
				{
					Hashtable data = Serialize();

					FileStream fs = (FileStream)this.saveTC1File.OpenFile();
					BinaryFormatter bfm = new BinaryFormatter();
					try
					{
						bfm.Serialize(fs, data);
					}
					catch (SerializationException ex)
					{
						MessageBox.Show("Serialization failed: " + ex.Message);
					}
					finally
					{
						fs.Close();
					}
				}
			}
			private void newToolStripMenuItem_Click(object sender, EventArgs e)
			{

			}
			private void openToolStripMenuItem_Click(object sender, EventArgs e)
			{
				string path = Application.ExecutablePath;
				//path += @"\Trade Files";
				string[] tmp = path.Split('\\');
				Hashtable inData = null;
				path = "";
				for (int i = 0; i < tmp.Length - 1; i++)
				{
					path += tmp[i] + @"\";
				}
				path += @"Trade Files\";
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				this.openTC1File.InitialDirectory = path;

				if (this.openTC1File.ShowDialog() == DialogResult.OK)
				{
					FileStream fs = (FileStream)this.openTC1File.OpenFile();
					BinaryFormatter bfm = new BinaryFormatter();

					try
					{
						inData = (Hashtable)bfm.Deserialize(fs);
						ReadInData(inData);
					}
					catch (SerializationException ex)
					{
						MessageBox.Show("Failed to deserialize file: " + ex.Message);
					}
					finally
					{
						fs.Close();
					}
				}
			}
			private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
			{
				saveToolStripMenuItem_Click(sender, e);
			}
			private void printToolStripMenuItem_Click(object sender, EventArgs e)
			{

			}
			private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
			{

			}
			private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
			{

			}
			private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
			{

			}

			#endregion

			// Other types of event handlers for objects
			#region Other Event Handlers

			/// <summary>
			/// Event Handler for the form closing event of the NewCurrencyPairForm.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void frm_FormClosing(object sender, FormClosingEventArgs e)
			{
				if (!((NewCurrencyPairForm)sender).Canceled) ReLoadCurrencies();
			}
			private void input_stopLossChk_CheckedChanged(object sender, EventArgs e)
			{
				if (this.input_stopLossChk.Checked)
				{
					this.statusBox_spreadSL.Text = "On";
					this.b_stopLoss = true;
				}
				else
				{
					this.statusBox_spreadSL.Text = "Off";
					this.b_stopLoss = false;
				}
			}
			private void input_exit1Chk_CheckedChanged(object sender, EventArgs e)
			{
				if (this.input_exit1Chk.Checked)
				{
					this.statusBox_spreadExit1.Text = "On";
					this.b_exit1 = true;
				}
				else
				{
					this.statusBox_spreadExit1.Text = "Off";
					this.b_exit1 = false;
				}
			}
			private void input_riskRewardOn_CheckedChanged(object sender, EventArgs e)
			{
				if (this.input_riskRewardOn.Checked)
				{
					this.Group_RiskReward.Enabled = true;
					this.input_exit1Chk.Enabled = false;
					this.statusBox_riskRewardOn.Text = "On";
					this.statusBox_spreadExit1.Enabled = false;
					this.statusBox_riskRewardType.Enabled = true;
					this.statusBox_riskRewardRatio.Enabled = true;
				}
				else
				{
					this.Group_RiskReward.Enabled = false;
					this.input_exit1Chk.Enabled = true;
					this.statusBox_riskRewardOn.Text = "Off";
					this.statusBox_spreadExit1.Enabled = true;
					this.statusBox_riskRewardType.Enabled = false;
					this.statusBox_riskRewardRatio.Enabled = false;
				}
			}
			private void input_riskRewRatio_Changed(object sender, EventArgs e)
			{
				this.statusBox_riskRewardRatio.Text = this.input_riskRewRatioData.SelectedItem.ToString();
			}
			private void input_riskRew_CheckedChanged(object sender, EventArgs e)
			{
				if (this.input_riskRewPips.Checked)
				{
					this.statusBox_riskRewardType.Text = "PIPs";
				}
				else
				{
					this.statusBox_riskRewardType.Text = "Amount";
				}
			}
			private void SideLabelSizeChanged(object sender, EventArgs e)
			{
				string name = ((Label)sender).Name;
				string[] n = name.Split('_');
				string boxName = "";

				for (int i = 0; i < n.Length - 1; i++)
				{
					if (i == 0) boxName = n[i];
					else boxName += "_" + n[i];
				}

				//int bLeft = this.Controls.Find(boxName,true)[0].Location.X;
				int bLeft = this.FindForm().Controls.Find(boxName, true)[0].Location.X;
				int labLength = ((Label)sender).Size.Width;
				//((Label)sender).Location.X = bLeft - labLength - offset;
				((Label)sender).SetBounds(bLeft - labLength - this.default_labelTxtBoxHorrOffset, ((Label)sender).Location.Y, ((Label)sender).Size.Width, ((Label)sender).Size.Height);
			}

			#endregion

			// Functions doing all the calculations of data input and recieved
			#region Calculation Functions

			private void CalculateSlingShotData()
			{
				try
				{
					// Offset is default value for caculation of stop loss and exit1 points. Part of the trading method,
					// collected from Resources storage.
					double offset = double.Parse(global::Data4Mat.TradeCalculator.Properties.Resources.SlingShotOffset);

					// Check that the Level (input1) not equals Exit2 (input2)
					if (this.dbl_input1 == this.dbl_input2)
					{
						throw new IOException("The Level and Exit 2 can not be the same value!");
					}

					// Calculate StopLoss and Exit1 points
					if (this.dbl_input1 < this.dbl_input2) // up direction
					{
						// Calculate entry point
						dbl_enter = this.dbl_input1 + curr.Spread / piplevel;

						// Calculate Stop/Loss point
						if (this.b_stopLoss)
						{
							dbl_sl = dbl_enter - (offset + 2 * curr.Spread) / piplevel;
							// '2*curr.spread' Spread 2 times one for the general method and ones for the compensation.
						}
						else
						{
							dbl_sl = dbl_enter - (offset + curr.Spread) / piplevel;
						}

						// Calculate Exit1 point
						if (this.b_riskRewardOn)
						{
							double ratio = this.rrItems[this.i_riskRewardRatioIndx].GetRatio();

							// Risk/Reward ratio calculations. Applies to Exit1. Exit2 and Exit3 are not directly
							// apart of the Risk/Reward ratio calculations since they are not a function of the Risk factor
							// but set on other criteria.
							if (this.b_riskRewPips) // Risk/Reward calculations based on PIPs
							{
								dbl_exit1 = dbl_enter + (Math.Abs(dbl_enter - dbl_sl) * ratio);
							}
							else // Risk/Reward calcualtions based on amount of currency
							{
								//					Calculation logic.
								// tmpAmountLoss = Math.Abs(dbl_enter - dbl_sl) * piplevel * curr.TickValue * 2;	(Gives amount for loss)
								// tmpAmountExit1 = tmpAmountLoss * ratio;											(Gives amount at exit1)
								// tmpExit1 = tmpAmountLoss / curr.TickValue;										(Translates amount into PIPs equivalent)
								// Calculates Exit1 point
								dbl_exit1 = dbl_enter + (Math.Abs(dbl_enter - dbl_sl) * ratio * 2);
							}
						}
						else
						{
							if (this.b_exit1)
							{
								// Compensate for spread on exit1
								dbl_exit1 = dbl_enter + (offset + 2 * curr.Spread) / piplevel;
							}
							else
							{
								dbl_exit1 = dbl_enter + (offset + curr.Spread) / piplevel;
							}
						}
					}
					else // down direction
					{
						// Calculate entry point
						dbl_enter = this.dbl_input1 - curr.Spread / piplevel;

						// Calculate Stop/Loss point
						if (this.b_stopLoss)
						{
							dbl_sl = dbl_enter + (offset + 2 * curr.Spread) / piplevel;
							// '2*curr.spread' Spread 2 times one for the general method and ones for the compensation.
						}
						else
						{
							dbl_sl = dbl_enter + (offset + curr.Spread) / piplevel;
						}

						// Calculate Exit1 point
						if (this.b_riskRewardOn)
						{
							double ratio = this.rrItems[this.i_riskRewardRatioIndx].GetRatio();

							// Risk/Reward ratio calculations. Applies to Exit1. Exit2 and Exit3 are not directly
							// apart of the Risk/Reward ratio calculations since they are not a function of the Risk factor
							// but set on other criteria.
							if (this.b_riskRewPips) // Risk/Reward calculations based on PIPs
							{
								dbl_exit1 = dbl_enter - Math.Abs(dbl_enter - dbl_sl) * ratio;
							}
							else // Risk/Reward calcualtions based on amount of currency
							{
								//					Calculation logic.
								// tmpAmountLoss = Math.Abs(dbl_enter - dbl_sl) * piplevel * curr.TickValue * 2;	(Gives amount for loss)
								// tmpAmountExit1 = tmpAmountLoss * ratio;											(Gives amount at exit1)
								// tmpExit1 = tmpAmountLoss / curr.TickValue;										(Translates amount into PIPs equivalent)
								// Calculates Exit1 point
								dbl_exit1 = dbl_enter - (Math.Abs(dbl_enter - dbl_sl) * ratio * 2);
							}
						}
						else
						{
							if (this.b_exit1)
							{
								// Compensate for spread on exit1
								dbl_exit1 = dbl_enter - (offset + 2 * curr.Spread) / piplevel;
							}
							else
							{
								dbl_exit1 = dbl_enter - (offset + curr.Spread) / piplevel;
							}
						}
					}
					// Calclate Exit2 and Exit3 points
					dbl_exit2 = this.dbl_input2;
					dbl_exit3 = this.dbl_input3;

					// Caculate displayable values
					this.dbl_pipLoss = Math.Round(Math.Abs(this.dbl_enter - this.dbl_sl) * piplevel * 2, 0);
					this.dbl_pipExit1 = Math.Round(Math.Abs(this.dbl_enter - this.dbl_exit1) * piplevel, 0);
					this.dbl_pipExit2 = Math.Round(Math.Abs(this.dbl_enter - dbl_exit2) * piplevel, 0);
					this.dbl_pipExit3 = dbl_exit3;
					this.dbl_pipSubTot = this.dbl_pipExit1 + this.dbl_pipExit2;
					if (this.dbl_pipExit3 > 0) this.dbl_pipTotal = this.dbl_pipSubTot + this.dbl_pipExit3;
					else if (this.dbl_pipSubTot > 0) this.dbl_pipTotal = this.dbl_pipSubTot;
					else this.dbl_pipTotal = -1.0;
					this.dbl_amountLoss = this.dbl_pipLoss * curr.TickValue * this.orderSize;
					this.dbl_amountExit1 = this.dbl_pipExit1 * curr.TickValue * this.orderSize;
					this.dbl_amountExit2 = this.dbl_pipExit2 * curr.TickValue * this.orderSize;
					this.dbl_amountSubTot = this.dbl_amountExit1 + this.dbl_amountExit2;
					this.dbl_amountExit3 = this.dbl_exit3 * curr.TickValue * this.orderSize;
					if (dbl_amountExit3 > 0) this.dbl_amountTotal = this.dbl_amountSubTot + this.dbl_amountExit3;
					else if (dbl_amountSubTot > 0) this.dbl_amountTotal = this.dbl_amountSubTot;
					else this.dbl_amountTotal = -1.0;
				}
				catch (IOException e)
				{
					MessageBox.Show(e.Message, "Input Error!", MessageBoxButtons.OK);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message + " - Source: " + e.Source, "Application Error!", MessageBoxButtons.OK);
				}

			}
			private void CalculateFlagpoleData()
			{
				// Check data
				if (this.dbl_input1 <= this.dbl_input2)
				{
					MessageBox.Show("High (" + dbl_input1.ToString(numFormat) + ") is less than or equal to Low (" + dbl_input2.ToString(numFormat) + ").\r\nHigh must of course be higher than Low.\r\nCorrect input values!", "Input Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					try
					{
						// Caculate intermittant values
						dbl_height = Math.Abs(dbl_input1 - dbl_input2);							// Calculate height of flagpole in decimal form
						i_tradeParam1 = (int)Math.Round(dbl_height * this.piplevel, 0);			// Calculates the height of the flapole in pips
						i_tradeParam2 = (int)Math.Round(perc10 * i_tradeParam1);				// Calculates 10% of the height of the flagpole in pips
						i_tradeParam3 = (int)Math.Round(perc25 * i_tradeParam1);				// Calculates 25% of the height of the flagpole in pips
						dbl_exit3 = -1.0;														// Set exit3 to 0 since it is not used in this trading method

						// Caculates entry, stop loss, exit1, and exit2 depending on direction of trade
						if (this.b_tradeDirectionUp) // Up direction
						{
							// Calculate entry point
							dbl_enter = dbl_input1 + double.Parse(i_tradeParam2.ToString()) / this.piplevel;

							// Calculate Stop/Loss point
							if (this.b_stopLoss) // Calculate with spread compensation
							{
								dbl_sl = dbl_enter - double.Parse((i_tradeParam3 + curr.Spread).ToString()) / this.piplevel;
							}
							else // Calculate without spread compensation
							{
								dbl_sl = dbl_enter - double.Parse(i_tradeParam3.ToString()) / this.piplevel;
							}

							// Calculate Exit1 point
							if (this.b_riskRewardOn) // Ratio calculations
							{
								double ratio = this.rrItems[this.i_riskRewardRatioIndx].GetRatio();
								if (this.b_riskRewPips) // Risk/Reward calculation based on PIPs
								{
									dbl_exit1 = dbl_enter + Math.Abs(dbl_enter - dbl_sl) * ratio;
								}
								else // Risk/Reward calculation based on currency amount
								{
									dbl_exit1 = dbl_enter + Math.Abs(dbl_enter - dbl_sl) * ratio * 2;
								}
							}
							else // No ratio calculations
							{
								if (this.b_exit1) // Compensate Exit1 with spread
								{
									dbl_exit1 = dbl_enter + double.Parse((i_tradeParam3 + curr.Spread).ToString()) / this.piplevel;
								}
								else // No spread compensation
								{
									dbl_exit1 = dbl_enter + double.Parse(i_tradeParam3.ToString()) / this.piplevel;
								}
							}

							// Calculate Exit2 point
							dbl_exit2 = dbl_input1 + dbl_height;
						}
						else // Down direction
						{
							// Calculate entry point
							dbl_enter = dbl_input2 - double.Parse(i_tradeParam2.ToString()) / this.piplevel;

							// Calculate Stop/Loss point
							if (this.b_stopLoss) // Calculate with spread compensation
							{
								dbl_sl = dbl_enter + double.Parse((i_tradeParam3 + curr.Spread).ToString()) / this.piplevel;
							}
							else // Calculate without spread compensation
							{
								dbl_sl = dbl_enter + double.Parse(i_tradeParam3.ToString()) / this.piplevel;
							}

							// Calculate Exit1 point
							if (this.b_riskRewardOn) // Ratio calculations
							{
								double ratio = this.rrItems[this.i_riskRewardRatioIndx].GetRatio();
								if (this.b_riskRewPips) // Risk/Reward calculation based on PIPs
								{
									dbl_exit1 = dbl_enter - Math.Abs(dbl_enter - dbl_sl) * ratio;
								}
								else // Risk/Reward calculation based on currency amount
								{
									dbl_exit1 = dbl_enter - Math.Abs(dbl_enter - dbl_sl) * ratio * 2;
								}
							}
							else // No ratio calculations
							{
								if (this.b_exit1) // Compensate Exit1 with spread compensation
								{
									dbl_exit1 = dbl_enter - double.Parse((i_tradeParam3 + curr.Spread).ToString()) / this.piplevel;
								}
								else // No spread compensation
								{
									dbl_exit1 = dbl_enter - double.Parse(i_tradeParam3.ToString()) / this.piplevel;
								}
							}

							//Calculate Exit2 point
							dbl_exit2 = dbl_input2 - dbl_height;
							if (this.dbl_exit1 < this.dbl_exit2)
							{
								this.out_warningText_lbl.Visible = true;
								this.out_warningText.Visible = true;
								this.out_warningText.Text = "Exit1 (" + this.dbl_exit1.ToString(numFormat) + ") will not execute before Exit2 (" + this.dbl_exit2.ToString(numFormat) + ") is reached.\r\nPlease, adjust ratio so that the proper execution order remains.";
							}
						}

						// Calculate Exit3 point
						this.dbl_exit3 = this.dbl_input3;

						// Sets all values for displaying
						this.dbl_pipLoss = Math.Round(Math.Abs(this.dbl_enter - this.dbl_sl) * piplevel * 2, 0);
						this.dbl_pipExit1 = Math.Round(Math.Abs(this.dbl_enter - this.dbl_exit1) * piplevel, 0);
						this.dbl_pipExit2 = Math.Round(Math.Abs(this.dbl_enter - this.dbl_exit2) * piplevel, 0);
						// Maybe future use will need a third exit point
						if (this.dbl_exit3 > 0) this.dbl_pipExit3 = Math.Round(Math.Abs(this.dbl_enter - this.dbl_exit3) * piplevel, 0);
						else this.dbl_pipExit3 = dbl_exit3;
						this.dbl_pipSubTot = this.dbl_pipExit1 + this.dbl_pipExit2;
						if (this.dbl_pipExit3 > 0) this.dbl_pipTotal = this.dbl_pipSubTot + this.dbl_pipExit3;
						else if (this.dbl_pipSubTot > 0) this.dbl_pipTotal = this.dbl_pipSubTot;
						else this.dbl_pipTotal = -1.0;
						this.dbl_amountLoss = this.dbl_pipLoss * curr.TickValue * this.orderSize;
						this.dbl_amountExit1 = this.dbl_pipExit1 * curr.TickValue * this.orderSize;
						this.dbl_amountExit2 = this.dbl_pipExit2 * curr.TickValue * this.orderSize;
						this.dbl_amountSubTot = this.dbl_amountExit1 + this.dbl_amountExit2;
						this.dbl_amountExit3 = this.dbl_pipExit3 * curr.TickValue * this.orderSize;
						if (dbl_amountExit3 > 0) this.dbl_amountTotal = this.dbl_amountSubTot + this.dbl_amountExit3;
						else if (dbl_amountSubTot > 0) this.dbl_amountTotal = this.dbl_amountSubTot;
						else this.dbl_amountTotal = -1.0;
					}
					catch (Exception e)
					{
						MessageBox.Show(e.Message + " - Source: " + e.Source, "Application Error!", MessageBoxButtons.OK);
					}
				}
			}
			private void CalculateEntryEMA10Data()
			{
				double tmp;
				bool inErr = false;

				// Calculate entry point
				this.dbl_enter = this.dbl_input1;
				// Calculate intemittant values
				tmp = this.dbl_input2 * 0.5;
				this.i_tradeParam1 = (int)Math.Round(tmp * piplevel, 0);

				// Check input data
				if (this.dbl_input1 <= this.dbl_input2)
				{
					DialogResult res = new DialogResult();

					res = MessageBox.Show("EMA(10) (" + dbl_input1.ToString(numFormat) + ") is less than or equal to ATR(14) (" + dbl_input2.ToString(numFormat) + ").\r\nAre these values correct?", "Input Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

					if (res == DialogResult.Yes) inErr = false;
					else inErr = true;
				}
				if (!inErr) // Calculate data if not inErr
				{
					if (this.b_tradeDirectionUp) // Up direction
					{
						// Calculate Stop/Loss
						if (this.b_stopLoss) // Spread Compensation
						{
							this.dbl_sl = this.dbl_enter - (Math.Round((tmp), curr.Decimals) + curr.Spread / piplevel);
						}
						else // No Spread Compensation
						{
							this.dbl_sl = this.dbl_enter - Math.Round(tmp, curr.Decimals);
						}
					}
					else // Down direction
					{
						// Calculate Stop/Loss
						if (this.b_stopLoss) // Spread Compensation
						{
							this.dbl_sl = this.dbl_enter + (Math.Round(tmp, curr.Decimals) + curr.Spread / piplevel);
						}
						else // No Spread Compensation
						{
							this.dbl_sl = this.dbl_enter + Math.Round(tmp, curr.Decimals);
						}
					}
				}

				// Calculate Output variables
				this.dbl_pipLoss = Math.Round(Math.Abs(this.dbl_sl - this.dbl_enter) * piplevel, 0);
				this.dbl_pipExit1 = -1.0;
				this.dbl_pipExit2 = -1.0;
				this.dbl_pipExit3 = -1.0;
				this.dbl_pipSubTot = this.dbl_pipExit1 + this.dbl_pipExit2;
				this.dbl_pipTotal = this.dbl_pipSubTot + this.dbl_pipExit3;
				this.dbl_amountLoss = this.dbl_pipLoss * curr.TickValue * this.orderSize;
				this.dbl_amountExit1 = this.dbl_pipExit1 * curr.TickValue * this.orderSize;
				this.dbl_amountExit2 = this.dbl_pipExit2 * curr.TickValue * this.orderSize;
				this.dbl_amountSubTot = this.dbl_amountExit1 + this.dbl_amountExit2;
				this.dbl_amountExit3 = this.dbl_exit3 * curr.TickValue * this.orderSize;
				this.dbl_amountTotal = this.dbl_amountSubTot + this.dbl_amountExit3;
			}
			private void CalculateEMA10MaintenanceData()
			{
				double tmp, ema, atr;
				bool inErr = false;

				// Check input data
				if (this.dbl_input1 <= this.dbl_input3 && this.b_tradeDirectionUp)
				{
					DialogResult res = MessageBox.Show("EMA(10) (" + dbl_input1.ToString(numFormat) + ") is greater than or equal to Exit2 (" + dbl_input3.ToString(numFormat) + ").\r\nEMA(10) should be less than Exit2 for an upward trade direction.\r\rDo you want to continue calulations with these entires?", "Input Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (res == DialogResult.Yes) inErr = false;
					else inErr = true;
				}
				else if (this.dbl_input1 >= this.dbl_input3 && !this.b_tradeDirectionUp)
				{
					DialogResult res = MessageBox.Show("EMA(10) (" + dbl_input1.ToString(numFormat) + ") is less than or equal to Exit2 (" + dbl_input3.ToString(numFormat) + ").\r\nEMA(10) should be greater than Exit2 for an upward trade direction.\r\rDo you want to continue calulations with these entires?", "Input Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (res == DialogResult.Yes) inErr = false;
					else inErr = true;
				}

				if (!inErr) // Do calculations if not inErr
				{

					// Calculate entry point
					this.dbl_enter = this.dbl_input3;			// Trade initial entry point.

					// Calculate intemittant values
					ema = this.dbl_input1;						// Last candle's EMA(10) value
					atr = this.dbl_input2;						// Last period's ATR(14) value
					tmp = atr * 0.5;							// Calculate 50% of ATR(14)
					this.i_tradeParam1 = (int)Math.Round(tmp * piplevel, 0);

					if (this.b_tradeDirectionUp) // Up direction
					{
						// Calculate Stop/Loss
						if (this.b_stopLoss) // Spread Compensation
						{
							this.dbl_sl = ema - (Math.Round(tmp, curr.Decimals) + curr.Spread / piplevel);
						}
						else // No Spread Compensation
						{
							this.dbl_sl = ema - Math.Round(tmp, curr.Decimals);
						}
					}
					else // Down direction
					{
						// Calculate Stop/Loss
						if (this.b_stopLoss) // Spread Compensation
						{
							this.dbl_sl = ema + (Math.Round(tmp, curr.Decimals) + curr.Spread / piplevel);
						}
						else // No Spread Compensation
						{
							this.dbl_sl = ema + Math.Round(tmp, curr.Decimals);
						}
					}

					// Caculate output values
					if (this.b_tradeDirectionUp)
					{
						this.dbl_pipLoss = Math.Round((this.dbl_enter - this.dbl_sl) * piplevel, 0);
					}
					else
					{
						this.dbl_pipLoss = Math.Round((this.dbl_sl - this.dbl_enter) * piplevel, 0);
					}


					if (this.dbl_pipLoss <= 0)
					{
						this.dbl_pipExit1 = -1.0 * this.dbl_pipLoss;
						this.dbl_pipExit2 = -1.0;
						this.dbl_pipExit3 = -1.0;
						this.dbl_pipSubTot = this.dbl_pipExit1;
						this.dbl_pipTotal = this.dbl_pipSubTot;
						this.dbl_amountLoss = -1.0;
						this.dbl_amountExit1 = this.dbl_pipExit1 * curr.TickValue * this.orderSize;
						this.dbl_amountExit2 = -1.0;
						this.dbl_amountSubTot = this.dbl_amountExit1;
						this.dbl_amountExit3 = -1.0;
						this.dbl_amountTotal = this.dbl_amountSubTot;
					}
					else
					{
						this.dbl_pipExit1 = -1.0;
						this.dbl_pipExit2 = -1.0;
						this.dbl_pipExit3 = -1.0;
						this.dbl_pipSubTot = -1.0;
						this.dbl_pipTotal = -1.0;
						this.dbl_amountLoss = this.dbl_pipLoss * curr.TickValue * this.orderSize;
						this.dbl_amountExit1 = -1.0;
						this.dbl_amountExit2 = -1.0;
						this.dbl_amountSubTot = -1.0;
						this.dbl_amountExit3 = -1.0;
						this.dbl_amountTotal = -1.0;
					}
				}
			}
			private void CalculateAscendingTriangleData()
			{
				double atr, offset;
				double ratio = this.rrItems[this.i_riskRewardRatioIndx].GetRatio();
				bool errInput = false;

				// Check input values
				if (this.dbl_input1 >= this.dbl_input2)
				{
					MessageBox.Show("Level (" + this.dbl_input1.ToString(numFormat) + ") is equal to or greater than Exit2 (" + this.dbl_input3.ToString(numFormat) + ")", "Input Inconsistent With Trade!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					errInput = true;
				}
				if (!errInput)
				{
					// Calculate trade parameters
					atr = this.dbl_input2;
					offset = Math.Round(atr * 0.5, curr.Decimals);
					this.i_tradeParam1 = (int)Math.Round((offset * piplevel), 0);

					// Calculate entry point
					this.dbl_enter = this.dbl_input1 + offset;

					// Calculate Stop/Loss
					if (this.b_stopLoss) // Spread Compensation
					{
						this.dbl_sl = this.dbl_enter - (offset + curr.Spread / piplevel);
					}
					else // No Spread Compensation
					{
						this.dbl_sl = this.dbl_enter - offset;
					}

					// Calculate Exit1
					if (this.b_riskRewardOn) // Risk/Reward ratio calculation
					{
						if (this.b_riskRewPips) // Risk/Reward ratio based on PIPs
						{
							this.dbl_exit1 = this.dbl_enter + (Math.Abs(this.dbl_enter - this.dbl_sl) * ratio);
						}
						else // Risk/Reward ratio based on amount
						{
							this.dbl_exit1 = this.dbl_enter + (Math.Abs(this.dbl_enter - this.dbl_sl) * ratio * 2);
						}
					}
					else // No Risk/Reward ratio calculation
					{
						if (this.b_exit1) // Spread compensate Exit1
						{
							this.dbl_exit1 = this.dbl_enter + (offset + curr.Spread / piplevel);
						}
						else // No spread compensation
						{
							this.dbl_exit1 = this.dbl_enter + offset;
						}
					}

					// Calculate Exit2 & Exit3 points
					this.dbl_exit2 = dbl_input3;
					this.dbl_exit3 = -1;

					// Calculate visible data
					this.dbl_pipLoss = Math.Round((dbl_enter - dbl_sl) * piplevel * 2, 0);
					this.dbl_pipExit1 = Math.Round((this.dbl_exit1 - this.dbl_enter) * piplevel, 0);
					this.dbl_pipExit2 = Math.Round((this.dbl_exit2 - this.dbl_enter) * piplevel, 0);
					this.dbl_pipExit3 = -1.0;
					this.dbl_pipSubTot = this.dbl_pipExit1 + this.dbl_pipExit2;
					this.dbl_pipTotal = this.dbl_pipSubTot;
					this.dbl_amountLoss = this.dbl_pipLoss * curr.TickValue * this.orderSize;
					this.dbl_amountExit1 = this.dbl_pipExit1 * curr.TickValue * this.orderSize;
					this.dbl_amountExit2 = this.dbl_pipExit2 * curr.TickValue * this.orderSize;
					this.dbl_amountSubTot = this.dbl_amountExit1 + this.dbl_amountExit2;
					this.dbl_amountExit3 = this.dbl_exit3 * curr.TickValue * this.orderSize;
					this.dbl_amountTotal = this.dbl_amountSubTot;
				}
			}
			private void CalculateDescendingTriangleData()
			{
				double atr, offset;
				double ratio = this.rrItems[this.i_riskRewardRatioIndx].GetRatio();
				bool errInput = false;

				// Check input values
				if (this.dbl_input1 <= this.dbl_input2)
				{
					MessageBox.Show("Level (" + this.dbl_input1.ToString(numFormat) + ") is equal to or less than Exit2 (" + this.dbl_input3.ToString(numFormat) + ")", "Input Inconsistent With Trade!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					errInput = true;
				}
				if (!errInput)
				{
					atr = this.dbl_input2;
					offset = Math.Round(atr * 0.5, curr.Decimals);
					this.i_tradeParam1 = (int)Math.Round((offset * piplevel), 0);

					// Calculate entry point
					this.dbl_enter = this.dbl_input1 - offset;

					// Calculate Stop/Loss
					if (this.b_stopLoss) // Spread Compensation
					{
						this.dbl_sl = this.dbl_enter + (offset + curr.Spread / piplevel);
					}
					else // No Spread Compensation
					{
						this.dbl_sl = this.dbl_enter + offset;
					}

					// Calculate Exit1
					if (this.b_riskRewardOn) // Risk/Reward ratio calculation
					{
						if (this.b_riskRewPips) // Risk/Reward ratio based on PIPs
						{
							this.dbl_exit1 = this.dbl_enter - (Math.Abs(this.dbl_enter - this.dbl_sl) * ratio);
						}
						else // Risk/Reward ratio based on amount
						{
							this.dbl_exit1 = this.dbl_enter - (Math.Abs(this.dbl_enter - this.dbl_sl) * ratio * 2);
						}
					}
					else // No Risk/Reward ratio calculation
					{
						if (this.b_exit1) // Spread compensate Exit1
						{
							this.dbl_exit1 = this.dbl_enter - (offset + curr.Spread / piplevel);
						}
						else // No spread compensation
						{
							this.dbl_exit1 = this.dbl_enter - offset;
						}
					}

					// Calculate Exit2 & Exit3 points
					this.dbl_exit2 = dbl_input3;
					this.dbl_exit3 = -1;

					// Calculate visible data
					this.dbl_pipLoss = Math.Round((dbl_sl - dbl_enter) * piplevel * 2, 0);
					this.dbl_pipExit1 = Math.Round((this.dbl_enter - this.dbl_exit1) * piplevel, 0);
					this.dbl_pipExit2 = Math.Round((this.dbl_enter - this.dbl_exit2) * piplevel, 0);
					this.dbl_pipExit3 = -1.0;
					this.dbl_pipSubTot = this.dbl_pipExit1 + this.dbl_pipExit2;
					this.dbl_pipTotal = this.dbl_pipSubTot;
					this.dbl_amountLoss = this.dbl_pipLoss * curr.TickValue * this.orderSize;
					this.dbl_amountExit1 = this.dbl_pipExit1 * curr.TickValue * this.orderSize;
					this.dbl_amountExit2 = this.dbl_pipExit2 * curr.TickValue * this.orderSize;
					this.dbl_amountSubTot = this.dbl_amountExit1 + this.dbl_amountExit2;
					this.dbl_amountExit3 = this.dbl_exit3 * curr.TickValue * this.orderSize;
					this.dbl_amountTotal = this.dbl_amountSubTot;
				}
			}
			private void CalculateHorrizontalRangeBreakoutData()
			{
				double ratio = this.rrItems[this.i_riskRewardRatioIndx].GetRatio();

				dbl_height = dbl_input1 - dbl_input2;
				i_tradeParam2 = (int)Math.Round(dbl_height * 0.25 * piplevel, 0);
				i_tradeParam3 = (int)Math.Round(dbl_height * 0.5 * piplevel, 0);

				if (this.b_tradeDirectionUp) // Up direction
				{
					// Calculate entry point
					dbl_enter = Math.Round(dbl_input1 + i_tradeParam2 / piplevel, curr.Decimals);

					// Calculate intemittant values (If not direction independant)

					// Calculate Stop/Loss
					if (this.b_stopLoss) // Spread Compensation
					{
						dbl_sl = Math.Round(dbl_enter - (i_tradeParam3 + curr.Spread) / piplevel, curr.Decimals);
					}
					else // No Spread Compensation
					{
						dbl_sl = Math.Round(dbl_enter - i_tradeParam3 / piplevel, curr.Decimals);
					}

					// Calculate Exit1
					if (this.b_riskRewardOn) // Risk/Reward ratio calculation
					{
						if (this.b_riskRewPips) // Risk/Reward ratio based on PIPs
						{
							dbl_exit1 = Math.Round(Math.Abs(this.dbl_enter - this.dbl_sl) * ratio, curr.Decimals);
						}
						else // Risk/Reward ratio based on amount
						{
							dbl_exit1 = Math.Round(Math.Abs(this.dbl_enter - this.dbl_sl) * ratio * 2, curr.Decimals);
						}
					}
					else // No Risk/Reward ratio calculation
					{
						if (this.b_exit1) // Spread compensate Exit1
						{
							dbl_exit1 = Math.Round(dbl_enter + (i_tradeParam3 + curr.Spread) / piplevel, curr.Decimals);
						}
						else // No spread compensation
						{
							dbl_exit1 = Math.Round(dbl_enter + i_tradeParam3 / piplevel, curr.Decimals);
						}
					}
				}
				else // Down direction
				{
					// Calculate entry point
					dbl_enter = Math.Round(dbl_input2 - i_tradeParam2 / piplevel, curr.Decimals);

					// Calculate Stop/Loss
					if (this.b_stopLoss) // Spread Compensation
					{
						dbl_sl = Math.Round(dbl_enter + (i_tradeParam3 + curr.Spread) / piplevel, curr.Decimals);
					}
					else // No Spread Compensation
					{
						dbl_sl = Math.Round(dbl_enter + i_tradeParam3 / piplevel, curr.Decimals);
					}

					// Calculate Exit1
					if (this.b_riskRewardOn) // Risk/Reward ratio calculation
					{
						if (this.b_riskRewPips) // Risk/Reward ratio based on PIPs
						{
							dbl_exit1 = this.dbl_enter - Math.Round(Math.Abs(this.dbl_enter - this.dbl_sl) * ratio, curr.Decimals);
						}
						else // Risk/Reward ratio based on amount
						{
							dbl_exit1 = this.dbl_enter - Math.Round(Math.Abs(this.dbl_enter - this.dbl_sl) * ratio * 2, curr.Decimals);
						}
					}
					else // No Risk/Reward ratio calculation
					{
						if (this.b_exit1) // Spread compensate Exit1
						{
							dbl_exit1 = Math.Round(dbl_enter - (i_tradeParam3 + curr.Spread) / piplevel, curr.Decimals);
						}
						else // No spread compensation
						{
							dbl_exit1 = Math.Round(dbl_enter - i_tradeParam3 / piplevel, curr.Decimals);
						}
					}
				}

				dbl_exit2 = dbl_input3;
				dbl_exit3 = -1;

				// Calculate visible data
				this.i_tradeParam1 = (int)(dbl_height * piplevel);
				this.dbl_pipLoss = Math.Round(Math.Abs(dbl_enter - dbl_sl) * piplevel * 2, 0);
				this.dbl_pipExit1 = Math.Round(Math.Abs(this.dbl_enter - this.dbl_exit1) * piplevel, 0);
				this.dbl_pipExit2 = Math.Round(Math.Abs(this.dbl_enter - this.dbl_exit2) * piplevel, 0);
				this.dbl_pipExit3 = dbl_exit3;
				this.dbl_pipSubTot = this.dbl_pipExit1 + this.dbl_pipExit2;
				this.dbl_pipTotal = this.dbl_pipSubTot;
				this.dbl_amountLoss = this.dbl_pipLoss * curr.TickValue * this.orderSize;
				this.dbl_amountExit1 = this.dbl_pipExit1 * curr.TickValue * this.orderSize;
				this.dbl_amountExit2 = this.dbl_pipExit2 * curr.TickValue * this.orderSize;
				this.dbl_amountSubTot = this.dbl_amountExit1 + this.dbl_amountExit2;
				this.dbl_amountExit3 = this.dbl_exit3 * curr.TickValue * this.orderSize;
				this.dbl_amountTotal = this.dbl_amountSubTot;
			}
			/// <summary>
			/// Symmetrical triangle breakout calculation 1 leg version.
			/// </summary>
			private void CalculateSymmetricalTriangleBreakoutData()
			{
				// Inputs:	dbl_input1 => Boundry line for the symmetrical triangle
				//			dbl_input2 => ATR(14) value for last completed candle
				//			dbl_input3 => Exit point from trade
				double atr = dbl_input2;
				double ratio = this.rrItems[this.i_riskRewardRatioIndx].GetRatio();
				bool errInput = false;

				// Check Input data
				if (this.dbl_input1 >= this.dbl_input3 && this.b_tradeDirectionUp)
				{
					MessageBox.Show("Trend Line (" + this.dbl_input1.ToString(numFormat) + ") is equal to or greater than Exit1 (" + this.dbl_input3.ToString(numFormat) + ").\r\nTrend Line must be less than Exit1 for an upward breakout.", "Input Inconsistent With Trade!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					errInput = true;
				}
				else if (this.dbl_input1 <= this.dbl_input3 && !this.b_tradeDirectionUp)
				{
					MessageBox.Show("Trend Line (" + this.dbl_input1.ToString(numFormat) + ") is equal to or less than Exit1 (" + this.dbl_input3.ToString(numFormat) + ").\r\nTrend Line must be greater than Exit1 for a downward breakout.", "Input Inconsistent With Trade!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					errInput = true;
				}
				else errInput = false;

				if (!errInput)
				{
					dbl_height = atr;
					i_tradeParam2 = (int)Math.Round(atr * 0.1 * piplevel, 0); // 10% of ATR(14)
					i_tradeParam3 = (int)Math.Round(atr * 0.5 * piplevel, 0); // 50% of ATR(14)

					if (this.b_tradeDirectionUp) // Up direction
					{
						// Calculate entry point
						dbl_enter = dbl_input1 + i_tradeParam2 / piplevel;

						// Calculate Stop/Loss
						if (this.b_stopLoss) // Spread Compensation
						{
							dbl_sl = Math.Round(dbl_enter - (i_tradeParam3 + curr.Spread) / piplevel, curr.Decimals);
						}
						else // No Spread Compensation
						{
							dbl_sl = Math.Round(dbl_enter - i_tradeParam3 / piplevel, curr.Decimals);
						}
					}
					else // Down direction
					{
						// Calculate entry point
						dbl_enter = dbl_input1 - i_tradeParam2 / piplevel;
						// Calculate intemittant values (If not direction independant)

						// Calculate Stop/Loss
						if (this.b_stopLoss) // Spread Compensation
						{
							dbl_sl = Math.Round(dbl_enter + (i_tradeParam3 + curr.Spread) / piplevel, curr.Decimals);
						}
						else // No Spread Compensation
						{
							dbl_sl = Math.Round(dbl_enter + i_tradeParam3 / piplevel, curr.Decimals);
						}
					}

					// Set Exit points
					dbl_exit1 = dbl_input3;
					dbl_exit2 = -1;
					dbl_exit3 = -1;

					// Calculate visible data
					i_tradeParam1 = (int)Math.Round((dbl_height * piplevel), 0);
					this.dbl_pipLoss = Math.Round(Math.Abs(dbl_enter - dbl_sl) * piplevel, 0);
					this.dbl_pipExit1 = Math.Round(Math.Abs(this.dbl_enter - this.dbl_exit1) * piplevel, 0);
					this.dbl_pipExit2 = dbl_exit2;
					this.dbl_pipExit3 = dbl_exit3;
					this.dbl_pipSubTot = this.dbl_pipExit1 + this.dbl_pipExit2;
					this.dbl_pipTotal = this.dbl_pipSubTot;
					this.dbl_amountLoss = this.dbl_pipLoss * curr.TickValue * this.orderSize;
					this.dbl_amountExit1 = this.dbl_pipExit1 * curr.TickValue * this.orderSize;
					this.dbl_amountExit2 = this.dbl_pipExit2 * curr.TickValue * this.orderSize;
					this.dbl_amountSubTot = this.dbl_amountExit1;
					this.dbl_amountExit3 = this.dbl_pipExit3 * curr.TickValue * this.orderSize;
					this.dbl_amountTotal = this.dbl_amountSubTot;
				}
			}
			/// <summary>
			/// Symmetrical triangle breakout calculation 2 leg version.
			/// </summary>
			private void CalculateSymmetricalTriangle2LegBreakoutData()
			{
				double atr = dbl_input2;
				double ratio = this.rrItems[this.i_riskRewardRatioIndx].GetRatio();
				bool errInput = false;

				// Check Input data
				if (this.dbl_input1 >= this.dbl_input3 && this.b_tradeDirectionUp)
				{
					MessageBox.Show("Trend Line (" + this.dbl_input1.ToString(numFormat) + ") is equal to or greater than Exit2 (" + this.dbl_input3.ToString(numFormat) + ").\r\nTrend Line must be less than Exit2 for an upward breakout.", "Input Inconsistent With Trade!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					errInput = true;
				}
				else if (this.dbl_input1 <= this.dbl_input3 && !this.b_tradeDirectionUp)
				{
					MessageBox.Show("Trend Line (" + this.dbl_input1.ToString(numFormat) + ") is equal to or less than Exit2 (" + this.dbl_input3.ToString(numFormat) + ").\r\nTrend Line must be greater than Exit2 for a downward breakout.", "Input Inconsistent With Trade!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					errInput = true;
				}
				else errInput = false;

				if (!errInput)
				{
					dbl_height = atr;
					i_tradeParam2 = (int)Math.Round(atr * 0.5 * piplevel, 0);

					if (this.b_tradeDirectionUp) // Up direction
					{
						// Calculate entry point
						dbl_enter = dbl_input1 + i_tradeParam2 / piplevel;

						// Calculate Stop/Loss
						if (this.b_stopLoss) // Spread Compensation
						{
							dbl_sl = Math.Round(dbl_enter - (i_tradeParam2 + curr.Spread) / piplevel, curr.Decimals);
						}
						else // No Spread Compensation
						{
							dbl_sl = Math.Round(dbl_enter - i_tradeParam2 / piplevel, curr.Decimals);
						}

						// Calculate Exit1
						if (this.b_riskRewardOn) // Risk/Reward ratio calculation
						{
							if (this.b_riskRewPips) // Risk/Reward ratio based on PIPs
							{
								dbl_exit1 = dbl_enter + Math.Round(Math.Abs(this.dbl_enter - this.dbl_sl) * ratio, curr.Decimals);
							}
							else // Risk/Reward ratio based on amount
							{
								dbl_exit1 = dbl_enter + Math.Round(Math.Abs(this.dbl_enter - this.dbl_sl) * ratio * 2, curr.Decimals);
							}
						}
						else // No Risk/Reward ratio calculation
						{
							if (this.b_exit1) // Spread compensate Exit1
							{
								dbl_exit1 = Math.Round(dbl_enter + (i_tradeParam2 + curr.Spread) / piplevel, curr.Decimals);
							}
							else // No spread compensation
							{
								dbl_exit1 = Math.Round(dbl_enter + i_tradeParam2 / piplevel, curr.Decimals);
							}
						}
					}
					else // Down direction
					{
						// Calculate entry point
						dbl_enter = dbl_input1 - i_tradeParam2 / piplevel;
						// Calculate intemittant values (If not direction independant)

						// Calculate Stop/Loss
						if (this.b_stopLoss) // Spread Compensation
						{
							dbl_sl = Math.Round(dbl_enter + (i_tradeParam2 + curr.Spread) / piplevel, curr.Decimals);
						}
						else // No Spread Compensation
						{
							dbl_sl = Math.Round(dbl_enter + i_tradeParam2 / piplevel, curr.Decimals);
						}

						// Calculate Exit1
						if (this.b_riskRewardOn) // Risk/Reward ratio calculation
						{
							if (this.b_riskRewPips) // Risk/Reward ratio based on PIPs
							{
								dbl_exit1 = this.dbl_enter - Math.Round(Math.Abs(this.dbl_enter - this.dbl_sl) * ratio, curr.Decimals);
							}
							else // Risk/Reward ratio based on amount
							{
								dbl_exit1 = this.dbl_enter - Math.Round(Math.Abs(this.dbl_enter - this.dbl_sl) * ratio * 2, curr.Decimals);
							}
						}
						else // No Risk/Reward ratio calculation
						{
							if (this.b_exit1) // Spread compensate Exit1
							{
								dbl_exit1 = Math.Round(dbl_enter - (i_tradeParam2 + curr.Spread) / piplevel, curr.Decimals);
							}
							else // No spread compensation
							{
								dbl_exit1 = Math.Round(dbl_enter - i_tradeParam2 / piplevel, curr.Decimals);
							}
						}
					}

					// Calculate Exit2 and Exit3 points
					dbl_exit2 = dbl_input3;
					dbl_exit3 = -1;

					// Calculate visible data
					i_tradeParam1 = (int)Math.Round((dbl_height * piplevel), 0);
					this.dbl_pipLoss = Math.Round(Math.Abs(dbl_enter - dbl_sl) * piplevel * 2, 0);
					this.dbl_pipExit1 = Math.Round(Math.Abs(this.dbl_enter - this.dbl_exit1) * piplevel, 0);
					this.dbl_pipExit2 = Math.Round(Math.Abs(this.dbl_enter - this.dbl_exit2) * piplevel, 0);
					this.dbl_pipExit3 = dbl_exit3;
					this.dbl_pipSubTot = this.dbl_pipExit1 + this.dbl_pipExit2;
					this.dbl_pipTotal = this.dbl_pipSubTot;
					this.dbl_amountLoss = this.dbl_pipLoss * curr.TickValue * this.orderSize;
					this.dbl_amountExit1 = this.dbl_pipExit1 * curr.TickValue * this.orderSize;
					this.dbl_amountExit2 = this.dbl_pipExit2 * curr.TickValue * this.orderSize;
					this.dbl_amountSubTot = this.dbl_amountExit1 + this.dbl_amountExit2;
					this.dbl_amountExit3 = this.dbl_exit3 * curr.TickValue * this.orderSize;
					this.dbl_amountTotal = this.dbl_amountSubTot;
				}
			}

			#endregion

			// Functions handling data
			#region Data Handling Functions
			/// <summary>
			/// Collects input data from the form.
			/// dbl_input1, dbl_input2, dbl_input3, and ordersize
			/// Sets: currFormat and numFormat
			/// </summary>
			private void GetData()
			{
				// Collect values
				try
				{
					// Collect input_Data1
					if (this.input_Data1.Enabled) this.dbl_input1 = double.Parse(this.input_Data1.Text);
					else dbl_input1 = -1.0;
					// Collect input_Data2
					if (this.input_Data2.Enabled) this.dbl_input2 = double.Parse(this.input_Data2.Text);
					else dbl_input2 = -1.0;
					// Collect input_Data3
					if (this.input_Data3.Enabled) this.dbl_input3 = double.Parse(this.input_Data3.Text);
					else dbl_input3 = -1.0;
					// Collect input_Data4
					if (this.input_Data4.Enabled) this.dbl_input4 = double.Parse(this.input_Data4.Text);
					else dbl_input4 = -1.0;
					// Collect trade input_directionData
					if (this.input_directionData.Enabled)
					{
						if (this.input_directionData.SelectedIndex == 0 || this.input_directionData.SelectedItem.ToString() == "Up")
							this.b_tradeDirectionUp = true;
						else this.b_tradeDirectionUp = false;
					}
					// Collect input_OrderSizeData
					if (this.input_OrderSizeData.SelectedItem != null)
						this.orderSize = double.Parse(this.input_OrderSizeData.SelectedItem.ToString());
					else
						this.orderSize = double.Parse(this.input_OrderSizeData.Text);
					// Collect input_currencyData
					currency.Currencies.GetCurrency(this.input_currencyData.SelectedItem.ToString()).Copy(ref curr);
					// Collect input_equityData
					this.dbl_equity = this.input_equityData.Value;
					// Collect input_stopLossChk
					this.b_stopLoss = this.input_stopLossChk.Checked;
					// Collect input_exitChk
					this.b_exit1 = this.input_exit1Chk.Checked;
					// Collect input_riskRewRatioData
					this.i_riskRewardRatioIndx = this.input_riskRewRatioData.SelectedIndex;
					// Collect input_riskRewPips/input_riskRewAmount (mutually exclusive)
					this.b_riskRewPips = this.input_riskRewPips.Checked;
					// Collect Risk/Reward On switch
					if (this.input_riskRewardOn.Enabled) this.b_riskRewardOn = this.input_riskRewardOn.Checked;
					else this.b_riskRewardOn = false;

				}
				catch (System.ArgumentNullException e)
				{
					MessageBox.Show("Object: " + e.ParamName + " | Message: " + e.Message, "Input Error!", MessageBoxButtons.OK);
				}
				catch (System.FormatException e)
				{
					MessageBox.Show("Message: " + e.Message, "Input Error!", MessageBoxButtons.OK);
				}
				catch (Exception e)
				{
					MessageBox.Show("Input not in correct format! Error message: " + e.Message, "Input Error!", MessageBoxButtons.OK);
				}

				// Set other variables
				piplevel = Math.Pow(10.0, double.Parse(curr.Decimals.ToString()));
				if (curr.Decimals == 2)
				{
					numFormat = "0.00";
				}
				else
				{
					numFormat = "0.0000";
				}

				currFormat = curr.CurrencyPrefix + "#,##0.00";
			}
			private void ResetAllData(object sender, EventArgs e)
			{
				ResetInputData();
				ResetGeneratedData(sender, e);
				ResetOutputVariables();
				this.input_Data1.Focus();
			}
			private void ResetInputData()
			{
				this.input_Data1.ResetText();
				this.input_Data2.ResetText();
				this.input_equityData.ResetText();
				this.input_currencyData.SelectedIndex = defaultIndx_boxCurrency;
				this.input_directionData.SelectedIndex = defaultIndx_direction;
				this.input_OrderSizeData.SelectedIndex = defaultIndx_OrderSize;
				this.input_exit1Chk.Checked = false;
				this.input_riskRewardOn.Checked = false;
				this.input_riskRewPips.Checked = true;
				this.input_riskRewRatioData.SelectedIndex = defaultIndx_riskRewardRatio;
				this.input_stopLossChk.Checked = false;
			}
			private void ResetGeneratedData(object sender, EventArgs e)
			{
				ResetTradeDataGroup(sender, e);
				ResetTradeParameters(sender, e);
				ResetTradeOutcome(sender, e);
				ResetTradeAccount(sender, e);
			}
			private void ResetTradeAccount(object sender, EventArgs e)
			{
				foreach (Object obj in this.Group_tradeAccount.Controls)
				{
					if (obj.GetType() == typeof(TextBox))
					{
						((TextBox)obj).ResetText();
						((TextBox)obj).BackColor = Control.DefaultBackColor;
					}
				}
			}
			private void ResetTradeOutcome(object sender, EventArgs e)
			{
				foreach (Object obj in this.Group_tradeOutcome.Controls)
				{
					if (obj.GetType() == typeof(TextBox))
					{
						((TextBox)obj).ResetText();
					}
				}
			}
			private void ResetTradeParameters(object sender, EventArgs e)
			{
				foreach (Object obj in this.Group_tradeParameters.Controls)
				{
					if (obj.GetType() == typeof(TextBox))
					{
						((TextBox)obj).ResetText();
					}
				}
			}
			private void ResetTradeDataGroup(object sender, EventArgs e)
			{
				foreach (Object obj in this.Group_tradeData.Controls)
				{
					if (obj.GetType() == typeof(TextBox))
					{
						((TextBox)obj).ResetText();
					}
				}
			}
			private void ResetWarning(object sender, EventArgs e)
			{
				this.out_warningText_lbl.Visible = false;
				this.out_warningText.Visible = false;
				this.out_warningText.ResetText();
			}
			private void ReLoadCurrencies()
			{
				currency = new CurrencyDataProvider();
				if (this.input_currencyData.Items.Count > 0)
				{
					this.input_currencyData.Items.Clear();
				}
				LoadCurrencies();
			}
			/// <summary>
			/// Loads the content of the currency provider into the combobox box_currency
			/// and sets the default to index 0.
			/// </summary>
			private void LoadCurrencies()
			{
				int l = currency.Currencies.Length;

				// Add all currencies available in the Currency provider to the combobox.
				for (int i = 0; i < l; i++)
				{
					this.input_currencyData.Items.Add(currency.Currencies.GetCurrency(i).CurrencyPair);
				}
			}
			private void ResetOutputVariables()
			{
				dbl_enter = -1;
				dbl_sl = -1;
				dbl_exit1 = -1;
				dbl_exit2 = -1;
				dbl_exit3 = -1;
				dbl_height = -1;
				dbl_input1 = -1;
				dbl_input2 = -1;
				dbl_input3 = -1;
				dbl_equity = -1;
				dbl_pipLoss = -1;
				dbl_pipExit1 = -1;
				dbl_pipExit2 = -1;
				dbl_pipExit3 = -1;
				dbl_pipSubTot = -1;
				dbl_pipTotal = -1;
				i_tradeParam1 = -1;
				i_tradeParam2 = -1;
				i_tradeParam3 = -1;
				dbl_amountLoss = -1;
				dbl_amountExit1 = -1;
				dbl_amountExit2 = -1;
				dbl_amountExit3 = -1;
				dbl_amountSubTot = -1;
				dbl_amountTotal = -1;
			}

			#endregion

			// Setup functions for the different trade types
			#region Form Handling and Setup Functions
			// Setup Functions
			private void SetupFlagPole()
			{
				// Reset forms
				ResetAllData(new object(), new EventArgs());				// Reset all data
				// Trade Lable Text
				this.lbl_FunctionCall.Text = "Flagpole - Flag / Pennant";	// Set text on form
				// Set trade type
				this.tradeType = TradeMethod.FlagPole;						// Set the trade type to flagpole
				// Tab control setup
				this.TabCntrl.Visible = true;								// Make tab control visible
				this.TabCntrl.Enabled = true;								// Enable tab control
				this.TabCntrl.SelectTab("setUpTab");						// Set the default tab (setUpTab)
				//Input Setup
				this.input_Data1_lbl.Text = "High:";						// Set text on the data1 label
				this.input_Data1.Enabled = true;							// Enable / disable data1 input field
				this.input_Data2_lbl.Text = "Low:";							// Set text on the data2 label
				this.input_Data2.Enabled = true;							// Enable / disable data2 field
				this.input_Data3_lbl.Text = "N/A";							// Set text on the data3 label
				this.input_Data3.Enabled = false;							// Enable / disable data3 field
				this.input_Data3.Text = "N/A";								// Set text on the data3 field
				this.input_Data4_lbl.Text = "N/A";							// Set text on the data4 label
				this.input_Data4.Enabled = false;							// Enable / disable data4 field
				this.input_Data4.Text = "N/A";								// Set text on the data4 field
				this.input_equityData.Enabled = true;						// Enable / disable equitydata field
				this.input_directionData.Enabled = true;					// Enable / disable direction data box
				this.input_OrderSizeData.Enabled = true;					// Enable / disable ordersize data box
				this.input_currencyData.Enabled = true;						// Enable / disable currency data box
				this.input_stopLossChk.Enabled = true;						// Enable / disable stop/loss checkbox
				this.input_exit1Chk.Enabled = true;							// Enable / disable exit1 checkbox
				this.input_riskRewardOn.Enabled = true;						// Enable / disable riskrewardOn checkbox
				this.input_riskRewardOn.Checked = false;					// Uncheck riskrewardOn checkbox
				this.Group_RiskReward.Enabled = false;						// Disable riskreward group
				this.Btn_Calc.Enabled = true;								// Enable calculate button
				// Trade setup outputs
				this.point_entry_lbl.Text = "Entry:";						// Set entry lable text
				this.point_entry.Enabled = true;							// Enable / disable entry field
				this.point_stoploss_lbl.Text = "Stop Loss:";				// Set text in stop loss label
				this.point_stoploss.Enabled = true;							// Enable / disable stoploss field
				this.point_exit1_lbl.Text = "Exit1:";						// Set text in exit2 label
				this.point_exit1.Enabled = true;							// Enable / disable exit1
				this.point_exit2_lbl.Text = "Exit2:";						// Set text in exit3 label
				this.point_exit2.Enabled = true;							// Enable / disable exit2
				this.point_exit3_lbl.Text = "N/A";							// Set text in exit3 label
				this.point_exit3.Enabled = false;							// Enable / disable exit3
				// Trade Parameters outputs
				this.point_param1_lbl.Text = "Height:";						// Set text in param1 lable
				this.point_param1.Enabled = true;							// Enable / disable param1 field
				this.point_param2_lbl.Text = "10%:";						// Set text in param2 label
				this.point_param2.Enabled = true;							// Enable / disable param2 field
				this.point_param3_lbl.Text = "25%:";						// Set text in param3 label
				this.point_param3.Enabled = true;							// Enable / disable param3 field
				// Trade Outcome PIP outputs
				this.pips_exit1.Enabled = true;								// Enable / disable pips_exit1
				this.pips_exit2.Enabled = true;								// Enable / disable pips_exit2
				this.pips_exit3.Enabled = false;							// Enable / disable pips_exit3
				this.pips_subTotal.Enabled = true;							// Enable / disable pips_subtotal
				this.pips_tot.Enabled = true;								// Enable / disable pips_tot
				// Trade Outcome Amount outputs
				this.amount_exit1.Enabled = true;							// Enable / disable amount_exit1
				this.amount_exit2.Enabled = true;							// Enable / disable amount_exit2
				this.amount_exit3.Enabled = false;							// Enable / disable amount_exit3
				this.amount_subTotal.Enabled = true;						// Enable / disable amount_subtotal
				this.amount_tot.Enabled = true;								// Enable / disable amount_total
				// Account outputs
				this.acc_risk.Enabled = true;								// Enable / disable acc_risk
				this.acc_balOnLoss.Enabled = true;							// Enable / disable acc_balonloss
				this.acc_balOnExit1.Enabled = true;							// Enable / disable acc_balonexit1
				this.acc_balOnExit2.Enabled = true;							// Enable / disable acc_balonexit2
				this.acc_balOnExit3.Enabled = false;						// Enable / disable acc_balonexit3
				this.acc_profitPerc.Enabled = true;							// Enable / disable acc_profitperc		
				this.acc_profitLossRatio.Enabled = true;					// Enable / disable acc_profitlossratio
				// Other form element
				this.statusBox_riskRewardOn.Enabled = true;					// Enable / disable status bar riskrewardOn
				this.statusBox_spreadExit1.Enabled = true;					// Enable / disable status bar spread Exit1
				this.statusBox_spreadSL.Enabled = true;						// Enable / disable status bar spread StopLoss
				// Warning box
				this.out_warningText.Visible = false;						// Hide the warning message box
				this.out_warningText_lbl.Visible = false;					// Hide the warning box label
			}
			private void SetupSlingShot()
			{
				// Reset forms
				ResetAllData(new object(), new EventArgs());				// Reset all data
				// Trade Lable Text
				this.lbl_FunctionCall.Text = "Sling Shot Trade";			// Set text on form
				// Set trade type
				this.tradeType = TradeMethod.SlingShot;						// Set the trade type to slingshot
				// Tab control setup
				this.TabCntrl.Visible = true;								// Make tab control visible
				this.TabCntrl.Enabled = true;								// Enable tab control
				this.TabCntrl.SelectTab("setUpTab");						// Set the default tab (setUpTab)
				//Input Setup
				this.input_Data1_lbl.Text = "Level:";						// Set text on the data1 label
				this.input_Data1.Enabled = true;							// Enable / disable data1 input field
				this.input_Data2_lbl.Text = "Exit 2:";						// Set text on the data2 label
				this.input_Data2.Enabled = true;							// Enable / disable data2 field
				this.input_Data3_lbl.Text = "N/A";							// Set text on the data3 label
				this.input_Data3.Enabled = false;							// Enable / disable data3 field
				this.input_Data3.Text = "N/A";								// Set text on the data3 field
				this.input_Data4_lbl.Text = "N/A";							// Set text on the data4 label
				this.input_Data4.Enabled = false;							// Enable / disable data4 field
				this.input_Data4.Text = "N/A";								// Set text on the data4 field
				this.input_equityData.Enabled = true;						// Enable / disable equitydata field
				this.input_directionData.Enabled = false;					// Enable / disable direction data box
				this.input_OrderSizeData.Enabled = true;					// Enable / disable ordersize data box
				this.input_currencyData.Enabled = true;						// Enable / disable currency data box
				this.input_stopLossChk.Enabled = true;						// Enable / disable stop/loss checkbox
				this.input_exit1Chk.Enabled = true;							// Enable / disable exit1 checkbox
				this.input_riskRewardOn.Enabled = true;						// Enable / disable riskrewardOn checkbox
				this.input_riskRewardOn.Checked = false;					// Uncheck riskrewardOn checkbox
				this.Group_RiskReward.Enabled = false;						// Disable riskreward group
				this.Btn_Calc.Enabled = true;								// Enable calculate button
				// Trade setup outputs
				this.point_entry_lbl.Text = "Entry:";						// Set entry lable text
				this.point_entry.Enabled = true;							// Enable / disable entry field
				this.point_stoploss_lbl.Text = "Stop Loss:";				// Set text in stop loss label
				this.point_stoploss.Enabled = true;							// Enable / disable stoploss field
				this.point_exit1_lbl.Text = "Exit1:";						// Set text in exit2 label
				this.point_exit1.Enabled = true;							// Enable / disable exit1
				this.point_exit2_lbl.Text = "Exit2:";						// Set text in exit3 label
				this.point_exit2.Enabled = true;							// Enable / disable exit2
				this.point_exit3_lbl.Text = "N/A";							// Set text in exit3 label
				this.point_exit3.Enabled = false;							// Enable / disable exit3
				// Trade Parameters outputs
				this.point_param1_lbl.Text = "N/A";							// Set text in param1 lable
				this.point_param1.Enabled = false;							// Enable / disable param1 field
				this.point_param1.Text = "N/A";								// Set text in param1 field
				this.point_param2_lbl.Text = "N/A";							// Set text in param2 label
				this.point_param2.Enabled = false;							// Enable / disable param2 field
				this.point_param2.Text = "N/A";								// Set text in param2 field
				this.point_param3_lbl.Text = "N/A";							// Set text in param3 label
				this.point_param3.Enabled = false;							// Enable / disable param3 field
				this.point_param3.Text = "N/A";								// Set text in param3 field
				// Trade Outcome PIP outputs
				this.pips_exit1.Enabled = true;								// Enable / disable pips_exit1
				this.pips_exit2.Enabled = true;								// Enable / disable pips_exit2
				this.pips_exit3.Enabled = false;							// Enable / disable pips_exit3
				this.pips_subTotal.Enabled = true;							// Enable / disable pips_subtotal
				this.pips_tot.Enabled = true;								// Enable / disable pips_tot
				// Trade Outcome Amount outputs
				this.amount_exit1.Enabled = true;							// Enable / disable amount_exit1
				this.amount_exit2.Enabled = true;							// Enable / disable amount_exit2
				this.amount_exit3.Enabled = false;							// Enable / disable amount_exit3
				this.amount_subTotal.Enabled = true;						// Enable / disable amount_subtotal
				this.amount_tot.Enabled = true;								// Enable / disable amount_total
				// Account outputs
				this.acc_risk.Enabled = true;								// Enable / disable acc_risk
				this.acc_balOnLoss.Enabled = true;							// Enable / disable acc_balonloss
				this.acc_balOnExit1.Enabled = true;							// Enable / disable acc_balonexit1
				this.acc_balOnExit2.Enabled = true;							// Enable / disable acc_balonexit2
				this.acc_balOnExit3.Enabled = false;						// Enable / disable acc_balonexit3
				this.acc_profitPerc.Enabled = true;							// Enable / disable acc_profitperc		
				this.acc_profitLossRatio.Enabled = true;					// Enable / disable acc_profitlossratio
				// Other form element
				this.statusBox_riskRewardOn.Enabled = true;					// Enable / disable status bar riskrewardOn
				this.statusBox_spreadExit1.Enabled = true;					// Enable / disable status bar spread Exit1
				this.statusBox_spreadSL.Enabled = true;						// Enable / disable status bar spread StopLoss
				// Warning box
				this.out_warningText.Visible = false;						// Hide the warning message box
				this.out_warningText_lbl.Visible = false;					// Hide the warning box label
			}
			private void SetupEMA10TradeSetup()
			{
				// Reset forms
				ResetAllData(new object(), new EventArgs());				// Reset all data
				// Trade Lable Text
				this.lbl_FunctionCall.Text = "EMA 10 Follower - Setup";		// Set text on form
				// Set trade type
				this.tradeType = TradeMethod.EMA10Setup;					// Set the trade type to EMA10Setup
				// Tab control setup
				this.TabCntrl.Visible = true;								// Make tab control visible
				this.TabCntrl.Enabled = true;								// Enable tab control
				this.TabCntrl.SelectTab("setUpTab");						// Set the default tab (setUpTab)
				//Input Setup
				this.input_Data1_lbl.Text = "EMA(10):";						// Set text on the data1 label
				this.input_Data1.Enabled = true;							// Enable / disable data1 input field
				this.input_Data2_lbl.Text = "ATR(14):";						// Set text on the data2 label
				this.input_Data2.Enabled = true;							// Enable / disable data2 field
				this.input_Data3_lbl.Text = "N/A";							// Set text on the data3 label
				this.input_Data3.Enabled = false;							// Enable / disable data3 field
				this.input_Data3.Text = "N/A";								// Set text on the data3 field
				this.input_Data4_lbl.Text = "N/A";							// Set text on the data4 label
				this.input_Data4.Enabled = false;							// Enable / disable data4 field
				this.input_Data4.Text = "N/A";								// Set text on the data4 field
				this.input_equityData.Enabled = true;						// Enable / disable equitydata field
				this.input_directionData.Enabled = true;					// Enable / disable direction data box
				this.input_OrderSizeData.Enabled = true;					// Enable / disable ordersize data box
				this.input_currencyData.Enabled = true;						// Enable / disable currency data box
				this.input_stopLossChk.Enabled = true;						// Enable / disable stop/loss checkbox
				this.input_exit1Chk.Enabled = false;						// Enable / disable exit1 checkbox
				this.input_riskRewardOn.Enabled = false;					// Enable / disable riskrewardOn checkbox
				this.input_riskRewardOn.Checked = false;					// Uncheck riskrewardOn checkbox
				this.Group_RiskReward.Enabled = false;						// Disable riskreward group
				this.Btn_Calc.Enabled = true;								// Enable calculate button
				// Trade setup outputs
				this.point_entry_lbl.Text = "Entry:";						// Set entry lable text
				this.point_entry.Enabled = true;							// Enable / disable entry field
				this.point_stoploss_lbl.Text = "Stop Loss:";				// Set text in stop loss label
				this.point_stoploss.Enabled = true;							// Enable / disable stoploss field
				this.point_exit1_lbl.Text = "N/A";							// Set text in exit2 label
				this.point_exit1.Enabled = false;							// Enable / disable exit1
				this.point_exit2_lbl.Text = "N/A";							// Set text in exit3 label
				this.point_exit2.Enabled = false;							// Enable / disable exit2
				this.point_exit3_lbl.Text = "N/A";							// Set text in exit3 label
				this.point_exit3.Enabled = false;							// Enable / disable exit3
				// Trade Parameters outputs
				this.point_param1_lbl.Text = "ATR 50%";						// Set text in param1 lable
				this.point_param1.Enabled = true;							// Enable / disable param1 field
				this.point_param1.Text = "";								// Set text in param1 field
				this.point_param2_lbl.Text = "N/A";							// Set text in param2 label
				this.point_param2.Enabled = false;							// Enable / disable param2 field
				this.point_param2.Text = "N/A";								// Set text in param2 field
				this.point_param3_lbl.Text = "N/A";							// Set text in param3 label
				this.point_param3.Enabled = false;							// Enable / disable param3 field
				this.point_param3.Text = "N/A";								// Set text in param3 field
				// Trade Outcome PIP outputs
				this.pips_exit1.Enabled = false;							// Enable / disable pips_exit1
				this.pips_exit2.Enabled = false;							// Enable / disable pips_exit2
				this.pips_exit3.Enabled = false;							// Enable / disable pips_exit3
				this.pips_subTotal.Enabled = false;							// Enable / disable pips_subtotal
				this.pips_tot.Enabled = false;								// Enable / disable pips_tot
				// Trade Outcome Amount outputs
				this.amount_exit1.Enabled = false;							// Enable / disable amount_exit1
				this.amount_exit2.Enabled = false;							// Enable / disable amount_exit2
				this.amount_exit3.Enabled = false;							// Enable / disable amount_exit3
				this.amount_subTotal.Enabled = false;						// Enable / disable amount_subtotal
				this.amount_tot.Enabled = false;							// Enable / disable amount_total
				// Account outputs
				this.acc_risk.Enabled = true;								// Enable / disable acc_risk
				this.acc_balOnLoss.Enabled = true;							// Enable / disable acc_balonloss
				this.acc_balOnExit1.Enabled = false;						// Enable / disable acc_balonexit1
				this.acc_balOnExit2.Enabled = false;						// Enable / disable acc_balonexit2
				this.acc_balOnExit3.Enabled = false;						// Enable / disable acc_balonexit3
				this.acc_profitPerc.Enabled = false;						// Enable / disable acc_profitperc		
				this.acc_profitLossRatio.Enabled = false;					// Enable / disable acc_profitlossratio
				// Other form element
				this.statusBox_riskRewardOn.Enabled = false;				// Enable / disable status bar riskrewardOn
				this.statusBox_spreadExit1.Enabled = false;					// Enable / disable status bar spread Exit1
				this.statusBox_spreadSL.Enabled = true;						// Enable / disable status bar spread StopLoss
				// Warning box
				this.out_warningText.Visible = false;						// Hide the warning message box
				this.out_warningText_lbl.Visible = false;					// Hide the warning box label
			}
			private void SetupEMA10TradeMaintenance()
			{
				// Reset forms
				ResetAllData(new object(), new EventArgs());				// Reset all data
				// Trade Lable Text
				this.lbl_FunctionCall.Text = "EMA 10 Follower - Maintanance";// Set text on form
				// Set trade type
				this.tradeType = TradeMethod.EMA10Maintain;					// Set the trade type to EMA10Maintain
				// Tab control setup
				this.TabCntrl.Visible = true;								// Make tab control visible
				this.TabCntrl.Enabled = true;								// Enable tab control
				this.TabCntrl.SelectTab("setUpTab");						// Set the default tab (setUpTab)
				//Input Setup
				this.input_Data1_lbl.Text = "EMA(10):";						// Set text on the data1 label
				this.input_Data1.Enabled = true;							// Enable / disable data1 input field
				this.input_Data2_lbl.Text = "ATR(14):";						// Set text on the data2 label
				this.input_Data2.Enabled = true;							// Enable / disable data2 field
				this.input_Data3_lbl.Text = "Entry:";						// Set text on the data3 label
				this.input_Data3.Enabled = true;							// Enable / disable data3 field
				this.input_Data3.Text = "";									// Set text on the data3 field
				this.input_Data4_lbl.Text = "N/A";							// Set text on the data4 label
				this.input_Data4.Enabled = false;							// Enable / disable data4 field
				this.input_Data4.Text = "N/A";								// Set text on the data4 field
				this.input_equityData.Enabled = true;						// Enable / disable equitydata field
				this.input_directionData.Enabled = true;					// Enable / disable direction data box
				this.input_OrderSizeData.Enabled = true;					// Enable / disable ordersize data box
				this.input_currencyData.Enabled = true;						// Enable / disable currency data box
				this.input_stopLossChk.Enabled = true;						// Enable / disable stop/loss checkbox
				this.input_exit1Chk.Enabled = false;						// Enable / disable exit1 checkbox
				this.input_riskRewardOn.Enabled = false;					// Enable / disable riskrewardOn checkbox
				this.input_riskRewardOn.Checked = false;					// Uncheck riskrewardOn checkbox
				this.Group_RiskReward.Enabled = false;						// Disable riskreward group
				this.Btn_Calc.Enabled = true;								// Enable calculate button
				// Trade setup outputs
				this.point_entry_lbl.Text = "Entry:";						// Set entry lable text
				this.point_entry.Enabled = true;							// Enable / disable entry field
				this.point_stoploss_lbl.Text = "New S/L point:";			// Set text in stop loss label
				this.point_stoploss.Enabled = true;							// Enable / disable stoploss field
				this.point_exit1_lbl.Text = "N/A";							// Set text in exit2 label
				this.point_exit1.Enabled = false;							// Enable / disable exit1
				this.point_exit2_lbl.Text = "N/A";							// Set text in exit3 label
				this.point_exit2.Enabled = false;							// Enable / disable exit2
				this.point_exit3_lbl.Text = "N/A";							// Set text in exit3 label
				this.point_exit3.Enabled = false;							// Enable / disable exit3
				// Trade Parameters outputs
				this.point_param1_lbl.Text = "ATR 50%";							// Set text in param1 lable
				this.point_param1.Enabled = true;							// Enable / disable param1 field
				this.point_param1.Text = "";								// Set text in param1 field
				this.point_param2_lbl.Text = "N/A";							// Set text in param2 label
				this.point_param2.Enabled = false;							// Enable / disable param2 field
				this.point_param2.Text = "N/A";								// Set text in param2 field
				this.point_param3_lbl.Text = "N/A";							// Set text in param3 label
				this.point_param3.Enabled = false;							// Enable / disable param3 field
				this.point_param3.Text = "N/A";								// Set text in param3 field
				// Trade Outcome PIP outputs
				this.pips_exit1.Enabled = true;								// Enable / disable pips_exit1
				this.pips_exit2.Enabled = true;								// Enable / disable pips_exit2
				this.pips_exit3.Enabled = false;							// Enable / disable pips_exit3
				this.pips_subTotal.Enabled = true;							// Enable / disable pips_subtotal
				this.pips_tot.Enabled = true;								// Enable / disable pips_tot
				// Trade Outcome Amount outputs
				this.amount_exit1.Enabled = true;							// Enable / disable amount_exit1
				this.amount_exit2.Enabled = true;							// Enable / disable amount_exit2
				this.amount_exit3.Enabled = false;							// Enable / disable amount_exit3
				this.amount_subTotal.Enabled = true;						// Enable / disable amount_subtotal
				this.amount_tot.Enabled = true;								// Enable / disable amount_total
				// Account outputs
				this.acc_risk.Enabled = true;								// Enable / disable acc_risk
				this.acc_balOnLoss.Enabled = true;							// Enable / disable acc_balonloss
				this.acc_balOnExit1.Enabled = true;							// Enable / disable acc_balonexit1
				this.acc_balOnExit2.Enabled = true;							// Enable / disable acc_balonexit2
				this.acc_balOnExit3.Enabled = false;						// Enable / disable acc_balonexit3
				this.acc_profitPerc.Enabled = true;							// Enable / disable acc_profitperc		
				this.acc_profitLossRatio.Enabled = true;					// Enable / disable acc_profitlossratio
				// Other form element
				this.statusBox_riskRewardOn.Enabled = false;					// Enable / disable status bar riskrewardOn
				this.statusBox_spreadExit1.Enabled = false;					// Enable / disable status bar spread Exit1
				this.statusBox_spreadSL.Enabled = true;						// Enable / disable status bar spread StopLoss
				// Warning box
				this.out_warningText.Visible = false;						// Hide the warning message box
				this.out_warningText_lbl.Visible = false;					// Hide the warning box label
			}
			private void SetupSymmTriangleBreakout()
			{
				// Reset forms
				ResetAllData(new object(), new EventArgs());				// Reset all data
				// Trade Lable Text
				this.lbl_FunctionCall.Text = "Symmetrical Triangle Breakout - DEMO ONLY";// Set text on form
				// Set trade type
				this.tradeType = TradeMethod.SymmTriBreak;					// Set the trade type to Symmetrical triangle
				// Tab control setup
				this.TabCntrl.Visible = true;								// Make tab control visible
				this.TabCntrl.Enabled = true;								// Enable tab control
				this.TabCntrl.SelectTab("setUpTab");						// Set the default tab (setUpTab)
				//Input Setup
				this.input_Data1_lbl.Text = "Trend Line:";					// Set text on the data1 label
				this.input_Data1.Enabled = true;							// Enable / disable data1 input field
				this.input_Data2_lbl.Text = "ATR(14):";						// Set text on the data2 label
				this.input_Data2.Enabled = true;							// Enable / disable data2 field
				this.input_Data3_lbl.Text = "Exit1:";						// Set text on the data3 label
				this.input_Data3.Enabled = true;							// Enable / disable data3 field
				this.input_Data3.Text = "";									// Set text on the data3 field
				this.input_Data4_lbl.Text = "N/A";							// Set text on the data4 label
				this.input_Data4.Enabled = false;							// Enable / disable data4 field
				this.input_Data4.Text = "N/A";								// Set text on the data4 field
				this.input_equityData.Enabled = true;						// Enable / disable equitydata field
				this.input_directionData.Enabled = true;					// Enable / disable direction data box
				this.input_OrderSizeData.Enabled = true;					// Enable / disable ordersize data box
				this.input_currencyData.Enabled = true;						// Enable / disable currency data box
				this.input_stopLossChk.Enabled = true;						// Enable / disable stop/loss checkbox
				this.input_exit1Chk.Enabled = false;						// Enable / disable exit1 checkbox
				this.input_riskRewardOn.Enabled = false;					// Enable / disable riskrewardOn checkbox
				this.input_riskRewardOn.Checked = false;					// Uncheck riskrewardOn checkbox
				this.Group_RiskReward.Enabled = false;						// Disable riskreward group
				this.Btn_Calc.Enabled = true;								// Enable calculate button
				// Trade setup outputs
				this.point_entry_lbl.Text = "Entry:";						// Set entry lable text
				this.point_entry.Enabled = true;							// Enable / disable entry field
				this.point_stoploss_lbl.Text = "Stop Loss:";				// Set text in stop loss label
				this.point_stoploss.Enabled = true;							// Enable / disable stoploss field
				this.point_exit1_lbl.Text = "Exit1:";						// Set text in exit2 label
				this.point_exit1.Enabled = true;							// Enable / disable exit1
				this.point_exit2_lbl.Text = "N/A";							// Set text in exit3 label
				this.point_exit2.Enabled = false;							// Enable / disable exit2
				this.point_exit3_lbl.Text = "N/A";							// Set text in exit3 label
				this.point_exit3.Enabled = false;							// Enable / disable exit3
				// Trade Parameters outputs
				this.point_param1_lbl.Text = "Height:";						// Set text in param1 lable
				this.point_param1.Enabled = true;							// Enable / disable param1 field
				this.point_param1.Text = "";								// Set text in param1 field
				this.point_param2_lbl.Text = "ATR 10%:";					// Set text in param2 label
				this.point_param2.Enabled = true;							// Enable / disable param2 field
				this.point_param2.Text = "";								// Set text in param2 field
				this.point_param3_lbl.Text = "ATR 50%:";					// Set text in param3 label
				this.point_param3.Enabled = true;							// Enable / disable param3 field
				this.point_param3.Text = "";								// Set text in param3 field
				// Trade Outcome PIP outputs
				this.pips_exit1.Enabled = true;								// Enable / disable pips_exit1
				this.pips_exit2.Enabled = false;							// Enable / disable pips_exit2
				this.pips_exit3.Enabled = false;							// Enable / disable pips_exit3
				this.pips_subTotal.Enabled = true;							// Enable / disable pips_subtotal
				this.pips_tot.Enabled = true;								// Enable / disable pips_tot
				// Trade Outcome Amount outputs
				this.amount_exit1.Enabled = true;							// Enable / disable amount_exit1
				this.amount_exit2.Enabled = false;							// Enable / disable amount_exit2
				this.amount_exit3.Enabled = false;							// Enable / disable amount_exit3
				this.amount_subTotal.Enabled = true;						// Enable / disable amount_subtotal
				this.amount_tot.Enabled = true;								// Enable / disable amount_total
				// Account outputs
				this.acc_risk.Enabled = true;								// Enable / disable acc_risk
				this.acc_balOnLoss.Enabled = true;							// Enable / disable acc_balonloss
				this.acc_balOnExit1.Enabled = true;							// Enable / disable acc_balonexit1
				this.acc_balOnExit2.Enabled = false;						// Enable / disable acc_balonexit2
				this.acc_balOnExit3.Enabled = false;						// Enable / disable acc_balonexit3
				this.acc_profitPerc.Enabled = true;							// Enable / disable acc_profitperc		
				this.acc_profitLossRatio.Enabled = true;					// Enable / disable acc_profitlossratio
				// Other form element
				this.statusBox_riskRewardOn.Enabled = true;					// Enable / disable status bar riskrewardOn
				this.statusBox_spreadExit1.Enabled = true;					// Enable / disable status bar spread Exit1
				this.statusBox_spreadSL.Enabled = true;						// Enable / disable status bar spread StopLoss
				// Warning box
				this.out_warningText.Visible = false;						// Hide the warning message box
				this.out_warningText_lbl.Visible = false;					// Hide the warning box label
			}
			private void SetupSymmTriangle2LegBreakout()
			{
				// Reset forms
				ResetAllData(new object(), new EventArgs());				// Reset all data
				// Trade Lable Text
				this.lbl_FunctionCall.Text = "Symmetrical Triangle Breakout 2 Leg - DEMO ONLY";// Set text on form
				// Set trade type
				this.tradeType = TradeMethod.SymmTriBreak;					// Set the trade type to Symmetrical triangle
				// Tab control setup
				this.TabCntrl.Visible = true;								// Make tab control visible
				this.TabCntrl.Enabled = true;								// Enable tab control
				this.TabCntrl.SelectTab("setUpTab");						// Set the default tab (setUpTab)
				//Input Setup
				this.input_Data1_lbl.Text = "Trend Line:";					// Set text on the data1 label
				this.input_Data1.Enabled = true;							// Enable / disable data1 input field
				this.input_Data2_lbl.Text = "ATR(14):";						// Set text on the data2 label
				this.input_Data2.Enabled = true;							// Enable / disable data2 field
				this.input_Data3_lbl.Text = "Exit1:";						// Set text on the data3 label
				this.input_Data3.Enabled = true;							// Enable / disable data3 field
				this.input_Data3.Text = "";									// Set text on the data3 field
				this.input_Data4_lbl.Text = "Exit2:";						// Set text on the data4 label
				this.input_Data4.Enabled = true;							// Enable / disable data4 field
				this.input_Data4.Text = "";									// Set text on the data4 field
				this.input_equityData.Enabled = true;						// Enable / disable equitydata field
				this.input_directionData.Enabled = true;					// Enable / disable direction data box
				this.input_OrderSizeData.Enabled = true;					// Enable / disable ordersize data box
				this.input_currencyData.Enabled = true;						// Enable / disable currency data box
				this.input_stopLossChk.Enabled = true;						// Enable / disable stop/loss checkbox
				this.input_exit1Chk.Enabled = true;							// Enable / disable exit1 checkbox
				this.input_riskRewardOn.Enabled = true;						// Enable / disable riskrewardOn checkbox
				this.input_riskRewardOn.Checked = false;					// Uncheck riskrewardOn checkbox
				this.Group_RiskReward.Enabled = false;						// Disable riskreward group
				this.Btn_Calc.Enabled = true;								// Enable calculate button
				// Trade setup outputs
				this.point_entry_lbl.Text = "Entry:";						// Set entry lable text
				this.point_entry.Enabled = true;							// Enable / disable entry field
				this.point_stoploss_lbl.Text = "Stop Loss:";				// Set text in stop loss label
				this.point_stoploss.Enabled = true;							// Enable / disable stoploss field
				this.point_exit1_lbl.Text = "Exit1:";						// Set text in exit2 label
				this.point_exit1.Enabled = true;							// Enable / disable exit1
				this.point_exit2_lbl.Text = "Exit2:";						// Set text in exit3 label
				this.point_exit2.Enabled = true;							// Enable / disable exit2
				this.point_exit3_lbl.Text = "N/A";							// Set text in exit3 label
				this.point_exit3.Enabled = false;							// Enable / disable exit3
				// Trade Parameters outputs
				this.point_param1_lbl.Text = "Height:";					// Set text in param1 lable
				this.point_param1.Enabled = true;							// Enable / disable param1 field
				this.point_param1.Text = "";								// Set text in param1 field
				this.point_param2_lbl.Text = "ATR 50%:";							// Set text in param2 label
				this.point_param2.Enabled = true;							// Enable / disable param2 field
				this.point_param2.Text = "";								// Set text in param2 field
				this.point_param3_lbl.Text = "N/A";							// Set text in param3 label
				this.point_param3.Enabled = false;							// Enable / disable param3 field
				this.point_param3.Text = "N/A";								// Set text in param3 field
				// Trade Outcome PIP outputs
				this.pips_exit1.Enabled = true;								// Enable / disable pips_exit1
				this.pips_exit2.Enabled = true;								// Enable / disable pips_exit2
				this.pips_exit3.Enabled = false;							// Enable / disable pips_exit3
				this.pips_subTotal.Enabled = true;							// Enable / disable pips_subtotal
				this.pips_tot.Enabled = true;								// Enable / disable pips_tot
				// Trade Outcome Amount outputs
				this.amount_exit1.Enabled = true;							// Enable / disable amount_exit1
				this.amount_exit2.Enabled = true;							// Enable / disable amount_exit2
				this.amount_exit3.Enabled = false;							// Enable / disable amount_exit3
				this.amount_subTotal.Enabled = true;						// Enable / disable amount_subtotal
				this.amount_tot.Enabled = true;								// Enable / disable amount_total
				// Account outputs
				this.acc_risk.Enabled = true;								// Enable / disable acc_risk
				this.acc_balOnLoss.Enabled = true;							// Enable / disable acc_balonloss
				this.acc_balOnExit1.Enabled = true;							// Enable / disable acc_balonexit1
				this.acc_balOnExit2.Enabled = true;							// Enable / disable acc_balonexit2
				this.acc_balOnExit3.Enabled = false;						// Enable / disable acc_balonexit3
				this.acc_profitPerc.Enabled = true;							// Enable / disable acc_profitperc		
				this.acc_profitLossRatio.Enabled = true;					// Enable / disable acc_profitlossratio
				// Other form element
				this.statusBox_riskRewardOn.Enabled = true;					// Enable / disable status bar riskrewardOn
				this.statusBox_spreadExit1.Enabled = true;					// Enable / disable status bar spread Exit1
				this.statusBox_spreadSL.Enabled = true;						// Enable / disable status bar spread StopLoss
				// Warning box
				this.out_warningText.Visible = false;						// Hide the warning message box
				this.out_warningText_lbl.Visible = false;					// Hide the warning box label
			}
			private void SetupHorizontalRangeBreakout()
			{
				// Reset forms
				ResetAllData(new object(), new EventArgs());				// Reset all data
				// Trade Lable Text
				this.lbl_FunctionCall.Text = "Horrizontal Line Breakout - DEMO ONLY";// Set text on form
				// Set trade type
				this.tradeType = TradeMethod.HorrLineBreak;					// Set the trade type to Horrizontal line breakout
				// Tab control setup
				this.TabCntrl.Visible = true;								// Make tab control visible
				this.TabCntrl.Enabled = true;								// Enable tab control
				this.TabCntrl.SelectTab("setUpTab");						// Set the default tab (setUpTab)
				//Input Setup
				this.input_Data1_lbl.Text = "High Line:";					// Set text on the data1 label
				this.input_Data1.Enabled = true;							// Enable / disable data1 input field
				this.input_Data2_lbl.Text = "Low Line:";					// Set text on the data2 label
				this.input_Data2.Enabled = true;							// Enable / disable data2 field
				this.input_Data3_lbl.Text = "Exit2:";						// Set text on the data3 label
				this.input_Data3.Enabled = true;							// Enable / disable data3 field
				this.input_Data3.Text = "";									// Set text on the data3 field
				this.input_Data4_lbl.Text = "N/A";							// Set text on the data4 label
				this.input_Data4.Enabled = false;							// Enable / disable data4 field
				this.input_Data4.Text = "N/A";								// Set text on the data4 field
				this.input_equityData.Enabled = true;						// Enable / disable equitydata field
				this.input_directionData.Enabled = true;					// Enable / disable direction data box
				this.input_OrderSizeData.Enabled = true;					// Enable / disable ordersize data box
				this.input_currencyData.Enabled = true;						// Enable / disable currency data box
				this.input_stopLossChk.Enabled = true;						// Enable / disable stop/loss checkbox
				this.input_exit1Chk.Enabled = true;							// Enable / disable exit1 checkbox
				this.input_riskRewardOn.Enabled = true;						// Enable / disable riskrewardOn checkbox
				this.input_riskRewardOn.Checked = false;					// Uncheck riskrewardOn checkbox
				this.Group_RiskReward.Enabled = false;						// Disable riskreward group
				this.Btn_Calc.Enabled = true;								// Enable calculate button
				// Trade setup outputs
				this.point_entry_lbl.Text = "Entry:";						// Set entry lable text
				this.point_entry.Enabled = true;							// Enable / disable entry field
				this.point_stoploss_lbl.Text = "Stop Loss:";				// Set text in stop loss label
				this.point_stoploss.Enabled = true;							// Enable / disable stoploss field
				this.point_exit1_lbl.Text = "Exit1:";						// Set text in exit2 label
				this.point_exit1.Enabled = true;							// Enable / disable exit1
				this.point_exit2_lbl.Text = "Exit2:";						// Set text in exit3 label
				this.point_exit2.Enabled = true;							// Enable / disable exit2
				this.point_exit3_lbl.Text = "N/A";							// Set text in exit3 label
				this.point_exit3.Enabled = false;							// Enable / disable exit3
				// Trade Parameters outputs
				this.point_param1_lbl.Text = "Height:";						// Set text in param1 lable
				this.point_param1.Enabled = true;							// Enable / disable param1 field
				this.point_param1.Text = "";								// Set text in param1 field
				this.point_param2_lbl.Text = "25%:";						// Set text in param2 label
				this.point_param2.Enabled = true;							// Enable / disable param2 field
				this.point_param2.Text = "";								// Set text in param2 field
				this.point_param3_lbl.Text = "50%:";						// Set text in param3 label
				this.point_param3.Enabled = true;							// Enable / disable param3 field
				this.point_param3.Text = "";								// Set text in param3 field
				// Trade Outcome PIP outputs
				this.pips_exit1.Enabled = true;								// Enable / disable pips_exit1
				this.pips_exit2.Enabled = true;								// Enable / disable pips_exit2
				this.pips_exit3.Enabled = false;							// Enable / disable pips_exit3
				this.pips_subTotal.Enabled = true;							// Enable / disable pips_subtotal
				this.pips_tot.Enabled = true;								// Enable / disable pips_tot
				// Trade Outcome Amount outputs
				this.amount_exit1.Enabled = true;							// Enable / disable amount_exit1
				this.amount_exit2.Enabled = true;							// Enable / disable amount_exit2
				this.amount_exit3.Enabled = false;							// Enable / disable amount_exit3
				this.amount_subTotal.Enabled = true;						// Enable / disable amount_subtotal
				this.amount_tot.Enabled = true;								// Enable / disable amount_total
				// Account outputs
				this.acc_risk.Enabled = true;								// Enable / disable acc_risk
				this.acc_balOnLoss.Enabled = true;							// Enable / disable acc_balonloss
				this.acc_balOnExit1.Enabled = true;							// Enable / disable acc_balonexit1
				this.acc_balOnExit2.Enabled = true;							// Enable / disable acc_balonexit2
				this.acc_balOnExit3.Enabled = false;						// Enable / disable acc_balonexit3
				this.acc_profitPerc.Enabled = true;							// Enable / disable acc_profitperc		
				this.acc_profitLossRatio.Enabled = true;					// Enable / disable acc_profitlossratio
				// Other form element
				this.statusBox_riskRewardOn.Enabled = true;					// Enable / disable status bar riskrewardOn
				this.statusBox_spreadExit1.Enabled = true;					// Enable / disable status bar spread Exit1
				this.statusBox_spreadSL.Enabled = true;						// Enable / disable status bar spread StopLoss
				// Warning box
				this.out_warningText.Visible = false;						// Hide the warning message box
				this.out_warningText_lbl.Visible = false;					// Hide the warning box label
			}
			private void SetupAscendingTriangleBreakout()
			{
				// Reset forms
				ResetAllData(new object(), new EventArgs());				// Reset all data
				// Trade Lable Text
				this.lbl_FunctionCall.Text = "Ascending Triangle Breakout - DEMO ONLY";// Set text on form
				// Set trade type
				this.tradeType = TradeMethod.AscendingTriangle;				// Set the trade type to Ascending triangle
				// Tab control setup
				this.TabCntrl.Visible = true;								// Make tab control visible
				this.TabCntrl.Enabled = true;								// Enable tab control
				this.TabCntrl.SelectTab("setUpTab");						// Set the default tab (setUpTab)
				//Input Setup
				this.input_Data1_lbl.Text = "Trend Line:";					// Set text on the data1 label
				this.input_Data1.Enabled = true;							// Enable / disable data1 input field
				this.input_Data2_lbl.Text = "ATR(14):";						// Set text on the data2 label
				this.input_Data2.Enabled = true;							// Enable / disable data2 field
				this.input_Data3_lbl.Text = "Exit2:";							// Set text on the data3 label
				this.input_Data3.Enabled = true;							// Enable / disable data3 field
				this.input_Data3.Text = "";									// Set text on the data3 field
				this.input_Data4_lbl.Text = "N/A";							// Set text on the data4 label
				this.input_Data4.Enabled = false;							// Enable / disable data4 field
				this.input_Data4.Text = "N/A";								// Set text on the data4 field
				this.input_equityData.Enabled = true;						// Enable / disable equitydata field
				this.input_directionData.Enabled = false;					// Enable / disable direction data box
				this.input_directionData.SelectedIndex = 0;					// Set indication to up
				this.input_OrderSizeData.Enabled = true;					// Enable / disable ordersize data box
				this.input_currencyData.Enabled = true;						// Enable / disable currency data box
				this.input_stopLossChk.Enabled = true;						// Enable / disable stop/loss checkbox
				this.input_exit1Chk.Enabled = true;							// Enable / disable exit1 checkbox
				this.input_riskRewardOn.Enabled = true;						// Enable / disable riskrewardOn checkbox
				this.input_riskRewardOn.Checked = false;					// Uncheck riskrewardOn checkbox
				this.Group_RiskReward.Enabled = false;						// Disable riskreward group
				this.Btn_Calc.Enabled = true;								// Enable calculate button
				// Trade setup outputs
				this.point_entry_lbl.Text = "Entry:";						// Set entry lable text
				this.point_entry.Enabled = true;							// Enable / disable entry field
				this.point_stoploss_lbl.Text = "Stop Loss:";				// Set text in stop loss label
				this.point_stoploss.Enabled = true;							// Enable / disable stoploss field
				this.point_exit1_lbl.Text = "Exit1:";						// Set text in exit2 label
				this.point_exit1.Enabled = true;							// Enable / disable exit1
				this.point_exit2_lbl.Text = "Exit2:";						// Set text in exit3 label
				this.point_exit2.Enabled = true;							// Enable / disable exit2
				this.point_exit3_lbl.Text = "N/A";							// Set text in exit3 label
				this.point_exit3.Enabled = false;							// Enable / disable exit3
				// Trade Parameters outputs
				this.point_param1_lbl.Text = "ATR 50%:";					// Set text in param1 lable
				this.point_param1.Enabled = true;							// Enable / disable param1 field
				this.point_param1.Text = "";								// Set text in param1 field
				this.point_param2_lbl.Text = "N/A";							// Set text in param2 label
				this.point_param2.Enabled = false;							// Enable / disable param2 field
				this.point_param2.Text = "N/A";								// Set text in param2 field
				this.point_param3_lbl.Text = "N/A";							// Set text in param3 label
				this.point_param3.Enabled = false;							// Enable / disable param3 field
				this.point_param3.Text = "N/A";								// Set text in param3 field
				// Trade Outcome PIP outputs
				this.pips_exit1.Enabled = true;								// Enable / disable pips_exit1
				this.pips_exit2.Enabled = true;								// Enable / disable pips_exit2
				this.pips_exit3.Enabled = false;							// Enable / disable pips_exit3
				this.pips_subTotal.Enabled = true;							// Enable / disable pips_subtotal
				this.pips_tot.Enabled = true;								// Enable / disable pips_tot
				// Trade Outcome Amount outputs
				this.amount_exit1.Enabled = true;							// Enable / disable amount_exit1
				this.amount_exit2.Enabled = true;							// Enable / disable amount_exit2
				this.amount_exit3.Enabled = false;							// Enable / disable amount_exit3
				this.amount_subTotal.Enabled = true;						// Enable / disable amount_subtotal
				this.amount_tot.Enabled = true;								// Enable / disable amount_total
				// Account outputs
				this.acc_risk.Enabled = true;								// Enable / disable acc_risk
				this.acc_balOnLoss.Enabled = true;							// Enable / disable acc_balonloss
				this.acc_balOnExit1.Enabled = true;							// Enable / disable acc_balonexit1
				this.acc_balOnExit2.Enabled = true;							// Enable / disable acc_balonexit2
				this.acc_balOnExit3.Enabled = false;						// Enable / disable acc_balonexit3
				this.acc_profitPerc.Enabled = true;							// Enable / disable acc_profitperc		
				this.acc_profitLossRatio.Enabled = true;					// Enable / disable acc_profitlossratio
				// Other form element
				this.statusBox_riskRewardOn.Enabled = true;					// Enable / disable status bar riskrewardOn
				this.statusBox_spreadExit1.Enabled = true;					// Enable / disable status bar spread Exit1
				this.statusBox_spreadSL.Enabled = true;						// Enable / disable status bar spread StopLoss
				// Warning box
				this.out_warningText.Visible = false;						// Hide the warning message box
				this.out_warningText_lbl.Visible = false;					// Hide the warning box label
			}
			private void SetupDescendingTriangleBreakout()
			{
				// Reset forms
				ResetAllData(new object(), new EventArgs());				// Reset all data
				// Trade Lable Text
				this.lbl_FunctionCall.Text = "Descending Triangle Breakout - DEMO ONLY";// Set text on form
				// Set trade type
				this.tradeType = TradeMethod.DescendingTriangle;			// Set the trade type to Descending triangle
				// Tab control setup
				this.TabCntrl.Visible = true;								// Make tab control visible
				this.TabCntrl.Enabled = true;								// Enable tab control
				this.TabCntrl.SelectTab("setUpTab");						// Set the default tab (setUpTab)
				//Input Setup
				this.input_Data1_lbl.Text = "Trend Line:";					// Set text on the data1 label
				this.input_Data1.Enabled = true;							// Enable / disable data1 input field
				this.input_Data2_lbl.Text = "ATR(14):";						// Set text on the data2 label
				this.input_Data2.Enabled = true;							// Enable / disable data2 field
				this.input_Data3_lbl.Text = "Exit2:";						// Set text on the data3 label
				this.input_Data3.Enabled = true;							// Enable / disable data3 field
				this.input_Data3.Text = "";									// Set text on the data3 field
				this.input_Data4_lbl.Text = "N/A";							// Set text on the data4 label
				this.input_Data4.Enabled = false;							// Enable / disable data4 field
				this.input_Data4.Text = "N/A";								// Set text on the data4 field
				this.input_equityData.Enabled = true;						// Enable / disable equitydata field
				this.input_directionData.Enabled = false;					// Enable / disable direction data box
				this.input_directionData.SelectedIndex = 1;					// Set to show down direction
				this.input_OrderSizeData.Enabled = true;					// Enable / disable ordersize data box
				this.input_currencyData.Enabled = true;						// Enable / disable currency data box
				this.input_stopLossChk.Enabled = true;						// Enable / disable stop/loss checkbox
				this.input_exit1Chk.Enabled = true;							// Enable / disable exit1 checkbox
				this.input_riskRewardOn.Enabled = true;						// Enable / disable riskrewardOn checkbox
				this.input_riskRewardOn.Checked = false;					// Uncheck riskrewardOn checkbox
				this.Group_RiskReward.Enabled = false;						// Disable riskreward group
				this.Btn_Calc.Enabled = true;								// Enable calculate button
				// Trade setup outputs
				this.point_entry_lbl.Text = "Entry:";						// Set entry lable text
				this.point_entry.Enabled = true;							// Enable / disable entry field
				this.point_stoploss_lbl.Text = "Stop Loss:";				// Set text in stop loss label
				this.point_stoploss.Enabled = true;							// Enable / disable stoploss field
				this.point_exit1_lbl.Text = "Exit1:";						// Set text in exit2 label
				this.point_exit1.Enabled = true;							// Enable / disable exit1
				this.point_exit2_lbl.Text = "Exit2:";						// Set text in exit3 label
				this.point_exit2.Enabled = true;							// Enable / disable exit2
				this.point_exit3_lbl.Text = "N/A";							// Set text in exit3 label
				this.point_exit3.Enabled = false;							// Enable / disable exit3
				// Trade Parameters outputs
				this.point_param1_lbl.Text = "ATR 50%:";					// Set text in param1 lable
				this.point_param1.Enabled = true;							// Enable / disable param1 field
				this.point_param1.Text = "";								// Set text in param1 field
				this.point_param2_lbl.Text = "N/A";							// Set text in param2 label
				this.point_param2.Enabled = false;							// Enable / disable param2 field
				this.point_param2.Text = "N/A";								// Set text in param2 field
				this.point_param3_lbl.Text = "N/A";							// Set text in param3 label
				this.point_param3.Enabled = false;							// Enable / disable param3 field
				this.point_param3.Text = "N/A";								// Set text in param3 field
				// Trade Outcome PIP outputs
				this.pips_exit1.Enabled = true;								// Enable / disable pips_exit1
				this.pips_exit2.Enabled = true;								// Enable / disable pips_exit2
				this.pips_exit3.Enabled = false;							// Enable / disable pips_exit3
				this.pips_subTotal.Enabled = true;							// Enable / disable pips_subtotal
				this.pips_tot.Enabled = true;								// Enable / disable pips_tot
				// Trade Outcome Amount outputs
				this.amount_exit1.Enabled = true;							// Enable / disable amount_exit1
				this.amount_exit2.Enabled = true;							// Enable / disable amount_exit2
				this.amount_exit3.Enabled = false;							// Enable / disable amount_exit3
				this.amount_subTotal.Enabled = true;						// Enable / disable amount_subtotal
				this.amount_tot.Enabled = true;								// Enable / disable amount_total
				// Account outputs
				this.acc_risk.Enabled = true;								// Enable / disable acc_risk
				this.acc_balOnLoss.Enabled = true;							// Enable / disable acc_balonloss
				this.acc_balOnExit1.Enabled = true;							// Enable / disable acc_balonexit1
				this.acc_balOnExit2.Enabled = true;							// Enable / disable acc_balonexit2
				this.acc_balOnExit3.Enabled = false;						// Enable / disable acc_balonexit3
				this.acc_profitPerc.Enabled = true;							// Enable / disable acc_profitperc		
				this.acc_profitLossRatio.Enabled = true;					// Enable / disable acc_profitlossratio
				// Other form element
				this.statusBox_riskRewardOn.Enabled = true;					// Enable / disable status bar riskrewardOn
				this.statusBox_spreadExit1.Enabled = true;					// Enable / disable status bar spread Exit1
				this.statusBox_spreadSL.Enabled = true;						// Enable / disable status bar spread StopLoss
				// Warning box
				this.out_warningText.Visible = false;						// Hide the warning message box
				this.out_warningText_lbl.Visible = false;					// Hide the warning box label
			}

			// Other Form functions
			private void MakeDataVisible()
			{
				// Make values visible
				// point_entry
				if (this.dbl_enter > 0) this.point_entry.Text = dbl_enter.ToString(numFormat);
				else this.point_entry.Text = "N/A";
				// point_stoploss
				if (this.dbl_sl > 0) this.point_stoploss.Text = dbl_sl.ToString(numFormat);
				else this.point_stoploss.Text = "N/A";
				// point_exit1
				if (dbl_exit1 > 0) this.point_exit1.Text = dbl_exit1.ToString(numFormat);
				else this.point_exit1.Text = "N/A";
				// point_exit2
				if (dbl_exit2 > 0) this.point_exit2.Text = dbl_exit2.ToString(numFormat);
				else this.point_exit2.Text = "N/A";
				// point_exit3
				if (dbl_exit3 > 0) this.point_exit3.Text = dbl_exit3.ToString(numFormat);
				else this.point_exit3.Text = "N/A";
				// pips_loss
				if (dbl_pipLoss > 0) this.pips_loss.Text = this.dbl_pipLoss.ToString();
				else this.pips_loss.Text = "N/A";
				// pips_exit1
				if (dbl_pipExit1 > 0) this.pips_exit1.Text = this.dbl_pipExit1.ToString();
				else this.pips_exit1.Text = "N/A";
				// pips_exit2
				if (dbl_pipExit2 > 0) this.pips_exit2.Text = this.dbl_pipExit2.ToString();
				else this.pips_exit2.Text = "N/A";
				// pips_exit3
				if (dbl_pipExit3 > 0) this.pips_exit3.Text = this.dbl_pipExit3.ToString();
				else this.pips_exit3.Text = "N/A";
				// pips_subTotal
				if (dbl_pipSubTot > 0) this.pips_subTotal.Text = this.dbl_pipSubTot.ToString();
				else this.pips_subTotal.Text = "N/A";
				// pips_total
				if (dbl_pipTotal > 0) this.pips_tot.Text = this.dbl_pipTotal.ToString();
				else this.pips_tot.Text = "N/A";
				// amount_loss
				if (dbl_amountLoss > 0) this.amount_loss.Text = this.dbl_amountLoss.ToString(currFormat);
				else this.amount_loss.Text = "N/A";
				// amount_exit1
				if (dbl_amountExit1 > 0) this.amount_exit1.Text = this.dbl_amountExit1.ToString(currFormat);
				else this.amount_exit1.Text = "N/A";
				// amount_exit2
				if (dbl_amountExit2 > 0) this.amount_exit2.Text = this.dbl_amountExit2.ToString(currFormat);
				else this.amount_exit2.Text = "N/A";
				// amount_exit3
				if (dbl_amountExit3 > 0) this.amount_exit3.Text = this.dbl_amountExit3.ToString(currFormat);
				else this.amount_exit3.Text = "N/A";
				// amount_subTotal
				if (dbl_amountSubTot > 0) this.amount_subTotal.Text = this.dbl_amountSubTot.ToString(currFormat);
				else this.amount_subTotal.Text = "N/A";
				// amount_tot
				if (dbl_amountTotal > 0) this.amount_tot.Text = this.dbl_amountTotal.ToString(currFormat);
				else this.amount_tot.Text = "N/A";
				// Trade Parameter 1
				if (this.i_tradeParam1 > 0) this.point_param1.Text = this.i_tradeParam1.ToString();
				else this.point_param1.Text = "N/A";
				// Trade Parameter 2
				if (this.i_tradeParam2 > 0) this.point_param2.Text = this.i_tradeParam2.ToString();
				else this.point_param2.Text = "N/A";
				// Trade Parameter 3
				if (this.i_tradeParam3 > 0) this.point_param3.Text = this.i_tradeParam3.ToString();
				else this.point_param3.Text = "N/A";

				// Calculate and show Account properties
				// acc_risk
				if (this.input_equityData.Value != 0 && dbl_amountLoss > 0)
				{
					this.acc_risk.Text = ((this.dbl_amountLoss / this.input_equityData.Value)).ToString("0.00%");
					if ((this.dbl_amountLoss / this.input_equityData.Value) > 0.05) this.acc_risk.BackColor = Color.Red;
					else this.acc_risk.BackColor = Color.Green;
				}
				else
				{
					this.acc_risk.Text = "N/A";
					this.acc_risk.BackColor = Color.White;
				}
				// acc_balOnLoss
				if (dbl_amountLoss > 0 && input_equityData.Value != 0) this.acc_balOnLoss.Text = (this.input_equityData.Value - this.dbl_amountLoss).ToString(currFormat);
				else this.acc_balOnLoss.Text = "N/A";
				// acc_balOnExit1
				if (dbl_amountExit1 > 0 && input_equityData.Value != 0) this.acc_balOnExit1.Text = (this.input_equityData.Value + this.dbl_amountExit1).ToString(currFormat);
				else this.acc_balOnExit1.Text = "N/A";
				// acc_balOnExit2
				if (dbl_amountExit1 > 0 && dbl_amountExit2 > 0 && input_equityData.Value != 0) this.acc_balOnExit2.Text = (this.input_equityData.Value + this.dbl_amountExit1 + this.dbl_amountExit2).ToString(currFormat);
				else this.acc_balOnExit2.Text = "N/A";
				// acc_balOnExit3
				if (dbl_amountExit1 > 0 && dbl_amountExit2 > 0 && dbl_amountExit3 > 0 && input_equityData.Value != 0) this.acc_balOnExit3.Text = (this.input_equityData.Value + this.dbl_amountExit1 + this.dbl_amountExit2 + this.dbl_amountExit3).ToString(currFormat);
				else this.acc_balOnExit3.Text = "N/A";
				// acc_profitPerc
				if (dbl_amountExit1 > 0 && dbl_amountExit2 > 0 && input_equityData.Value != 0) this.acc_profitPerc.Text = ((this.dbl_amountExit1 + this.dbl_amountExit2) / this.input_equityData.Value).ToString("0.00%");
				else if (dbl_pipLoss < 0 && tradeType == TradeMethod.EMA10Maintain && input_equityData.Value != 0) this.acc_profitPerc.Text = (-1.0 * dbl_pipLoss / this.input_equityData.Value).ToString("0.00%");
				else if (dbl_amountExit1 > 0 && tradeType == TradeMethod.SymmTriBreak) this.acc_profitPerc.Text = (this.dbl_amountExit1 / this.dbl_equity).ToString("0.00%");
				else this.acc_profitPerc.Text = "N/A";
				// acc_profitLossRatio
				if (dbl_amountExit1 > 0 && dbl_amountExit2 > 0 && dbl_amountLoss > 0 && dbl_amountLoss != 0) this.acc_profitLossRatio.Text = ((this.dbl_amountExit1 + this.dbl_amountExit2) / this.dbl_amountLoss).ToString("0.0000");
				else if (this.dbl_amountExit1 > 0 && tradeType == TradeMethod.SymmTriBreak && dbl_amountLoss != 0) this.acc_profitLossRatio.Text = (this.dbl_amountExit1 / this.dbl_amountLoss).ToString("0.0000");
				else this.acc_profitLossRatio.Text = "N/A";
			}
			private void ClearForm_Click(object sender, EventArgs e)
			{
				ResetAllData(sender, e);
			}
			private void SelectAllInputsOnEnter(object sender, EventArgs e)
			{
				if (sender.GetType() == typeof(TextBox))
				{
					((TextBox)sender).SelectAll();
				}
			}
			private void Btn_Calc_Click(object sender, EventArgs e)
			{
				ResetOutputVariables();
				this.out_warningText.Visible = false;
				this.out_warningText_lbl.Visible = false;
				GetData();
				switch (this.tradeType)
				{
					case TradeMethod.FlagPole:
						CalculateFlagpoleData();
						break;
					case TradeMethod.SlingShot:
						CalculateSlingShotData();
						break;
					case TradeMethod.AscendingTriangle:
						CalculateAscendingTriangleData();
						break;
					case TradeMethod.DescendingTriangle:
						CalculateDescendingTriangleData();
						break;
					case TradeMethod.HorrLineBreak:
						CalculateHorrizontalRangeBreakoutData();
						break;
					case TradeMethod.EMA10Setup:
						CalculateEntryEMA10Data();
						break;
					case TradeMethod.EMA10Maintain:
						CalculateEMA10MaintenanceData();
						break;
					case TradeMethod.SymmTriBreak:
						CalculateSymmetricalTriangleBreakoutData();
						break;
					case TradeMethod.SymmTri2LegBreak:
						CalculateSymmetricalTriangle2LegBreakoutData();
						break;
					default:
						MessageBox.Show("Select trading method from the tools menu!", "Selection Errror!", MessageBoxButtons.OK);
						break;
				}
				MakeDataVisible();
			}
			private void tabPage_Enter(object sender, EventArgs e)
			{
				this.input_Data1.Focus();
			}

			#endregion

			// Other functionalities
			#region Application functionalities
			private Hashtable Serialize()
			{
				Hashtable x = new Hashtable();

				// Serialize application state data
				x.Add("tradeMethod", this.tradeType);

				// Serialize Input data
				foreach (object obj in this.Panel_Inputs.Controls)
				{
					if (obj.GetType() == typeof(TextBox))
					{
						x.Add(((TextBox)obj).Name, ((TextBox)obj).Text);
					}
					else if (obj.GetType() == typeof(CurrencyTextBox))
					{
						x.Add(((CurrencyTextBox)obj).Name, ((CurrencyTextBox)obj).Value);
					}
					else if (obj.GetType() == typeof(ComboBox))
					{
						x.Add(((ComboBox)obj).Name, ((ComboBox)obj).SelectedIndex);
					}
					else if (obj.GetType() == typeof(CheckBox))
					{
						x.Add(((CheckBox)obj).Name, ((CheckBox)obj).Checked);
					}
					else if (obj.GetType() == typeof(RadioButton))
					{
						x.Add(((RadioButton)obj).Name, ((RadioButton)obj).Checked);
					}
					else if (obj.GetType() == typeof(GroupBox))
					{
						foreach (Object o in ((GroupBox)obj).Controls)
						{
							if (o.GetType() == typeof(TextBox))
							{
								x.Add(((TextBox)o).Name, ((TextBox)o).Text);
							}
							else if (o.GetType() == typeof(CurrencyTextBox))
							{
								x.Add(((CurrencyTextBox)o).Name, ((CurrencyTextBox)o).Value);
							}
							else if (o.GetType() == typeof(ComboBox))
							{
								x.Add(((ComboBox)o).Name, ((ComboBox)o).SelectedIndex);
							}
							else if (o.GetType() == typeof(CheckBox))
							{
								x.Add(((CheckBox)o).Name, ((CheckBox)o).Checked);
							}
							else if (o.GetType() == typeof(RadioButton))
							{
								x.Add(((RadioButton)o).Name, ((RadioButton)o).Checked);
							}
						}
					}
				}

				// Serialize setuptab Page data
				foreach (object obj in this.setUpTab.Controls)
				{
					if (obj.GetType() == typeof(TextBox))
					{
						x.Add(((TextBox)obj).Name, ((TextBox)obj).Text);
					}
					else if (obj.GetType() == typeof(CurrencyTextBox))
					{
						x.Add(((CurrencyTextBox)obj).Name, ((CurrencyTextBox)obj).Value);
					}
					else if (obj.GetType() == typeof(ComboBox))
					{
						x.Add(((ComboBox)obj).Name, ((ComboBox)obj).SelectedIndex);
					}
					else if (obj.GetType() == typeof(CheckBox))
					{
						x.Add(((CheckBox)obj).Name, ((CheckBox)obj).Checked);
					}
					else if (obj.GetType() == typeof(RadioButton))
					{
						x.Add(((RadioButton)obj).Name, ((RadioButton)obj).Checked);
					}
					else if (obj.GetType() == typeof(GroupBox))
					{
						foreach (Object o in ((GroupBox)obj).Controls)
						{
							if (o.GetType() == typeof(TextBox))
							{
								x.Add(((TextBox)o).Name, ((TextBox)o).Text);
							}
							else if (o.GetType() == typeof(CurrencyTextBox))
							{
								x.Add(((CurrencyTextBox)o).Name, ((CurrencyTextBox)o).Value);
							}
							else if (o.GetType() == typeof(ComboBox))
							{
								x.Add(((ComboBox)o).Name, ((ComboBox)o).SelectedIndex);
							}
							else if (o.GetType() == typeof(CheckBox))
							{
								x.Add(((CheckBox)o).Name, ((CheckBox)o).Checked);
							}
							else if (o.GetType() == typeof(RadioButton))
							{
								x.Add(((RadioButton)o).Name, ((RadioButton)o).Checked);
							}
						}
					}
				}

				// Serialize results Page data
				foreach (object obj in this.resultsTab.Controls)
				{
					if (obj.GetType() == typeof(TextBox))
					{
						x.Add(((TextBox)obj).Name, ((TextBox)obj).Text);
					}
					else if (obj.GetType() == typeof(CurrencyTextBox))
					{
						x.Add(((CurrencyTextBox)obj).Name, ((CurrencyTextBox)obj).Value);
					}
					else if (obj.GetType() == typeof(ComboBox))
					{
						x.Add(((ComboBox)obj).Name, ((ComboBox)obj).SelectedIndex);
					}
					else if (obj.GetType() == typeof(CheckBox))
					{
						x.Add(((CheckBox)obj).Name, ((CheckBox)obj).Checked);
					}
					else if (obj.GetType() == typeof(RadioButton))
					{
						x.Add(((RadioButton)obj).Name, ((RadioButton)obj).Checked);
					}
					else if (obj.GetType() == typeof(GroupBox))
					{
						foreach (Object o in ((GroupBox)obj).Controls)
						{
							if (o.GetType() == typeof(TextBox))
							{
								x.Add(((TextBox)o).Name, ((TextBox)o).Text);
							}
							else if (o.GetType() == typeof(CurrencyTextBox))
							{
								x.Add(((CurrencyTextBox)o).Name, ((CurrencyTextBox)o).Value);
							}
							else if (o.GetType() == typeof(ComboBox))
							{
								x.Add(((ComboBox)o).Name, ((ComboBox)o).SelectedIndex);
							}
							else if (o.GetType() == typeof(CheckBox))
							{
								x.Add(((CheckBox)o).Name, ((CheckBox)o).Checked);
							}
							else if (o.GetType() == typeof(RadioButton))
							{
								x.Add(((RadioButton)o).Name, ((RadioButton)o).Checked);
							}
						}
					}
				}
				return x;
			}
			private void ReadInData(Hashtable tbl)
			{
				Control[] cntrls;

				TradeSetup((TradeMethod)tbl["tradeMethod"]);
				tbl.Remove("tradeMethod");

				foreach (string k in tbl.Keys)
				{
					// Find the control
					cntrls = this.Controls.Find(k, true);

					if (cntrls[0].GetType() == typeof(TextBox))
					{
						((TextBox)cntrls[0]).Text = tbl[k].ToString();
					}
					else if (cntrls[0].GetType() == typeof(ComboBox))
					{
						((ComboBox)cntrls[0]).SelectedIndex = (int)tbl[k];
					}
					else if (cntrls[0].GetType() == typeof(CurrencyTextBox))
					{
						((CurrencyTextBox)cntrls[0]).SetText(tbl[k].ToString());
					}
					else if (cntrls[0].GetType() == typeof(CheckBox))
					{
						((CheckBox)cntrls[0]).Checked = (bool)tbl[k];
					}
					else if (cntrls[0].GetType() == typeof(RadioButton))
					{
						((RadioButton)cntrls[0]).Checked = (bool)tbl[k];
					}
				}
			}
			/// <summary>
			/// Runs the trade setup for the provided input.
			/// Used for setting up the correct trade setup when reading in
			/// data from a file.
			/// </summary>
			/// <param name="tm">TradeMethod enumeration</param>
			private void TradeSetup(TradeMethod tm)
			{
				switch (tm)
				{
					case TradeMethod.AscendingTriangle:
						SetupAscendingTriangleBreakout();
						break;
					case TradeMethod.DescendingTriangle:
						SetupDescendingTriangleBreakout();
						break;
					case TradeMethod.EMA10Maintain:
						SetupEMA10TradeMaintenance();
						break;
					case TradeMethod.EMA10Setup:
						SetupEMA10TradeSetup();
						break;
					case TradeMethod.FlagPole:
						SetupFlagPole();
						break;
					case TradeMethod.HorrLineBreak:
						SetupHorizontalRangeBreakout();
						break;
					case TradeMethod.SlingShot:
						SetupSlingShot();
						break;
					case TradeMethod.SymmTriBreak:
						SetupSymmTriangleBreakout();
						break;
					case TradeMethod.SymmTri2LegBreak:
						SetupSymmTriangle2LegBreakout();
						break;
				}
			}

			#endregion



























		}
	}
}