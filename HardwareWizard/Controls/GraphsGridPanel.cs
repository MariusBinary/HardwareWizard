using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;

namespace HardwareWizard.Controls
{
    public class GraphsGridPanel : Grid
    {
        #region Properties
        public static readonly DependencyProperty GapProperty =
            DependencyProperty.Register("Gap", typeof(double), typeof(GraphsGridPanel),
                new PropertyMetadata(10.0));

        public double Gap
        {
            get { return (double)GetValue(GapProperty); }
            set { SetValue(GapProperty, value); }
        }
        #endregion

        #region Variables
        private int lastIndex = 0;
        private Dictionary<string, GraphControl> graphs;
        #endregion

        #region Main
        public GraphsGridPanel()
        {
            this.graphs = new Dictionary<string, GraphControl>();
            this.Loaded += ResponsiveGridPanel_Loaded;
        }

        private void ResponsiveGridPanel_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (GraphControl graph in graphs.Values) {
                AddToLayout(graph);
            }
        }
        #endregion

        #region Controls
        public void AddTag(string tag)
        {
            if (graphs.ContainsKey(tag)) return;

            var graph = new GraphControl() {
                FixedXPixels = 130,
                FixedYPixels = 10,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6AD35E")),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#40444C"))
            };

            graphs.Add(tag, graph);

            if (this != null) {
                //AddToLayout(graph);
            }
        }

        public void RemoveTag(string tag)
        {
            this.Children.Remove(graphs[tag]);
            if (graphs.ContainsKey(tag))
            {
                graphs.Remove(tag);
            }
        }
        /// <summary>
        /// Ritorna l'elemento in base all'indice.
        /// </summary>
        public GraphControl GetGraph(string tag)
        {
            return graphs[tag];
        }
        #endregion

        #region Helpers
        private void AddToLayout(GraphControl graph)
        {
            int row = (lastIndex / 3);
            int col = (lastIndex - (row * 3));

            if (col > this.ColumnDefinitions.Count){
                ExpandColumns();
            }

            if (row > this.RowDefinitions.Count) {
                ExpandRows();
            }

            Grid.SetColumn(graph, col);
            Grid.SetRow(graph, row);
            this.Children.Add(graph);

            lastIndex += ((lastIndex + 1) % 3 == 0 && lastIndex != 0) ? 4 : 2;
        }
        private int ExpandColumns()
        {
            if (this.RowDefinitions.Count == 0) {
                this.ColumnDefinitions.Add(new ColumnDefinition() {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }
            this.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(Gap, GridUnitType.Pixel)
            });
            this.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            return this.ColumnDefinitions.Count - 1;
        }
        private int ExpandRows()
        {
            if (this.RowDefinitions.Count == 0) {
                this.RowDefinitions.Add(new RowDefinition() {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }
            this.RowDefinitions.Add(new RowDefinition() {
                Height = new GridLength(Gap, GridUnitType.Pixel)
            });
            this.RowDefinitions.Add(new RowDefinition() {
                Height = new GridLength(1, GridUnitType.Star)
            });

            return this.RowDefinitions.Count - 1;
        }
        #endregion
    }
}
