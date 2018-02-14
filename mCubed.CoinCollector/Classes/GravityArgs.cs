namespace mCubed.CoinCollector {
	public class GravityArgs {
		#region Properties

		public double InitialPosition { get; private set; }
		public double InitialVelocity { get; private set; }
		public int TimeCount { get; private set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Create a new argument set for gravity
		/// </summary>
		/// <param name="position">The initial position of the item being acted upon</param>
		/// <param name="velocity">The velocity at which the gravity is being applied</param>
		public GravityArgs(double position, double velocity) {
			InitialPosition = position;
			InitialVelocity = velocity;
		}

		#endregion

		#region Members

		/// <summary>
		/// Get the elapsed amount of time since the gravity effect took place
		/// </summary>
		/// <returns>The elapsed time since this gravity started</returns>
		public double ElapsedTime() {
			return (++TimeCount) / MagicNumbers.TIMEPART;
		}

		/// <summary>
		/// Get the new position for the item based on gravity
		/// </summary>
		/// <returns>The new position for the item</returns>
		public double NewPosition() {
			double t = ElapsedTime();
			double pos = InitialPosition;
			pos += (InitialVelocity * t);
			pos += (0.5 * MagicNumbers.GRAVITY * t * t);
			return pos;
		}

		#endregion
	}
}