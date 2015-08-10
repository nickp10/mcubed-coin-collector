using System;
using mCubed.CoinCollector.Tiles;

namespace mCubed.CoinCollector {
	public class CollisionEventArgs : EventArgs {
		#region Properties

		public string Command { get; set; }
		public object Argument { get; set; }
		public BaseTile Sender { get; set; }

		#endregion
	}
}