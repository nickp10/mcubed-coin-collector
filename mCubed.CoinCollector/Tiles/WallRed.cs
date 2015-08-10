using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace mCubed.CoinCollector.Tiles {
	public class WallRed : BaseTile {
		#region Constructor

		/// <summary>
		/// Create a new red wall barrier
		/// </summary>
		public WallRed()
			: base(new Image
			{
				Source = GenerateImage("WallRed.png")
			}) { }

		#endregion

		#region BaseTile Members

		/// <summary>
		/// Perform the logic when the given tile collides with this tile
		/// </summary>
		/// <param name="tile">The tile that collided with this tile</param>
		/// <param name="side">The prominent side of the collision</param>
		/// <returns>The event arguments for the collision</returns>
		public override CollisionEventArgs CollideWith(Player tile, CollisionSide side) {
			return side == CollisionSide.None ? null :
				new CollisionEventArgs
			{
				Command = "died",
				Sender = this
			};
		}

		#endregion
	}
}