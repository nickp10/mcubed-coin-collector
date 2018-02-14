using System;
using System.Collections.Generic;
using mCubed.CoinCollector.Tiles;
using System.Linq;

namespace mCubed.CoinCollector.Maps {
	public class StreamMap : BaseMap {
		#region Data Store

		private IEnumerable<BaseTile> _tiles = Enumerable.Empty<BaseTile>();

		#endregion

		#region Constructor

		/// <summary>
		/// Create a new stream map with the given lines
		/// </summary>
		/// <param name="lines">The lines to create the map from</param>
		public StreamMap(string[] lines) {
			_tiles = ParseLines(lines).ToArray();
		}

		#endregion

		#region BaseMap Members

		/// <summary>
		/// Generate a map to be used with the given controller
		/// </summary>
		/// <param name="controller">The controller that will maintain the map</param>
		public override void GenerateMap(CoinController controller) {
			foreach (BaseTile tile in _tiles) {
				if (tile is Player) {
					// Move the player tile
					controller.Player.Top = tile.Top;
					controller.Player.Left = tile.Left;
				} else {
					// Create a shallow copy of the instance
					BaseTile t = (BaseTile)Activator.CreateInstance(tile.GetType());
					t.Top = tile.Top;
					t.Left = tile.Left;

					// Place the tile
					controller.PlaceTile(t, true);
				}
			}
		}

		#endregion

		#region Stream Members

		/// <summary>
		/// Parse the given lines into the map
		/// </summary>
		/// <param name="lines">The lines to create the map from</param>
		private IEnumerable<BaseTile> ParseLines(string[] lines) {
			// Setup from the first two lines
			Name = lines[0];
			MapNumber = int.Parse(lines[1]);
			int rows = lines.Length - 2;
			int yOffset = rows < MagicNumbers.TILEVISUALROWS ? MagicNumbers.TILEVISUALROWS - rows : 0;
			int maxCols = 0;

			// Setup all tiles based on the x,y location
			for (int i = 2; i < lines.Length; i++) {
				int row = i - 2 + yOffset;
				string line = lines[i];
				maxCols = Math.Max(maxCols, line.Length);
				for (int j = 0; j < line.Length; j++) {
					int col = j;
					BaseTile tile = Generate(line[j]);
					if (tile != null) {
						tile.Top = MagicNumbers.TILESIZE * row + tile.TopOffset;
						tile.Left = MagicNumbers.TILESIZE * col + tile.LeftOffset;
						yield return tile;
					}
				}
			}

			// Set the height and width
			MapHeight = MagicNumbers.TILESIZE * (rows + yOffset);
			MapWidth = MagicNumbers.TILESIZE * maxCols;
		}

		/// <summary>
		/// Generate a base tile based off the character representation
		/// </summary>
		/// <param name="c">The character representation for the tile</param>
		/// <returns>The tile that was generated</returns>
		private BaseTile Generate(char c) {
			BaseTile tile = null;
			if (c == 'M')
				tile = new MegaCoin();
			else if (c == 'C')
				tile = new Coin();
			else if (c == 'F')
				tile = new Finish();
			else if (c == 'P')
				tile = new Player();
			else if (c == 'B')
				tile = new WallBlue();
			else if (c == 'G')
				tile = new WallGreen();
			else if (c == 'R')
				tile = new WallRed();
			else if (c == 'W')
				tile = new WallWhite();
			else if (c == 'Y')
				tile = new WallYellow();
			return tile;
		}

		#endregion
	}
}