using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace mCubed.CoinCollector {
	public class OutlineText : FrameworkElement {
		#region Private Fields

		private FormattedText _formattedText;

		#endregion

		#region Private Methods

		/// <summary>
		/// Invoked when a dependency property has changed. Generate a new FormattedText object to display.
		/// </summary>
		/// <param name="d">OutlineText object whose property was updated.</param>
		/// <param name="e">Event arguments for the dependency property.</param>
		private static void OnOutlineTextInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((OutlineText)d).CreateText();
		}

		#endregion

		#region FrameworkElement Overrides

		/// <summary>
		/// OnRender override draws the geometry of the text and optional highlight.
		/// </summary>
		/// <param name="drawingContext">Drawing context of the OutlineText control.</param>
		protected override void OnRender(DrawingContext drawingContext) {
			// Set the width and height.
			Width = _formattedText.Width + Padding.Left + Padding.Right;
			Height = _formattedText.Height + Padding.Top + Padding.Bottom;

			// Draw an empty background first.
			drawingContext.DrawRectangle(Background, null, new Rect(0, 0, Width, Height));

			// Draw the outline based on the properties that are set.
			drawingContext.DrawGeometry(Fill, new Pen(Stroke, StrokeThickness), _formattedText.BuildGeometry(new Point(Padding.Left, Padding.Top)));
		}

		/// <summary>
		/// Create the outline geometry based on the formatted text.
		/// </summary>
		public void CreateText() {
			// Setup the font style and weight
			FontStyle fontStyle = Italic ? FontStyles.Italic : FontStyles.Normal;
			FontWeight fontWeight = Bold ? FontWeights.Bold : FontWeights.Medium;

			// Create the formatted text based on the properties set.
			_formattedText = new FormattedText(
			    Text,
			    CultureInfo.GetCultureInfo("en-us"),
			    FlowDirection.LeftToRight,
			    new Typeface(
				   Font,
				   fontStyle,
				   fontWeight,
				   FontStretches.Normal),
			    FontSize,
			    Brushes.Black
			    );
		}

		#endregion

		#region Dependency Properties

		/// <summary>
		/// Specifies the background brush to be displayed beneath the text.
		/// </summary>
		public Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}

		/// <summary>
		/// Identifies the Background dependency property.
		/// </summary>
		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
			"Background",
			typeof(Brush),
			typeof(OutlineText),
			new FrameworkPropertyMetadata(
				Brushes.Transparent,
				FrameworkPropertyMetadataOptions.AffectsRender,
				new PropertyChangedCallback(OnOutlineTextInvalidated),
				null
				)
			);

		/// <summary>
		/// Specifies whether the font should display Bold font weight.
		/// </summary>
		public bool Bold {
			get { return (bool)GetValue(BoldProperty); }
			set { SetValue(BoldProperty, value); }
		}

		/// <summary>
		/// Identifies the Bold dependency property.
		/// </summary>
		public static readonly DependencyProperty BoldProperty = DependencyProperty.Register(
		    "Bold",
		    typeof(bool),
		    typeof(OutlineText),
		    new FrameworkPropertyMetadata(
			   false,
			   FrameworkPropertyMetadataOptions.AffectsRender,
			   new PropertyChangedCallback(OnOutlineTextInvalidated),
			   null
			   )
		    );

		/// <summary>
		/// Specifies the brush to use for the fill of the formatted text.
		/// </summary>
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}

		/// <summary>
		/// Identifies the Fill dependency property.
		/// </summary>
		public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
		    "Fill",
		    typeof(System.Windows.Media.Brush),
		    typeof(OutlineText),
		    new FrameworkPropertyMetadata(
			   new SolidColorBrush(Colors.LightSteelBlue),
			   FrameworkPropertyMetadataOptions.AffectsRender,
			   new PropertyChangedCallback(OnOutlineTextInvalidated),
			   null
			   )
		    );

		/// <summary>
		/// The font to use for the displayed formatted text.
		/// </summary>
		public FontFamily Font {
			get { return (FontFamily)GetValue(FontProperty); }
			set { SetValue(FontProperty, value); }
		}

		/// <summary>
		/// Identifies the Font dependency property.
		/// </summary>
		public static readonly DependencyProperty FontProperty = DependencyProperty.Register(
		    "Font",
		    typeof(FontFamily),
		    typeof(OutlineText),
		    new FrameworkPropertyMetadata(
			   new FontFamily("Arial"),
			   FrameworkPropertyMetadataOptions.AffectsRender,
			   new PropertyChangedCallback(OnOutlineTextInvalidated),
			   null
			   )
		    );

		/// <summary>
		/// The current font size.
		/// </summary>
		public double FontSize {
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		/// <summary>
		/// Identifies the FontSize dependency property.
		/// </summary>
		public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
		    "FontSize",
		    typeof(double),
		    typeof(OutlineText),
		    new FrameworkPropertyMetadata(
			    (double)48.0,
			    FrameworkPropertyMetadataOptions.AffectsRender,
			    new PropertyChangedCallback(OnOutlineTextInvalidated),
			    null
			    )
		    );

		/// <summary>
		/// Specifies whether the font should display Italic font style.
		/// </summary>
		public bool Italic {
			get { return (bool)GetValue(ItalicProperty); }
			set { SetValue(ItalicProperty, value); }
		}

		/// <summary>
		/// Identifies the Italic dependency property.
		/// </summary>
		public static readonly DependencyProperty ItalicProperty = DependencyProperty.Register(
		    "Italic",
		    typeof(bool),
		    typeof(OutlineText),
		    new FrameworkPropertyMetadata(
			    false,
			    FrameworkPropertyMetadataOptions.AffectsRender,
			    new PropertyChangedCallback(OnOutlineTextInvalidated),
			    null
			    )
		    );

		/// <summary>
		/// Specifies the padding for the text to be displayed.
		/// </summary>
		public Thickness Padding {
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}

		/// <summary>
		/// Identified the Padding dependency property.
		/// </summary>
		public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
			"Padding",
			typeof(Thickness),
			typeof(OutlineText),
			new FrameworkPropertyMetadata(
				new Thickness(0, 0, 0, 0),
				FrameworkPropertyMetadataOptions.AffectsRender,
				new PropertyChangedCallback(OnOutlineTextInvalidated),
				null
				)
			);

		/// <summary>
		/// Specifies the brush to use for the stroke and optional hightlight of the formatted text.
		/// </summary>
		public Brush Stroke {
			get { return (Brush)GetValue(StrokeProperty); }
			set { SetValue(StrokeProperty, value); }
		}

		/// <summary>
		/// Identifies the Stroke dependency property.
		/// </summary>
		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
		    "Stroke",
		    typeof(Brush),
		    typeof(OutlineText),
		    new FrameworkPropertyMetadata(
			    new SolidColorBrush(Colors.Teal),
			    FrameworkPropertyMetadataOptions.AffectsRender,
			    new PropertyChangedCallback(OnOutlineTextInvalidated),
			    null
			    )
		    );

		/// <summary>
		/// The stroke thickness of the font.
		/// </summary>
		public double StrokeThickness {
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}

		/// <summary>
		/// Identifies the StrokeThickness dependency property.
		/// </summary>
		public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
		    "StrokeThickness",
		    typeof(double),
		    typeof(OutlineText),
		    new FrameworkPropertyMetadata(
			    (double)0,
			    FrameworkPropertyMetadataOptions.AffectsRender,
			    new PropertyChangedCallback(OnOutlineTextInvalidated),
			    null
			    )
		    );

		/// <summary>
		/// Specifies the text string to display.
		/// </summary>
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		/// <summary>
		/// Identifies the Text dependency property.
		/// </summary>
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
		    "Text",
		    typeof(string),
		    typeof(OutlineText),
		    new FrameworkPropertyMetadata(
			    "",
			    FrameworkPropertyMetadataOptions.AffectsRender,
			    new PropertyChangedCallback(OnOutlineTextInvalidated),
			    null
			    )
		    );

		#endregion
	}
}