namespace Data4Mat
{
	namespace TradeCalculator
	{
		/// <summary>
		/// Enumeration for trade methods
		/// </summary>
		public enum TradeMethod
		{
			/// <summary>
			/// No function selected, Default.
			/// </summary>
			None = -1,
			/// <summary>
			/// Trading the Flagpole formation
			/// </summary>
			FlagPole = 0,
			/// <summary>
			/// Trading Sling Shot
			/// </summary>
			SlingShot = 1,
			/// <summary>
			/// Trading Ascending Triangle Breakout
			/// </summary>
			AscendingTriangle = 3,
			/// <summary>
			/// Trading Descending Triangel Breakout
			/// </summary>
			DescendingTriangle = 4,
			/// <summary>
			/// Trading Horrizontal Range Breakout
			/// </summary>
			HorrLineBreak = 5,
			/// <summary>
			/// Trading EMA(10) Follower - Trending technique
			/// </summary>
			EMA10Setup = 6,
			/// <summary>
			/// Periodic maintanance for EMA(10) trend following trade
			/// </summary>
			EMA10Maintain = 7,
			/// <summary>
			/// Symmetrical Triangle Breakout 1 leg version
			/// </summary>
			SymmTriBreak = 8,
			/// <summary>
			/// Symmetrical Triangle Breakout 2 leg version
			/// </summary>
			SymmTri2LegBreak = 9
		}

		/// <summary>
		/// Enumeration for Risk/Reward types
		/// </summary>
		public enum RiskRewardType
		{
			/// <summary>
			/// Base calculations and assumptions on that it is the PIP value that
			/// should be used for any Risk/Reward calculations and selections.
			/// </summary>
			PIP_Based = 0,
			/// <summary>
			/// Base calculations and assumptions on that it is the Amount in currency that
			/// should be used for any Risk/Reward calculations and selections.
			/// </summary>
			Amount_Based = 1
		}
	}
}