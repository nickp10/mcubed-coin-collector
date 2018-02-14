using System.Windows.Controls;

namespace mCubed.CoinCollector {
	public interface IDisplayable {
		Canvas GameArea { get; }
		void GameOver();
		bool JumpAgain();
	}
}