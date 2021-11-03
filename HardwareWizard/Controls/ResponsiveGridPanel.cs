using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HardwareWizard.Core;
using HardwareWizard.Interfaces;

namespace HardwareWizard.Controls
{
    public class ResponsiveGridPanel : Canvas, IPanelUpdate
    {
        #region Properties
        // Property: Columns
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(ResponsiveGridPanel),
                new PropertyMetadata(1));

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        // Property: Gap
        public static readonly DependencyProperty GapProperty =
            DependencyProperty.Register("Gap", typeof(double), typeof(ResponsiveGridPanel),
                new PropertyMetadata(10.0));

        public double Gap
        {
            get { return (double)GetValue(GapProperty); }
            set { SetValue(GapProperty, value); }
        }

        // Property: HeightBasedAssegnation
        public static readonly DependencyProperty HeightBasedAssegnationProperty =
            DependencyProperty.Register("HeightBasedAssegnation", typeof(bool), typeof(ResponsiveGridPanel),
                new PropertyMetadata(false));

        public bool HeightBasedAssegnation
        {
            get { return (bool)GetValue(HeightBasedAssegnationProperty); }
            set { SetValue(HeightBasedAssegnationProperty, value); }
        }
        #endregion

        #region Variables
        private int availableColumns = -1;
        private double breakPoint = 0.45;
        private double columnWidth = 0;
        private List<ResponsiveGridItem> items;
        private bool isLoaded = false;
        #endregion

        #region Main
        public ResponsiveGridPanel()
        {
            this.items = new List<ResponsiveGridItem>();
            this.Loaded += ResponsiveGridPanel_Loaded;
            this.SizeChanged += ResponsiveGridPanel_SizeChanged;
            this.ClipToBounds = true;
        }
        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
        }

        private void ResponsiveGridPanel_Loaded(object sender, RoutedEventArgs e)
        {
            columnWidth = (1300 / Columns);
            isLoaded = true;
        }

        private void ResponsiveGridPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (isLoaded) {
                Update();
            }
        }
        #endregion

        #region Controls
        /// <summary>
        /// Aggiunge un elemento con una colonna preferita al layout.
        /// </summary>
        public void AddChild(Control child, int desiredColumn)
        {
            this.items.Add(new ResponsiveGridItem(child, -1, desiredColumn));
            this.Children.Add(child);
        }

        /// <summary>
        /// Aggiunge un elemento al layout.
        /// </summary>
        public void AddChild(Control child)
        {
            AddChild(child, -1);
        }
        /// <summary>
        /// Rimuove un elemento dal layout.
        /// </summary>
        public void RemoveChild(Control child)
        {
            foreach (ResponsiveGridItem item in items)
            {
                if (item.Control == child)
                {
                    this.items.Remove(item);
                    this.Children.Remove(child);
                    this.Update(true);
                }
            }
        }
        /// <summary>
        /// Ritorna l'elemento in base all'indice.
        /// </summary>
        public Control GetChild(int index)
        {
            return Children[index] as Control;
        }
        /// <summary>
        /// Aggiorna il layout e riposiziona gli elementi contenuti all'interno. 
        /// </summary>
        private void Update(bool elementAdded = false)
        {
            double actualWidth = this.ActualWidth;
            int columns = Utils.RoundWithBreak((actualWidth / columnWidth), breakPoint);
    
            // Calcola se necessario le colonne degli elementi in base a quelle disposibili.
            if (columns != availableColumns || elementAdded)
            {
                int responsiveColumnIndex = 0;
                double[] responsiveHeight = new double[columns];
                foreach (ResponsiveGridItem item in items)
                {
                    /*
                     * OLD RESPONSIVE ALGORITHM:
                     * - Incorrect item dynamic column assegnation. [X]
                     * - Not flexible [X]
                     */

                    bool canUseDesiredColumn = (item.DesiredColumn < columns && item.DesiredColumn != -1);
                    item.ActualColumn = canUseDesiredColumn ? item.DesiredColumn : responsiveColumnIndex;
                    if (HeightBasedAssegnation) {
                        responsiveColumnIndex = Array.IndexOf(responsiveHeight, responsiveHeight.Min());
                    } else {
                        responsiveColumnIndex = item.ActualColumn + 1 < columns ? item.ActualColumn + 1 : 0;
                    }
                    Canvas.SetTop(item.Control, responsiveHeight[item.ActualColumn]);
                    responsiveHeight[item.ActualColumn] += item.Control.ActualHeight + Gap;
                }
                this.availableColumns = columns;
                this.Height = responsiveHeight.Max();
            }

            // Aggiorna la larghezza degli elementi.
            foreach (ResponsiveGridItem item in items)
            {
                item.Control.Width = (actualWidth - ((availableColumns - 1) * Gap)) / availableColumns;
                Canvas.SetLeft(item.Control, (item.Control.Width * item.ActualColumn) + (item.ActualColumn * Gap));
            }
        }

        #endregion

        #region Interface
        public void OnChildResized()
        {
            this.Update(true);
        }
        #endregion
    }
}
