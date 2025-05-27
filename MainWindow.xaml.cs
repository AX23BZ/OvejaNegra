using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WOLFSOFT001C8.Services;

namespace WOLFSOFT001C8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SincronizarProductos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SincronizarButton.IsEnabled = false;
                EstadoTextBlock.Text = "🔄 Sincronizando productos desde Azure...";
                SyncProgressBar.Visibility = Visibility.Visible;

                var sync = new SyncService();
                await Task.Run(() => sync.SincronizarProductosDesdeAzure());

                EstadoTextBlock.Text = "✅ Sincronización completada.";
            }
            catch (Exception ex)
            {
                EstadoTextBlock.Text = "❌ Error durante la sincronización.";
                MessageBox.Show("Error:\n" + ex.Message);
            }
            finally
            {
                SincronizarButton.IsEnabled = true;
                SyncProgressBar.Visibility = Visibility.Collapsed;
            }
        }
    }
}