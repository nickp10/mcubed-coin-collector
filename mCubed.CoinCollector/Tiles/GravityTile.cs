using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace mCubed.CoinCollector.Tiles {
	public abstract class GravityTile : BaseTile {
		#region Propeties

		/// <summary>
		/// Get the gravity for the the player tile
		/// </summary>
		public GravityArgs Gravity { get; private set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Create a new gravity tile based on the representation
		/// </summary>
		/// <param name="representation">The representation for the tile</param>
		public GravityTile(FrameworkElement representation) : base(representation) { }

		#endregion

		#region Members

		/// <summary>
		/// Performs the gravity on the player
		/// </summary>
		public void PerformGravity() {
			if (Gravity != null)
				Bottom = Gravity.NewPosition();
		}

		/// <summary>
		/// Unground the player with a velocity of 0
		/// </summary>
		public void Unground() {
			Unground(0);
		}

		/// <summary>
		/// Unground the player 
		/// </summary>
		/// <param name="velocity">The velocity at which the player has been ungrounded</param>
		public void Unground(double velocity) {
			if (Gravity == null)
				Gravity = new GravityArgs(Bottom, velocity);
		}

		/// <summary>
		/// Ground the player
		/// </summary>
		public void Ground() {
			Gravity = null;
		}

		#endregion
	}
}