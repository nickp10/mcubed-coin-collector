using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace mCubed.CoinCollector.Tiles {
	public class Finish : BaseTile {
		#region Properties

		/// <summary>
		/// Get whether or not the side must be open when checking collision
		/// </summary>
		public override bool CheckOpenSide { get { return false; } }

		#endregion

		#region Constructor

		/// <summary>
		/// Construct a new finish line
		/// </summary>
		public Finish()
			: base(new Image
			{
				Source = GenerateImage("Finish.png")
			}) {
			Width = 64;
			Height = 64;
		}

		#endregion

		#region BaseTile Members

		/// <summary>
		/// Perform the logic when the given tile collides with this tile
		/// </summary>
		/// <param name="tile">The tile that collided with this tile</param>
		/// <param name="side">The prominent side of the collision</param>
		/// <returns>The event arguments for the collision</returns>
		public override CollisionEventArgs CollideWith(Player tile, CollisionSide side) {
			return (tile.Left >= Left) ? new CollisionEventArgs
				{
					Command = "haltandwin",
					Argument = tile.Left - Left,
					Sender = this
				} : null;
		}

		#endregion
	}
}