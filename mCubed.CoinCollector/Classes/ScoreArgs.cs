using System.ComponentModel;

namespace mCubed.CoinCollector {
	public class ScoreArgs : INotifyPropertyChanged {
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Data Store

		private int _coinsCollected;
		private int _coinsAvailable;
		private bool _isCompletedBonus;
		private bool _isHighScore;
		private int _megaCoinsCollected;
		private int _megaCoinsAvailable;

		#endregion

		#region Properties

		/// <summary>
		/// Get the value for each coin collected
		/// </summary>
		public ulong CoinValue { get { return 100UL; } }

		/// <summary>
		/// Get/set the number of coins that are available to be collected
		/// </summary>
		public int CoinsAvailable {
			get { return _coinsAvailable; }
			set { this.SetAndNotify(ref _coinsAvailable, value, "CoinsAvailable"); }
		}

		/// <summary>
		/// Get/set the number of coins that have been collected
		/// </summary>
		public int CoinsCollected {
			get { return _coinsCollected; }
			set { this.SetAndNotify(ref _coinsCollected, value, "CoinsCollected", "CoinsScore", "TotalScore"); }
		}

		/// <summary>
		/// Get the score for the total number of coins collected
		/// </summary>
		public ulong CoinsScore { get { return (ulong)CoinsCollected * CoinValue; } }

		/// <summary>
		/// Get the bonus that has been rewarded for completing the map
		/// </summary>
		public ulong CompletedBonus { get { return IsCompletedBonus ? 1000UL : 0UL; } }

		/// <summary>
		/// Get/set whether or not the map was completed and should be rewarded the completion bonus
		/// </summary>
		public bool IsCompletedBonus {
			get { return _isCompletedBonus; }
			set { this.SetAndNotify(ref _isCompletedBonus, value, "IsCompletedBonus", "CompletedBonus", "TotalScore"); }
		}

		/// <summary>
		/// Get/set whether or not this score qualified for a high score
		/// </summary>
		public bool IsHighScore {
			get { return _isHighScore; }
			set { this.SetAndNotify(ref _isHighScore, value, "IsHighScore"); }
		}

		/// <summary>
		/// Get whether or not the mega coin bonus should be rewarded meaning all the available mega coins have been collected
		/// </summary>
		public bool IsMegaCoinsBonus { get { return MegaCoinsCollected >= MegaCoinsAvailable; } }

		/// <summary>
		/// Get the value for each mega coin collected
		/// </summary>
		public ulong MegaCoinValue { get { return 500UL; } }

		/// <summary>
		/// Get/set the number of mega coins that are available to be collected
		/// </summary>
		public int MegaCoinsAvailable {
			get { return _megaCoinsAvailable; }
			set { this.SetAndNotify(ref _megaCoinsAvailable, value, "MegaCoinsAvailable", "IsMegaCoinsBonus", "MegaCoinsBonus", "TotalScore"); }
		}

		/// <summary>
		/// Get the bonus that has been rewarded for collecting mega coins
		/// </summary>
		public ulong MegaCoinsBonus { get { return IsMegaCoinsBonus ? 1000UL : 0UL; } }

		/// <summary>
		/// Get/set the number of mega coins that have been collected
		/// </summary>
		public int MegaCoinsCollected {
			get { return _megaCoinsCollected; }
			set { this.SetAndNotify(ref _megaCoinsCollected, value, "MegaCoinsCollected", "MegaCoinsScore", "IsMegaCoinsBonus", "MegaCoinsBonus", "TotalScore"); }
		}

		/// <summary>
		/// Get the score for the total number of mega coins collected
		/// </summary>
		public ulong MegaCoinsScore { get { return (ulong)MegaCoinsCollected * MegaCoinValue; } }

		/// <summary>
		/// Get the total score awarded for the map
		/// </summary>
		public ulong TotalScore { get { return CoinsScore + CompletedBonus + MegaCoinsScore + MegaCoinsBonus; } }

		#endregion
	}
}