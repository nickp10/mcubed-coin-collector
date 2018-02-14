using System.ComponentModel;
using mCubed.CoinCollector.Tiles;

namespace mCubed.CoinCollector.Maps {
	public abstract class BaseMap : INotifyPropertyChanged {
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Data Store

		private ulong _highScore;
		private int _mapNumber;
		private string _name;

		#endregion

		#region Properties

		/// <summary>
		/// Get/set the high score for this given map
		/// </summary>
		public ulong HighScore {
			get { return _highScore; }
			set { this.SetAndNotify(ref _highScore, value, "HighScore"); }
		}

		/// <summary>
		/// Get the height of the map
		/// </summary>
		public double MapHeight { get; protected set; }

		/// <summary>
		/// Get/set the map number for this map
		/// </summary>
		public int MapNumber {
			get { return _mapNumber; }
			set { this.SetAndNotify(ref _mapNumber, value, "MapNumber"); }
		}

		/// <summary>
		/// Get the width of the map
		/// </summary>
		public double MapWidth { get; protected set; }

		/// <summary>
		/// Get/set the name of this given map
		/// </summary>
		public string Name {
			get { return _name; }
			set { this.SetAndNotify(ref _name, value, "Name"); }
		}

		#endregion

		#region Abstract Members

		public abstract void GenerateMap(CoinController controller);

		#endregion
	}
}