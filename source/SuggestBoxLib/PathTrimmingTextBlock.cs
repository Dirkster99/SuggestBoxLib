namespace SuggestBoxLib
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;

	/// <summary>
	/// Enum for specifying where the ellipsis should appear.
	/// </summary>
	public enum EllipsisPlacement
	{
		/// <summary>
		/// Do not show an ellipsis in the Text of the PathTrimming TexBlock
		/// </summary>
		None,

		/// <summary>
		/// Show an ellipsis in the center of the Text in the PathTrimming TexBlock
		/// </summary>
		Center,

		/// <summary>
		/// Show an ellipsis in the left side of the Text in the PathTrimming TexBlock
		/// </summary>
		Left,

		/// <summary>
		/// Show an ellipsis in the right side of the Text in the PathTrimming TexBlock
		/// </summary>
		Right
	}

	/*********************
    https://www.codeproject.com/Tips/467054/WPF-PathTrimmingTextBlock
    Improved Implementation:
    Forum Entry from 2017:
    Thanks for this, it was a good start. I've made some modifications to introduce a the following improvements:
    1. No longer necessary to be inside a Grid
    2. Won't throw exception if the path is not in a valid format
    3. Measures text size more accurately (ujb1's suggestion - thanks)
    4. Tries to obtain as much space as needed to display complete path
    5. Uses MeasureOverride to size itself (I'm not an expert, but I understand this is more efficient than using the SizeChanged event)
    6. Updates the displayed path if the path is changed via Data Binding
    **********************/

	/// <summary>
	/// A TextBlock like control that provides special text trimming logic
	/// designed for a file or folder path.
	/// </summary>
	public class PathTrimmingTextBlock : UserControl
	{
		#region fields

		/// <summary>
		/// Implements the backing store of the <see cref="Path"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty PathProperty =
			DependencyProperty.Register("Path", typeof(string),
				typeof(PathTrimmingTextBlock), new UIPropertyMetadata("", OnPathChanged));

		/// <summary>
		/// Implements the backing store of the <see cref="ShowElipses"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ShowElipsesProperty =
			DependencyProperty.Register("ShowElipses", typeof(EllipsisPlacement),
				typeof(PathTrimmingTextBlock),
				new PropertyMetadata(EllipsisPlacement.Center, OnShowElipsesChanged));

		private readonly TextBlock _TextBlock;

		#endregion fields

		#region ctors

		/// <summary>
		/// Class constructor
		/// </summary>
		public PathTrimmingTextBlock()
		{
			_TextBlock = new TextBlock();
			AddChild(_TextBlock);
		}

		#endregion ctors

		#region properties

		/// <summary>
		/// Gets/sets the path to display.
		/// The text that is actually displayed will be trimmed with Ellipses.
		/// </summary>
		public string Path
		{
			get { return (string)GetValue(PathProperty); }
			set { SetValue(PathProperty, value); }
		}

		/// <summary>
		/// Gets/sets whether the Path string should be shortend and displayed with an elipses or not.
		/// </summary>
		public EllipsisPlacement ShowElipses
		{
			get { return (EllipsisPlacement)GetValue(ShowElipsesProperty); }
			set { SetValue(ShowElipsesProperty, value); }
		}

		#endregion properties

		#region methods

		/// <summary>
		/// Called to remeasure a control.
		/// </summary>
		/// <param name="constraint">The maximum size that the method can return.</param>
		/// <returns>The size of the control, up to the maximum specified by constraint.</returns>
		protected override Size MeasureOverride(Size constraint)
		{
			base.MeasureOverride(constraint);

			// This is where the control requests to be as large
			// as is needed while fitting within the given bounds
			var meas = GetTrimmedPath(Path, constraint, this.ShowElipses, this);

			// Update the text with or without short version + Ellipses '...' depending on available space
			_TextBlock.Text = meas.Item1;

			return meas.Item2;
		}

		/// <summary>
		/// Compute the text to display (with ellipsis) that fits the ActualWidth of the container
		/// </summary>
		/// <param name="inputString">Input string to measure whether it fits into container width or not.</param>
		/// <param name="constraint">ActualWidth restriction by container element (eg.: Grid)</param>
		/// <param name="placement"></param>
		/// <param name="ctrl"></param>
		/// <returns></returns>
		private static Tuple<string, Size> GetTrimmedPath(string inputString,
														  Size constraint,
														  EllipsisPlacement placement,
														  Control ctrl)
		{
			Size size = new Size();
			string filename = string.Empty;
			string directory = string.Empty;

			if (constraint.Width != double.PositiveInfinity)
				size.Width = constraint.Width;

			if (constraint.Height != double.PositiveInfinity)
				size.Height = constraint.Height;

			switch (placement)
			{
				// We don't want no ellipses to be shown for a string shortener
				case EllipsisPlacement.None:
					size = MeasureString(inputString, ctrl);
					return new Tuple<string, Size>(inputString, size);

				// Try to show a nice ellipses somewhere in the middle of the string
				case EllipsisPlacement.Center:
					try
					{
						if (string.IsNullOrEmpty(inputString) == false)
						{
							if (inputString.Contains(string.Empty + System.IO.Path.DirectorySeparatorChar))
							{
								// Lets try to display the file name with priority
								filename = System.IO.Path.GetFileName(inputString);
								directory = System.IO.Path.GetDirectoryName(inputString);
							}
							else
							{
								// Cut this right in the middle since it does not seem to hold path info
								int len = inputString.Length;
								int firstLen = inputString.Length / 2;
								filename = inputString.Substring(0, firstLen);

								if (inputString.Length >= (firstLen + 1))
									directory = inputString.Substring(firstLen);
							}
						}
						else
						{
							size = MeasureString(string.Empty, ctrl);
							return new Tuple<string, Size>(inputString, size);
						}
					}
					catch (Exception)
					{
						directory = inputString;
						filename = string.Empty;
					}
					break;

				case EllipsisPlacement.Left:
					directory = inputString;
					filename = string.Empty;
					break;

				case EllipsisPlacement.Right:
					directory = string.Empty;
					filename = inputString;
					break;

				default:
					throw new ArgumentOutOfRangeException(placement.ToString());
			}

			double fudgeValue = 3.0;
			bool widthOK = false;
			int indexString = 0;
			string path = inputString;
			bool changedWidth = false;

			if (placement == EllipsisPlacement.Left)
				indexString = 1;

			do
			{
				path = FormatWith(placement, directory, filename);
				size = MeasureString(path, ctrl);

				widthOK = (size.Width + fudgeValue) < constraint.Width;

				if (widthOK == false)
				{
					if (directory.Length == 0)
					{
						if (filename.Length > 0)
						{
							changedWidth = true;
							filename = filename.Substring(indexString, filename.Length - 1);
						}
						else
						{
							size = MeasureString(string.Empty, ctrl);
							return new Tuple<string, Size>(inputString, size);
						}
					}
					else
					{
						changedWidth = true;
						directory = directory.Substring(indexString, directory.Length - 1);
					}
				}
			}
			while (widthOK == false);

			size.Width += fudgeValue;

			if (changedWidth == false)
				return new Tuple<string, Size>(inputString, size);

			return new Tuple<string, Size>(path, size);
		}

		/// <summary>
		/// Returns the size of the given string if it were to be rendered.
		/// </summary>
		/// <param name="str">The string to measure.</param>
		/// <param name="ctrl"></param>
		/// <returns>The size of the string.</returns>
		private static Size MeasureString(string str, Control ctrl)
		{
			var typeFace = new Typeface(ctrl.FontFamily, ctrl.FontStyle, ctrl.FontWeight, ctrl.FontStretch);
			var text = new FormattedText(str, CultureInfo.CurrentCulture,
											  ctrl.FlowDirection, typeFace,
											  ctrl.FontSize, ctrl.Foreground);

			return new Size(text.Width, text.Height);
		}

		/// <summary>
		/// Extend the string constructor with a string.Format like syntax.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="placing"></param>
		/// <returns></returns>
		private static string FormatWith(EllipsisPlacement placing,
										params object[] args)
		{
			string formatString;

			switch (placing)
			{
				case EllipsisPlacement.None:
					formatString = "{0}{1}";
					break;

				case EllipsisPlacement.Left:
					formatString = "...{0}{1}";
					break;

				case EllipsisPlacement.Center:
					formatString = "{0}...{1}";
					break;

				case EllipsisPlacement.Right:
					formatString = "{0}{1}...";
					break;

				default:
					throw new ArgumentOutOfRangeException(placing.ToString());
			}

			return string.Format(CultureInfo.InvariantCulture, formatString, args);
		}

		/// <summary>
		/// Update measured and trimmed text output if path text property has changed.
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PathTrimmingTextBlock @this = (PathTrimmingTextBlock)d;

			// This element will be re-measured
			// The text will be updated during that process
			@this.InvalidateMeasure();
		}

		/// <summary>
		/// Update measured and trimmed text output if text trimming option has changed.
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnShowElipsesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PathTrimmingTextBlock @this = (PathTrimmingTextBlock)d;

			// This element will be re-measured
			// The text will be updated during that process
			@this.InvalidateMeasure();
		}

		#endregion methods
	}
}