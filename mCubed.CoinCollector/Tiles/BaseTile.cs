using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;

namespace mCubed.CoinCollector.Tiles {
	public abstract class BaseTile : INotifyPropertyChanged {
		#region Static Members

		private static readonly Dictionary<string, BitmapImage> _imageDict = new Dictionary<string, BitmapImage>();

		/// <summary>
		/// Generate a bitmap image from the given image name including the extension
		/// </summary>
		/// <param name="imageName">The filename of the image with the extension at the end</param>
		/// <returns>The generated bitmap image for visual use</returns>
		public static BitmapImage GenerateImage(string imageName) {
			if(!_imageDict.ContainsKey(imageName))
				_imageDict[imageName] = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageName));
			return _imageDict[imageName];
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Data Store

		private readonly FrameworkElement _representation;

		#endregion

		#region Properties

		/// <summary>
		/// Get/set the bottom position of the tile
		/// </summary>
		public double Bottom {
			get { return Top + Height; }
			set { Top = value - Height; }
		}

		/// <summary>
		/// Get whether or not the side must be open when checking collision
		/// </summary>
		public virtual bool CheckOpenSide { get { return true; } }

		/// <summary>
		/// Get/set the height of the tile
		/// </summary>
		public double Height {
			get { return Representation.Height; }
			set { Representation.Height = value; }
		}

		/// <summary>
		/// Get/set the left position of the tile
		/// </summary>
		public double Left {
			get { return Canvas.GetLeft(Representation); }
			set { Canvas.SetLeft(Representation, value); }
		}

		/// <summary>
		/// Get the left offset for the tile
		/// </summary>
		public virtual double LeftOffset { get { return 0; } }

		/// <summary>
		/// Get/set the location of the tile
		/// </summary>
		public Point Location {
			get { return new Point(Left, Top); }
			set {
				Left = value.X;
				Top = value.Y;
			}
		}

		/// <summary>
		/// Get the rectangle that the tile occupies
		/// </summary>
		public Rect Rect { get { return new Rect(Location, Size); } }

		/// <summary>
		/// Get the visual representation for the tile
		/// </summary>
		public FrameworkElement Representation { get { return _representation; } }

		/// <summary>
		/// Get/set the right position of the tile
		/// </summary>
		public double Right {
			get { return Left + Width; }
			set { Left = value - Width; }
		}

		/// <summary>
		/// Get/set the size of the tile
		/// </summary>
		public Size Size {
			get { return new Size(Width, Height); }
			set {
				Width = value.Width;
				Height = value.Height;
			}
		}

		/// <summary>
		/// Get/set the top position of the tile
		/// </summary>
		public double Top {
			get { return Canvas.GetTop(Representation); }
			set { Canvas.SetTop(Representation, value); }
		}

		/// <summary>
		/// Get the top offset for the tile
		/// </summary>
		public virtual double TopOffset { get { return 0; } }

		/// <summary>
		/// Get/set the width of the tile
		/// </summary>
		public double Width {
			get { return Representation.Width; }
			set { Representation.Width = value; }
		}

		/// <summary>
		/// Get/set the z-index for this tile
		/// </summary>
		public int ZIndex {
			get { return Panel.GetZIndex(Representation); }
			set { Panel.SetZIndex(Representation, value); }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Construct a base tile with the given representation for the tile
		/// </summary>
		/// <param name="representation">The representation for the tile</param>
		public BaseTile(FrameworkElement representation) {
			_representation = representation;
			Width = MagicNumbers.TILESIZE;
			Height = MagicNumbers.TILESIZE;
		}

		#endregion

		#region Collision Members

		/// <summary>
		/// Determine if two tiles have collided
		/// </summary>
		/// <param name="tile">The tile to check against</param>
		/// <returns>True if the tiles have collided or false otherwise</returns>
		public bool IsCollided(BaseTile tile) {
			return tile != null && tile != this && IsCollided(tile.Rect);
		}

		/// <summary>
		/// Determine if two rectangles have collided
		/// </summary>
		/// <param name="rect">The rectangle to check against</param>
		/// <returns>True if the tiles have collided or false otherwise</returns>
		public bool IsCollided(Rect rect) {
			return Rect.IntersectsWith(rect);
		}

		/// <summary>
		/// Determine the prominent collision side of the two tiles
		/// </summary>
		/// <param name="tile">The tile to check against</param>
		/// <returns>The prominent side for the collision of the two tiles</returns>
		public CollisionSide GetCollisionSide(BaseTile tile) {
			CollisionSide side = CollisionSide.None;
			if (IsCollided(tile)) {
				// Calculate offsets
				double topOffset = Bottom - tile.Top;
				double botOffset = tile.Bottom - Top;
				double leftOffset = Right - tile.Left;
				double rightOffset = tile.Right - Left;

				// Start with the top side
				double newOffset = topOffset;
				side = CollisionSide.Top;

				// Check the bottom side
				side = botOffset > newOffset ? CollisionSide.Bottom : side;
				newOffset = Math.Max(newOffset, botOffset);

				// Check the left side
				side = leftOffset > newOffset ? CollisionSide.Left : side;
				newOffset = Math.Max(newOffset, leftOffset);

				// Check the right side
				side = rightOffset > newOffset ? CollisionSide.Right : side;
				newOffset = Math.Max(newOffset, rightOffset);

				// Check if no side
				side = newOffset > MagicNumbers.TILESIZE * 2 ? CollisionSide.None : side;
			}
			return side;
		}

		#endregion

		#region Abstract Members

		public abstract CollisionEventArgs CollideWith(Player tile, CollisionSide side);

		#endregion
	}
}