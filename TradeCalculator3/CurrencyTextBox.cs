using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Data4Mat
{
	namespace TradeCalculator
	{
		class CurrencyTextBox : System.Windows.Forms.TextBox
		{
			private double val;
			private bool changingText = false;
			private string prefix = "$";
			private double defVal = 3000.0;

			public CurrencyTextBox()
				: base()
			{
				changingText = true;
				this.Text = defVal.ToString(prefix + "#,##0.00");
				this.val = this.defVal;
				changingText = false;
			}

			public void SetText(string txt)
			{
				if (txt.Length > 0)
				{
					CleanString(ref txt);
				}
				else
				{
					txt = global::Data4Mat.TradeCalculator.Properties.Settings.Default.Equity;
					if (txt.Length > 0)
					{
						CleanString(ref txt);
					}
					else
					{
						txt = "0";
					}
				}
				this.val = double.Parse(txt);
				if (!this.changingText) this.Text = double.Parse(txt).ToString(prefix + "#,##0.00");
			}

			public string CurrencyPrefix
			{
				get { return this.prefix; }
				set { this.prefix = value; }
			}

			protected override void OnEnter(EventArgs e)
			{
				this.SelectAll();
				base.OnEnter(e);
			}

			protected override void OnTextChanged(EventArgs e)
			{
				if (!this.changingText)
				{
					this.changingText = true;
					SetText(this.Text);
					this.changingText = false;
				}
				base.OnTextChanged(e);
			}

			protected override void OnTextAlignChanged(EventArgs e)
			{
				base.OnTextAlignChanged(e);
			}

			protected override void OnLeave(EventArgs e)
			{
				SetText(this.Text);
				base.OnLeave(e);
			}

			public override void ResetText()
			{
				base.ResetText();
				changingText = true;
				if (val > 0) this.Text = val.ToString(prefix + "#,##0.00");
				else
				{
					this.Text = defVal.ToString(prefix + "#,##0.00");
					this.val = defVal;
				}
				changingText = false;
			}

			private void CleanString(ref string x)
			{
				string tmp = "";
				char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };
				foreach (char c in x)
				{
					if (c.ToString().IndexOfAny(digits) > -1)
					{
						tmp += c.ToString();
					}
				}
				x = tmp;
			}

			public double Value
			{
				get { return this.val; }
			}

			public double Default
			{
				set
				{
					if (value > 0) this.defVal = value;
					else this.defVal = 3000.0;
				}
			}
		}
	}
}