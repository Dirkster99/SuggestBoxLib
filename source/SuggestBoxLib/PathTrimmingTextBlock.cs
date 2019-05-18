namespace SuggestBoxLib
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

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

    /// <summary>
    /// This PathTrimmingTextBlock textblock attaches itself to the events of a parent container and
    /// displays a trimmed path text when the size of the parent (container) is changed.
    /// 
    /// http://www.codeproject.com/Tips/467054/WPF-PathTrimmingTextBlock
    /// 
    /// Make sure you set, if you use this within an ListBox or ListView:
    ///           ScrollViewer.HorizontalScrollBarVisibility="Disabled"
    /// </summary>
    public class PathTrimmingTextBlock : TextBlock
    {
        #region fields
        /// <summary>
        /// Path dependency property that stores the trimmed path
        /// </summary>
        private static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path",
                                        typeof(string),
                                        typeof(PathTrimmingTextBlock),
                                        new UIPropertyMetadata(string.Empty, OnPathChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="ShowElipses"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowElipsesProperty =
            DependencyProperty.Register("ShowElipses", typeof(EllipsisPlacement),
                typeof(PathTrimmingTextBlock), new PropertyMetadata(EllipsisPlacement.None, OnShowElipsesChanged));

        private FrameworkElement _Container;

        private readonly TextBlock _MeasureBlock;
        private double _lastMeasuredWith;
        private string _lastString;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class Constructor
        /// </summary>
        public PathTrimmingTextBlock()
        {
            _MeasureBlock = new TextBlock();
            BindOneWay(this, "Style", _MeasureBlock, TextBlock.StyleProperty);
            BindOneWay(this, "FontWeight", _MeasureBlock, TextBlock.FontWeightProperty);
            BindOneWay(this, "FontStyle", _MeasureBlock, TextBlock.FontStyleProperty);
            BindOneWay(this, "FontStretch", _MeasureBlock, TextBlock.FontStretchProperty);
            BindOneWay(this, "FontSize", _MeasureBlock, TextBlock.FontSizeProperty);
            BindOneWay(this, "FontFamily", _MeasureBlock, TextBlock.FontFamilyProperty);

            this.Loaded += new RoutedEventHandler(this.PathTrimmingTextBlock_Loaded);
            this.Unloaded += new RoutedEventHandler(this.PathTrimmingTextBlock_Unloaded);
            this.IsVisibleChanged += PathTrimmingTextBlock_IsVisibleChanged;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Path dependency property that stores the trimmed path
        /// </summary>
        public string Path
        {
            get { return (string)this.GetValue(PathProperty); }
            set { this.SetValue(PathProperty, value); }
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
        /// Updates the trimmed text based on current measurements, text input, and Show Ellipses parameter.
        /// </summary>
        protected virtual void UpdateText()
        {
            if (_Container != null)
                this.Text = this.GetTrimmedPath(this.Path, _Container.ActualWidth, this.ShowElipses);
            //// else
            ////  throw new InvalidOperationException("PathTrimmingTextBlock must have a container such as a Grid.");
        }

        /// <summary>
        /// Update measured and trimmed text output if text input has changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PathTrimmingTextBlock).UpdateText();
        }

        /// <summary>
        /// Update measured and trimmed text output if text trimming option has changed.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnShowElipsesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PathTrimmingTextBlock).UpdateText();
        }

        /// <summary>
        /// Textblock is constructed and start its live - lets attach to the
        /// size changed event handler of the containing parent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathTrimmingTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement p = null;
            if (this.Parent is FrameworkElement)
            {
                p = (FrameworkElement)this.Parent;
                _Container = p;
            }
            else
            {

                if (this.Parent is DependencyObject dp)
                {
                    for (DependencyObject parent = LogicalTreeHelper.GetParent(dp as DependencyObject);
                         parent != null;
                         parent = LogicalTreeHelper.GetParent(parent as DependencyObject))
                    {
                        p = parent as FrameworkElement;

                        if (p != null)
                            break;
                    }

                    _Container = p;
                }
            }

            if (_Container != null)
            {
                _Container.SizeChanged += new SizeChangedEventHandler(this.container_SizeChanged);

                UpdateText();
            }
            //// else
            ////  throw new InvalidOperationException("PathTrimmingTextBlock must have a container such as a Grid.");
        }

        /// <summary>
        /// Remove custom event handlers and clean-up on unload.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathTrimmingTextBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_Container != null)
                _Container.SizeChanged -= this.container_SizeChanged;
        }

        /// <summary>
        /// Trim the containing text (path) accordingly whenever the parent container chnages its size.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateText();
        }

        /// <summary>
        /// Make sure we show the current string if visibility has changed to visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathTrimmingTextBlock_IsVisibleChanged(object sender,
                                                            DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
                UpdateText();
        }

        /// <summary>
        /// Compute the text to display (with ellipsis) that fits the ActualWidth of the container
        /// </summary>
        /// <param name="inputString">Input string to measure whether it fits into container width or not.</param>
        /// <param name="containerWidth">ActualWidth restriction by container element (eg.: Grid)</param>
        /// <param name="placement"></param>
        /// <returns></returns>
        private string GetTrimmedPath(string inputString,
                                      double containerWidth,
                                      EllipsisPlacement placement)
        {
            // Lets not measure the same thing twice
            if (Math.Abs(_lastMeasuredWith - containerWidth) <= 0.5 &&
                string.Compare(_lastString, inputString) == 0)
               return _MeasureBlock.Text;

            _lastMeasuredWith = containerWidth;
            _lastString = inputString;

            string filename = string.Empty;
            string directory = string.Empty;

            switch (placement)
            {
                // We don't want no ellipses to be shown for a string shortener
                case EllipsisPlacement.None:
                    _MeasureBlock.Text = inputString;
                    return inputString;

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
                                int len      = inputString.Length;
                                int firstLen = inputString.Length / 2;
                                filename = inputString.Substring(0, firstLen);

                                if (inputString.Length >= (firstLen + 1))
                                    directory = inputString.Substring(firstLen);
                            }
                        }
                        else
                        {
                            _MeasureBlock.Text = string.Empty;
                            return string.Empty;
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

            bool widthOK = false;
            bool changedWidth = false;
            int indexString = 0;

            if (placement == EllipsisPlacement.Left)
                indexString = 1;

            do
            {
                _MeasureBlock.Text = FormatWith(placement, directory, filename);
                _MeasureBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                widthOK = _MeasureBlock.DesiredSize.Width < containerWidth;

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
                            _MeasureBlock.Text = string.Empty;
                            return string.Empty;
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

            if (changedWidth == false)
            {
                _MeasureBlock.Text = inputString;
                return inputString;
            }

            return _MeasureBlock.Text;
        }

        /// <summary>
        /// Extend the string constructor with a string.Format like syntax.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="placing"></param>
        /// <returns></returns>
        public static string FormatWith(EllipsisPlacement placing,
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

        private void BindOneWay(FrameworkElement soureOfBinding,
                                string sourcePathName,
                                FrameworkElement destinationOfBinding,
                                DependencyProperty destinationProperty)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(sourcePathName);
            binding.Source = soureOfBinding;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(destinationOfBinding, destinationProperty, binding);
        }
        #endregion methods
    }
}
