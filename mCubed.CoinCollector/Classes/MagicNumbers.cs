using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mCubed.CoinCollector {
	public static class MagicNumbers {
		public const double CANVASHEIGHT = MagicNumbers.TILESIZE * MagicNumbers.TILEVISUALROWS;
		public const double CANVASWIDTH = MagicNumbers.TILESIZE * MagicNumbers.TILEVISUALCOLS;

		public const int FRAMESPERSECOND = 62;
		public const int TILESIZE = 32;
		public const int TILEVISUALCOLS = 20;
		public const int TILEVISUALROWS = 16;

		public const double CALIBRATEDJUMPSPAN = 55d;
		public const double CALIBRATEDTIMEPART = 7d;

		public const double GRAVITY = 9.8;
		public const double TILEVELOCITY = 10d;
		public const double PLAYERVELOCITY = 46d;

		public const double TIMEPART = (CALIBRATEDJUMPSPAN * CALIBRATEDTIMEPART * GRAVITY) / (TILEVELOCITY * PLAYERVELOCITY * 2);
	}
}