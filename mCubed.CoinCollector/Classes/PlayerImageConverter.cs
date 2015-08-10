using System;
using System.Windows.Data;
using mCubed.CoinCollector.Tiles;

namespace mCubed.CoinCollector {
	public class PlayerImageConverter : IValueConverter {
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value == null ? null : BaseTile.GenerateImage("Player" + value.ToString() + ".gif");
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		#endregion
	}
}