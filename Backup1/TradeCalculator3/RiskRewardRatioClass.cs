using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Data4Mat
{
	namespace TradeCalculator
	{
		class RiskRewardRatioItem : Object
		{
			private int risk;
			private int reward;

			/// <summary>
			/// Constructor RiskRewardRatioItem. Overloded.
			/// </summary>
			public RiskRewardRatioItem()
				: base()
			{
				this.risk = 1;
				this.reward = 1;
			}

			/// <summary>
			/// Constructor overload of RiskRewardRatioItem.
			/// </summary>
			/// <param name="risk">The risk value</param>
			/// <param name="reward">The reward value</param>
			public RiskRewardRatioItem(int risk, int reward)
				: base()
			{
				if (risk > 0) this.risk = risk;
				else this.risk = 1;

				if (reward > 0) this.reward = reward;
				else this.reward = 1;
			}

			/// <summary>
			/// Get set the Risk value
			/// </summary>
			public int Risk
			{
				get { return this.risk; }
				set
				{
					if (value > 0)
					{
						this.risk = value;
					}
					else
					{
						MessageBox.Show("Risk value must be greater than 0", "Invalid Input!", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
			}

			/// <summary>
			/// Get Set the Reward value
			/// </summary>
			public int Reward
			{
				get { return this.reward; }
				set
				{
					if (value > 0)
					{
						this.reward = value;
					}
					else
					{
						MessageBox.Show("Reward value must be greater than 0", "Invalid Input!", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
			}

			/// <summary>
			/// Get the ratio in a string format of Reward:Risk
			/// </summary>
			public string Ratio
			{
				get { return this.reward.ToString() + ":" + this.risk.ToString(); }
			}

			/// <summary>
			/// Returns the ratio on the form of Reward/Risk rounded to "decimals" decimals accuracy.
			/// </summary>
			/// /// <param name="decimals">Number of decimals in the return value.</param>
			/// <returns>The ratio between reward and risk</returns>
			public double GetRatio(int decimals)
			{
				if (decimals < 0) decimals = 4;
				return Math.Round((double)(this.reward / this.risk), decimals);
			}

			/// <summary>
			/// Returns the ratio on the form of Reward/Risk rounded to 4 decimals accuracy.
			/// </summary>
			/// <returns>The ratio between reward and risk</returns>
			public double GetRatio()
			{
				return Math.Round((double)this.reward / (double)this.risk, 4);
			}

			/// <summary>
			/// Copies the provided item to this object.
			/// </summary>
			/// <param name="item">The RiskRewardRatioItem to copy from</param>
			public void Copy(RiskRewardRatioItem item)
			{
				this.risk = item.risk;
				this.reward = item.reward;
			}

			/// <summary>
			/// Copy's this object to the supplied object.
			/// </summary>
			/// <param name="item">The RiskRewardRatioItem to copy to</param>
			public void CopyTo(ref RiskRewardRatioItem item)
			{
				item.risk = this.risk;
				item.reward = this.reward;
			}
		}

		class RiskRewardRatioItems
		{
			RiskRewardRatioItem[] itms;

			// Default values
			//2:1,7:4,5:3,3:2,4:3,5:4,1:1,4:5,3:4,2:3,3:5,4:7,1:2,

			/// <summary>
			/// Constructor RiskRewardRatioItems.
			/// Sets the default array of 13 ratios on the form Reward:Risk.
			/// 2:1, 7:4, 5:3, 3:2, 4:3, 5:4, 1:1, 4:5, 3:4, 2:3, 3:5, 4:7, 1:2,
			/// </summary>
			public RiskRewardRatioItems()
			{
				// Create default array of standard values
				Array.Resize(ref itms, 13);
				for (int i = 0; i < itms.Length; i++)
				{
					this.itms[i] = new RiskRewardRatioItem();
				}
				this.itms[0].Reward = 2; this.itms[0].Risk = 1;
				this.itms[1].Reward = 7; this.itms[1].Risk = 4;
				this.itms[2].Reward = 5; this.itms[2].Risk = 3;
				this.itms[3].Reward = 3; this.itms[3].Risk = 2;
				this.itms[4].Reward = 4; this.itms[4].Risk = 3;
				this.itms[5].Reward = 5; this.itms[5].Risk = 4;
				this.itms[6].Reward = 1; this.itms[6].Risk = 1;
				this.itms[7].Reward = 4; this.itms[7].Risk = 5;
				this.itms[8].Reward = 3; this.itms[8].Risk = 4;
				this.itms[9].Reward = 2; this.itms[9].Risk = 3;
				this.itms[10].Reward = 3; this.itms[10].Risk = 5;
				this.itms[11].Reward = 4; this.itms[11].Risk = 7;
				this.itms[12].Reward = 1; this.itms[12].Risk = 2;
			}

			//public RiskRewardRatioItems(RiskRewardRatioItem itm)
			//{
			//    Array.Resize(ref itms, 1);
			//    this.itms[0].Copy(itm);
			//}

			public RiskRewardRatioItem this[int indx]
			{
				get
				{
					if (indx < itms.Length) return itms[indx];
					else return null;
				}
				set
				{
					if (indx < itms.Length) itms[indx] = value;
					else this.Add(value);
				}
			}

			public void Add(RiskRewardRatioItem itm)
			{
				int l = this.itms.Length;
				Array.Resize(ref itms, itms.Length + 1);
				this.itms[l].Copy(itm);
			}

			public int Length
			{
				get { return this.itms.Length; }
			}
		}
	}
}
