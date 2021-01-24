namespace Data4Mat
{

	namespace TradeCalculator
	{
		/// <summary>
		/// 
		/// </summary>
		partial class NewCurrencyPairForm
		{
			/// <summary>
			/// Required designer variable.
			/// </summary>
			private System.ComponentModel.IContainer components = null;

			/// <summary>
			/// Clean up any resources being used.
			/// </summary>
			/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
			protected override void Dispose(bool disposing)
			{
				if (disposing && (components != null))
				{
					components.Dispose();
				}
				base.Dispose(disposing);
			}

			#region Windows Form Designer generated code

			/// <summary>
			/// Required method for Designer support - do not modify
			/// the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent()
			{
				this.components = new System.ComponentModel.Container();
				this.OK_btn = new System.Windows.Forms.Button();
				this.Cancel_btn = new System.Windows.Forms.Button();
				this.label1 = new System.Windows.Forms.Label();
				this.box_pair = new System.Windows.Forms.TextBox();
				this.crBindingSource = new System.Windows.Forms.BindingSource(this.components);
				this.box_quote = new System.Windows.Forms.TextBox();
				this.label2 = new System.Windows.Forms.Label();
				this.box_size = new System.Windows.Forms.TextBox();
				this.label3 = new System.Windows.Forms.Label();
				this.box_pipvalue = new System.Windows.Forms.TextBox();
				this.label4 = new System.Windows.Forms.Label();
				this.box_spread = new System.Windows.Forms.TextBox();
				this.label5 = new System.Windows.Forms.Label();
				this.box_decimals = new System.Windows.Forms.TextBox();
				this.label6 = new System.Windows.Forms.Label();
				this.box_prefix = new System.Windows.Forms.TextBox();
				this.label7 = new System.Windows.Forms.Label();
				this.box_indx = new System.Windows.Forms.TextBox();
				this.label8 = new System.Windows.Forms.Label();
				this.input_selectCurrPair = new System.Windows.Forms.ComboBox();
				this.input_selectCurrPair_lbl = new System.Windows.Forms.Label();
				((System.ComponentModel.ISupportInitialize)(this.crBindingSource)).BeginInit();
				this.SuspendLayout();
				// 
				// OK_btn
				// 
				this.OK_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
				this.OK_btn.AutoSize = true;
				this.OK_btn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
				this.OK_btn.Location = new System.Drawing.Point(72, 246);
				this.OK_btn.Name = "OK_btn";
				this.OK_btn.Size = new System.Drawing.Size(32, 23);
				this.OK_btn.TabIndex = 8;
				this.OK_btn.Text = "OK";
				this.OK_btn.UseVisualStyleBackColor = true;
				this.OK_btn.Click += new System.EventHandler(this.OK_btn_Click);
				// 
				// Cancel_btn
				// 
				this.Cancel_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
				this.Cancel_btn.AutoSize = true;
				this.Cancel_btn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
				this.Cancel_btn.Location = new System.Drawing.Point(110, 246);
				this.Cancel_btn.Name = "Cancel_btn";
				this.Cancel_btn.Size = new System.Drawing.Size(50, 23);
				this.Cancel_btn.TabIndex = 9;
				this.Cancel_btn.Text = "Cancel";
				this.Cancel_btn.UseVisualStyleBackColor = true;
				this.Cancel_btn.Click += new System.EventHandler(this.Cancel_btn_Click);
				// 
				// label1
				// 
				this.label1.AutoSize = true;
				this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				this.label1.Location = new System.Drawing.Point(14, 68);
				this.label1.Name = "label1";
				this.label1.Size = new System.Drawing.Size(61, 12);
				this.label1.TabIndex = 2;
				this.label1.Text = "Currency Pair";
				// 
				// box_pair
				// 
				this.box_pair.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.crBindingSource, "CurrencyPair", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "-1"));
				this.box_pair.Location = new System.Drawing.Point(88, 64);
				this.box_pair.Name = "box_pair";
				this.box_pair.Size = new System.Drawing.Size(67, 20);
				this.box_pair.TabIndex = 1;
				this.box_pair.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
				// 
				// crBindingSource
				// 
				this.crBindingSource.DataSource = typeof(Data4Mat.TradeCalculator.cr);
				// 
				// box_quote
				// 
				this.box_quote.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.crBindingSource, "QuoteCurrency", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "-1"));
				this.box_quote.Location = new System.Drawing.Point(88, 89);
				this.box_quote.Name = "box_quote";
				this.box_quote.Size = new System.Drawing.Size(67, 20);
				this.box_quote.TabIndex = 2;
				this.box_quote.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
				// 
				// label2
				// 
				this.label2.AutoSize = true;
				this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				this.label2.Location = new System.Drawing.Point(14, 93);
				this.label2.Name = "label2";
				this.label2.Size = new System.Drawing.Size(70, 12);
				this.label2.TabIndex = 4;
				this.label2.Text = "Quote Currency";
				// 
				// box_size
				// 
				this.box_size.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.crBindingSource, "LotSize", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N1"));
				this.box_size.Location = new System.Drawing.Point(88, 114);
				this.box_size.Name = "box_size";
				this.box_size.Size = new System.Drawing.Size(67, 20);
				this.box_size.TabIndex = 3;
				this.box_size.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
				// 
				// label3
				// 
				this.label3.AutoSize = true;
				this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				this.label3.Location = new System.Drawing.Point(14, 118);
				this.label3.Name = "label3";
				this.label3.Size = new System.Drawing.Size(38, 12);
				this.label3.TabIndex = 6;
				this.label3.Text = "Lot Size";
				// 
				// box_pipvalue
				// 
				this.box_pipvalue.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.crBindingSource, "TickValue", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "-1", "C0"));
				this.box_pipvalue.Location = new System.Drawing.Point(88, 139);
				this.box_pipvalue.Name = "box_pipvalue";
				this.box_pipvalue.Size = new System.Drawing.Size(67, 20);
				this.box_pipvalue.TabIndex = 4;
				this.box_pipvalue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
				// 
				// label4
				// 
				this.label4.AutoSize = true;
				this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				this.label4.Location = new System.Drawing.Point(14, 143);
				this.label4.Name = "label4";
				this.label4.Size = new System.Drawing.Size(46, 12);
				this.label4.TabIndex = 8;
				this.label4.Text = "PIP Value";
				// 
				// box_spread
				// 
				this.box_spread.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.crBindingSource, "Spread", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "-1", "N0"));
				this.box_spread.Location = new System.Drawing.Point(88, 164);
				this.box_spread.Name = "box_spread";
				this.box_spread.Size = new System.Drawing.Size(67, 20);
				this.box_spread.TabIndex = 5;
				this.box_spread.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
				// 
				// label5
				// 
				this.label5.AutoSize = true;
				this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				this.label5.Location = new System.Drawing.Point(14, 168);
				this.label5.Name = "label5";
				this.label5.Size = new System.Drawing.Size(34, 12);
				this.label5.TabIndex = 10;
				this.label5.Text = "Spread";
				// 
				// box_decimals
				// 
				this.box_decimals.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.crBindingSource, "Decimals", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "-1", "N0"));
				this.box_decimals.Location = new System.Drawing.Point(88, 189);
				this.box_decimals.Name = "box_decimals";
				this.box_decimals.Size = new System.Drawing.Size(67, 20);
				this.box_decimals.TabIndex = 6;
				this.box_decimals.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
				// 
				// label6
				// 
				this.label6.AutoSize = true;
				this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				this.label6.Location = new System.Drawing.Point(14, 193);
				this.label6.Name = "label6";
				this.label6.Size = new System.Drawing.Size(44, 12);
				this.label6.TabIndex = 12;
				this.label6.Text = "Decimals";
				// 
				// box_prefix
				// 
				this.box_prefix.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.crBindingSource, "CurrencyPrefix", true));
				this.box_prefix.Location = new System.Drawing.Point(88, 214);
				this.box_prefix.Name = "box_prefix";
				this.box_prefix.Size = new System.Drawing.Size(67, 20);
				this.box_prefix.TabIndex = 7;
				this.box_prefix.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
				// 
				// label7
				// 
				this.label7.AutoSize = true;
				this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				this.label7.Location = new System.Drawing.Point(14, 218);
				this.label7.Name = "label7";
				this.label7.Size = new System.Drawing.Size(29, 12);
				this.label7.TabIndex = 14;
				this.label7.Text = "Prefix";
				// 
				// box_indx
				// 
				this.box_indx.Enabled = false;
				this.box_indx.Location = new System.Drawing.Point(88, 39);
				this.box_indx.Name = "box_indx";
				this.box_indx.ReadOnly = true;
				this.box_indx.Size = new System.Drawing.Size(67, 20);
				this.box_indx.TabIndex = 15;
				this.box_indx.TabStop = false;
				this.box_indx.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
				this.box_indx.WordWrap = false;
				// 
				// label8
				// 
				this.label8.AutoSize = true;
				this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				this.label8.Location = new System.Drawing.Point(14, 43);
				this.label8.Name = "label8";
				this.label8.Size = new System.Drawing.Size(28, 12);
				this.label8.TabIndex = 16;
				this.label8.Text = "Index";
				// 
				// input_selectCurrPair
				// 
				this.input_selectCurrPair.FormattingEnabled = true;
				this.input_selectCurrPair.Location = new System.Drawing.Point(88, 12);
				this.input_selectCurrPair.Name = "input_selectCurrPair";
				this.input_selectCurrPair.Size = new System.Drawing.Size(67, 21);
				this.input_selectCurrPair.TabIndex = 17;
				// 
				// input_selectCurrPair_lbl
				// 
				this.input_selectCurrPair_lbl.AutoSize = true;
				this.input_selectCurrPair_lbl.Location = new System.Drawing.Point(13, 15);
				this.input_selectCurrPair_lbl.Name = "input_selectCurrPair_lbl";
				this.input_selectCurrPair_lbl.Size = new System.Drawing.Size(58, 13);
				this.input_selectCurrPair_lbl.TabIndex = 18;
				this.input_selectCurrPair_lbl.Text = "New / Edit";
				// 
				// NewCurrencyPairForm
				// 
				this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
				this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
				this.ClientSize = new System.Drawing.Size(157, 265);
				this.ControlBox = false;
				this.Controls.Add(this.input_selectCurrPair_lbl);
				this.Controls.Add(this.input_selectCurrPair);
				this.Controls.Add(this.box_indx);
				this.Controls.Add(this.label8);
				this.Controls.Add(this.box_prefix);
				this.Controls.Add(this.label7);
				this.Controls.Add(this.box_decimals);
				this.Controls.Add(this.label6);
				this.Controls.Add(this.box_spread);
				this.Controls.Add(this.label5);
				this.Controls.Add(this.box_pipvalue);
				this.Controls.Add(this.label4);
				this.Controls.Add(this.box_size);
				this.Controls.Add(this.label3);
				this.Controls.Add(this.box_quote);
				this.Controls.Add(this.label2);
				this.Controls.Add(this.box_pair);
				this.Controls.Add(this.label1);
				this.Controls.Add(this.Cancel_btn);
				this.Controls.Add(this.OK_btn);
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.Name = "NewCurrencyPairForm";
				this.Padding = new System.Windows.Forms.Padding(2);
				this.ShowIcon = false;
				this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
				this.Text = "Enter New Currency Pair";
				((System.ComponentModel.ISupportInitialize)(this.crBindingSource)).EndInit();
				this.ResumeLayout(false);
				this.PerformLayout();

			}

			#endregion

			private System.Windows.Forms.Button OK_btn;
			private System.Windows.Forms.Button Cancel_btn;
			private System.Windows.Forms.Label label1;
			private System.Windows.Forms.TextBox box_pair;
			private System.Windows.Forms.TextBox box_quote;
			private System.Windows.Forms.Label label2;
			private System.Windows.Forms.TextBox box_size;
			private System.Windows.Forms.Label label3;
			private System.Windows.Forms.TextBox box_pipvalue;
			private System.Windows.Forms.Label label4;
			private System.Windows.Forms.TextBox box_spread;
			private System.Windows.Forms.Label label5;
			private System.Windows.Forms.TextBox box_decimals;
			private System.Windows.Forms.Label label6;
			private System.Windows.Forms.TextBox box_prefix;
			private System.Windows.Forms.Label label7;
			private System.Windows.Forms.TextBox box_indx;
			private System.Windows.Forms.Label label8;
			private System.Windows.Forms.BindingSource crBindingSource;
			private System.Windows.Forms.ComboBox input_selectCurrPair;
			private System.Windows.Forms.Label input_selectCurrPair_lbl;
		}
	}
}