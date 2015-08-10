﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace mCubed.CoinCollector.Tiles {
	public class MegaCoin : BaseTile {
		#region Properties

		/// <summary>
		/// Get whether or not the side must be open when checking collision
		/// </summary>
		public override bool CheckOpenSide { get { return false; } }

		#endregion

		#region Constructor

		/// <summary>
		/// Create a new mega coin
		/// </summary>
		public MegaCoin()
			: base(new Image
			{
				Source = GenerateImage("MegaCoin.png")
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
			return new CollisionEventArgs
			{
				Command = "megacoin",
				Sender = this
			};
		}

		#endregion
	}
}