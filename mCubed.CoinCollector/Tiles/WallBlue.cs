using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace mCubed.CoinCollector.Tiles {
	public class WallBlue : GravityTile {
		#region Constructor

		/// <summary>
		/// Create a new blue wall barrier
		/// </summary>
		public WallBlue()
			: base(new Image
			{
				Source = GenerateImage("WallBlue.png")
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
			CollisionEventArgs args = null;
			if (side == CollisionSide.Top) {
				args = new CollisionEventArgs
				{
					Command = "ground",
					Argument = Top + 1,
					Sender = this
				};
				Unground();
			} else if (side == CollisionSide.Bottom) {
				args = new CollisionEventArgs
				{
					Command = "unground",
					Sender = this
				};
			} else if (side == CollisionSide.Left) {
				args = new CollisionEventArgs
				{
					Command = "died",
					Sender = this
				};
			}
			return args;
		}

		#endregion
	}
}