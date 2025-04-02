using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace SecondStreetBuyer
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Item> Items { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            ItemListView.ItemsSource = Items;
        }

        public void LoadItems(IEnumerable<Item> items)
        {
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
