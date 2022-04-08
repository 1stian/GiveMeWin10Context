using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace GiveMeWin10Context
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            StartCheck();
            CheckContext();
        }

        void StartCheck()
        {
            if (Environment.OSVersion.Version.Build < 22000)
            {
                MessageBox.Show("This is not windows 11?! Where am I???", "Yikes man...", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void buttonDoChanges_Click(object sender, RoutedEventArgs e)
        {
            DoChanges();
        }

        private void buttonRevertChanges_Click(object sender, RoutedEventArgs e)
        {
            RevertChanges();
        }

        void CheckContext()
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
            using (var key = hklm.OpenSubKey(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}"))
            {
                if (key == null)
                {
                    buttonDoChanges.IsEnabled = true;
                }
                else
                {
                    buttonDoChanges.IsEnabled = false;
                    buttonRevertChanges.IsEnabled = true;
                }
            }
        }

        void DoChanges()
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Bring it back?", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string topKey = @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}";
                string subKey = @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32";

                try
                {
                    Registry.CurrentUser.CreateSubKey(topKey);
                    Registry.CurrentUser.CreateSubKey(subKey);

                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(subKey, true))
                    {
                        key.SetValue(null, "", RegistryValueKind.String);
                    }

                    MessageBox.Show("You must reboot your computer to make these changes take effect!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBoxResult messageBoxResult2 = MessageBox.Show(ex.Message, "Error creating subKeys");
                }

                //String registryKey = @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32";
                //using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey))
                //{
                //    foreach (String subkeyName in key.GetSubKeyNames())
                //        Registry.CurrentUser.SetValue(subkeyName, "test", RegistryValueKind.String);
                //}

                CheckContext();
            }
        }

        void RevertChanges()
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string keyPath = @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}";

                try
                {
                    Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}");
                    MessageBox.Show("You must reboot your computer to make these changes take effect!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBoxResult resultBoxResult2 = MessageBox.Show(ex.Message, "Error reverting changes.....");
                }


                buttonDoChanges.IsEnabled = true;
                buttonRevertChanges.IsEnabled = false;
            }
        }
    }
}
