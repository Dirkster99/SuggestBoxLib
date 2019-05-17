namespace SuggestBoxLib
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Enum for specifying where the ellipsis should appear.
    /// </summary>
    public enum EllipsisPlacement
    {
        /// <summary>
        /// Do not show an ellipsis in PathTrimming TexBlock
        /// </summary>
        None,

        /// <summary>
        /// Show an ellipsis in the center of PathTrimming TexBlock
        /// </summary>
        Center,

        Left,

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
                                        new UIPropertyMetadata(string.Empty));


        public EllipsisPlacement ShowElipses
        {
            get { return (EllipsisPlacement)GetValue(ShowElipsesProperty); }
            set { SetValue(ShowElipsesProperty, value); }
        }

        public static readonly DependencyProperty ShowElipsesProperty =
            DependencyProperty.Register("ShowElipses", typeof(EllipsisPlacement),
                typeof(PathTrimmingTextBlock), new PropertyMetadata(EllipsisPlacement.None));

        private FrameworkElement mContainer;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class Constructor
        /// </summary>
        public PathTrimmingTextBlock()
        {
            this.mContainer = null;

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
        #endregion properties

        #region methods
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
                this.mContainer = p;
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

                    this.mContainer = p;
                }
            }

            if (this.mContainer != null)
            {
                this.mContainer.SizeChanged += new SizeChangedEventHandler(this.container_SizeChanged);

                this.Text = this.GetTrimmedPath(this.mContainer.ActualWidth, this.ShowElipses);
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
            if (this.mContainer != null)
                this.mContainer.SizeChanged -= this.container_SizeChanged;
        }

        /// <summary>
        /// Trim the containing text (path) accordingly whenever the parent container chnages its size.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.mContainer != null)
                this.Text = this.GetTrimmedPath(this.mContainer.ActualWidth, this.ShowElipses);
        }

        /// <summary>
        /// Make sure we show the current string if visibility has changed to visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathTrimmingTextBlock_IsVisibleChanged(object sender,
                                                            DependencyPropertyChangedEventArgs e)
        {
            if (this.mContainer != null && (bool)e.NewValue == true)
            {
                this.Text = this.GetTrimmedPath(this.mContainer.ActualWidth, this.ShowElipses);
            }
        }

        /// <summary>
        /// Compute the text to display (with ellipsis) that fits the ActualWidth of the container
        /// </summary>
        /// <param name="width"></param>
        /// <param name="placement"></param>
        /// <returns></returns>
        private string GetTrimmedPath(double width,
                                      EllipsisPlacement placement)
        {
            string filename = string.Empty;
            string directory = string.Empty;

            switch (placement)
            {
                // We don't want no ellipses to be shown for a string shortener
                case EllipsisPlacement.None:
                    return this.Path;

                // Try to show a nice ellipses somewhere in the middle of the string
                case EllipsisPlacement.Center:
                    try
                    {
                        if (string.IsNullOrEmpty(this.Path) == false)
                        {
                            if (this.Path.Contains(string.Empty + System.IO.Path.DirectorySeparatorChar))
                            {
                                // Lets try to display the file name with priority
                                filename = System.IO.Path.GetFileName(this.Path);
                                directory = System.IO.Path.GetDirectoryName(this.Path);
                            }
                            else
                            {
                                // Cut this right in the middle since it does not seem to hold path info
                                int len      = this.Path.Length;
                                int firstLen = this.Path.Length / 2;
                                filename = this.Path.Substring(0, firstLen);

                                if (this.Path.Length >= (firstLen + 1))
                                    directory = this.Path.Substring(firstLen);
                            }
                        }
                        else
                            return string.Empty;
                    }
                    catch (Exception)
                    {
                        directory = this.Path;
                        filename = string.Empty;
                    }
                    break;

                case EllipsisPlacement.Left:
                    directory = this.Path;
                    filename = string.Empty;
                    break;

                case EllipsisPlacement.Right:
                    directory = string.Empty;
                    filename = this.Path;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(placement.ToString());
            }

            bool widthOK = false;
            bool changedWidth = false;
            int indexString = 0;

            if (placement == EllipsisPlacement.Left)
                indexString = 1;

            TextBlock block = new TextBlock();
            block.Style = this.Style;
            block.FontWeight = this.FontWeight;
            block.FontStyle = this.FontStyle;
            block.FontStretch = this.FontStretch;
            block.FontSize = this.FontSize;
            block.FontFamily = this.FontFamily;

            do
            {
                block.Text = FormatWith(placement, directory, filename);
                block.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                widthOK = block.DesiredSize.Width < width;

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
                            return string.Empty;
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
                return this.Path;

            if (block != null)   // Optimize for speed
                return block.Text;

            return FormatWith(placement, directory, filename);
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
        #endregion methods
    }
}
