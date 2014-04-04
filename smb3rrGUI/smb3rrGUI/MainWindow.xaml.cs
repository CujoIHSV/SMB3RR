using smb3rr;

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace smb3rrGUI
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            treeView1.ItemsSource = TreeViewModel.SetTree();
        }

        private void btnBrowseRom_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "NES ROM Files (*.nes)|*.nes|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                romPathBox.Text = openFileDialog.FileName;
            }

        }

        private void btnGenerateRom_Click(object sender, RoutedEventArgs e)
        {

            Randomizer randomizer = null;
            if (radioPredefined.IsChecked == true)
            {
                randomizer = new Randomizer(romPathBox.Text, recycledSeed.Text);
            }
            else
            {
                randomizer = new Randomizer(romPathBox.Text, (List<TreeViewModel>)treeView1.ItemsSource);
            }

            try
            {
                string newRomPath = randomizer.CreateRom();
                MessageBox.Show("Randomized ROM successfully created: " + newRomPath, "ROM Created");
            }
            catch (FileNotFoundException fnfExcept)
            {
                MessageBox.Show("Error: " + fnfExcept.FileName + " not found", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void treeView1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            radioRandom.IsChecked = true;
            radioPredefined.IsChecked = false;
        }

        private void recycledSeed_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            radioPredefined.IsChecked = true;
            radioRandom.IsChecked = false;
        }

    }

}
