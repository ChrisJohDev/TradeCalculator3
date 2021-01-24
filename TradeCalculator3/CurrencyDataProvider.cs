using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Data4Mat
{
	namespace TradeCalculator
	{
		/// <summary>
		/// 
		/// </summary>
		public class CurrencyDataProvider : Object
		{
			static string dataFile = global::Data4Mat.TradeCalculator.Properties.Resources.CurrenciesDataFile;
			//"..\\..\\Data\\currencies.csv";
			private crs data = new crs();
			private int records;

			/// <summary>
			/// Constructor CurrencyDataProvider
			/// </summary>
			public CurrencyDataProvider()
			{
				using (StreamReader sr = new StreamReader(dataFile, true))
				{
					String line;
					string[] d;
					while ((line = sr.ReadLine()) != null)
					{
						d = line.Split(',');
						if (d.Length == 8)
						{
							data.Add(int.Parse(d[0]), d[1], d[2], int.Parse(d[3]), int.Parse(d[4]), int.Parse(d[5]), int.Parse(d[6]), d[7]);
						}
						else
						{
							new Exception("File \"" + dataFile + "\" in wrong format.");
						}
					}
				}
				records = data.Length;
			}

			/// <summary>
			/// 
			/// </summary>
			public crs Currencies
			{
				get { return this.data; }
				set { this.Currencies = value; }
			}

			/// <summary>
			/// Writes the new currency pair to the storage area
			/// </summary>
			/// <param name="append">True if new currency pair should be appeneded to the list of currency pairs</param>
			public void WriteData(bool append)
			{
				if (append)
				{
					using (StreamWriter sw = File.AppendText(dataFile))
					{
						cr c = data.GetCurrency(records);
						string x = records.ToString() + "," +
									c.CurrencyPair + "," +
									c.QuoteCurrency + "," +
									c.LotSize.ToString() + "," +
									c.TickValue.ToString() + "," +
									c.Spread.ToString() + "," +
									c.Decimals.ToString() + "," +
									c.CurrencyPrefix
									;

						sw.WriteLine(x);
					}
				}
			}


		}

		/// <summary>
		/// 
		/// </summary>
		public class cr : Object
		{
			private int indx;
			private string currpair;
			private string currency;
			private int lotsize;
			private int tickvalue;
			private int spread;
			private int decimals;
			private string symbol;

			/// <summary>
			/// 
			/// </summary>
			public int CurrencyIndex
			{
				get { return this.indx; }
				set { this.indx = value; }
			}

			/// <summary>
			/// 
			/// </summary>
			public int LotSize
			{
				get { return this.lotsize; }
				set { this.lotsize = value; }
			}

			/// <summary>
			/// 
			/// </summary>
			public int TickValue
			{
				get { return this.tickvalue; }
				set { this.tickvalue = value; }
			}

			/// <summary>
			/// 
			/// </summary>
			public string CurrencyPair
			{
				get { return this.currpair; }
				set { this.currpair = value; }
			}

			/// <summary>
			/// 
			/// </summary>
			public string QuoteCurrency
			{
				get { return this.currency; }
				set { this.currency = value; }
			}

			/// <summary>
			/// 
			/// </summary>
			public int Spread
			{
				get { return this.spread; }
				set { this.spread = value; }
			}

			/// <summary>
			/// 
			/// </summary>
			public int Decimals
			{
				get { return this.decimals; }
				set { this.decimals = value; }
			}

			/// <summary>
			/// 
			/// </summary>
			public string CurrencyPrefix
			{
				get { return this.symbol; }
				set { this.symbol = value; }
			}

			/// <summary>
			/// Copy's the content of this object to the object supplied
			/// </summary>
			/// <param name="obj">The object to copy to</param>
			public void Copy(ref cr obj)
			{
				obj.currency = this.currency;
				obj.CurrencyIndex = this.CurrencyIndex;
				obj.CurrencyPair = this.CurrencyPair;
				obj.CurrencyPrefix = this.CurrencyPrefix;
				obj.Decimals = this.Decimals;
				obj.LotSize = this.LotSize;
				obj.QuoteCurrency = this.QuoteCurrency;
				obj.Spread = this.Spread;
				obj.TickValue = this.TickValue;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public class crs : Object
		{
			private cr[] data;

			/// <summary>
			/// Constructor crs object
			/// </summary>
			public crs()
			{
				//data.Initialize();
				Array.Resize(ref data, 0);
			}

			/// <summary>
			/// Returns the currency pair at index location "index" as a cr object
			/// </summary>
			/// <param name="index">Index</param>
			/// <returns>Returns the cr object at location "index"</returns>
			public cr GetCurrency(int index)
			{
				if (index < this.data.Length)
				{
					return this.data[index];
				}
				else
				{
					new IndexOutOfRangeException("Index out of range. GetCurrency(int), CurrencyDataProvider");
					return null;
				}
			}

			/// <summary>
			/// Returns the Currency Pair that matches the string "currPair" if found
			/// otherwise returns null.
			/// </summary>
			/// <param name="currPair">The currency pair to look for</param>
			/// <returns>The cr object matching the search string on success, or null on failure</returns>
			public cr GetCurrency(string currPair)
			{
				int rv = -1;
				for (int i = 0; i < this.data.Length; i++)
				{
					if (currPair == this.data[i].CurrencyPair) rv = this.data[i].CurrencyIndex;
				}
				if (rv > -1) return this.data[rv];
				else
				{
					return null;
				}
			}

			/// <summary>
			/// Adds a cr object to the list of cr objects
			/// </summary>
			/// <param name="curr">Object to add to the list</param>
			public void Add(cr curr)
			{
				int l = data.Length;
				Array.Resize(ref data, data.Length + 1);
				data[l] = new cr();
				data[l] = curr;
			}

			/// <summary>
			/// Overloaded version of Add. Adds a new cr object to the list of cr objects
			/// </summary>
			/// <param name="indx">The index paramter</param>
			/// <param name="pair">The currency pair name. (On the form EUR/USD)</param>
			/// <param name="quote">The quote currency</param>
			/// <param name="lotsize">Size of a lot for the pair in the quote currency</param>
			/// <param name="tickvalue">The value of one PIP in the quote currency</param>
			/// <param name="spread">The pair's normal spread</param>
			/// <param name="digits">Number of decimals used for quoting the currency. Normally 4 or 2</param>
			/// <param name="prefix">The quote currency's prefix for example ¥ for JPY</param>
			public void Add(int indx, string pair, string quote, int lotsize, int tickvalue, int spread, int digits, string prefix)
			{
				int l = data.Length;
				Array.Resize(ref data, data.Length + 1);
				data[l] = new cr();
				data[l].CurrencyIndex = indx;
				data[l].CurrencyPair = pair;
				data[l].QuoteCurrency = quote;
				data[l].LotSize = lotsize;
				data[l].TickValue = tickvalue;
				data[l].Spread = spread;
				data[l].Decimals = digits;
				data[l].CurrencyPrefix = prefix;
			}

			/// <summary>
			/// 
			/// </summary>
			public int Length
			{
				get { return this.data.Length; }
			}

			internal void CopyTo(ref crs curr)
			{
				foreach (cr x in this.data)
				{
					curr.Add(x);
				}
			}
		}
	}
}