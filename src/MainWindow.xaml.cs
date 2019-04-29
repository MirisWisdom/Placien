/**
 * Copyright (c) 2019 Emilian Roman
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.DialogResult;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Placien
{
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    private readonly Main _main;

    public MainWindow()
    {
      InitializeComponent();
      Version.Content = $"build-{Assembly.GetEntryAssembly().GetName().Version.Major:D4}";

      _main = (Main) DataContext;
      _main.Load();

      if (!File.Exists(_main.Sapien))
        SapienButton.Visibility = Visibility.Collapsed;
    }

    private void BrowsePlaceholder(object sender, RoutedEventArgs e)
    {
      var dialog = new OpenFileDialog
      {
        Filter = "Sapien bitmap (*.bitmap)|*.bitmap"
      };

      if (dialog.ShowDialog() == true)
        _main.Placeholder = dialog.FileName;
    }

    private void BrowseTarget(object sender, RoutedEventArgs e)
    {
      using (var dialog = new FolderBrowserDialog())
      {
        if (dialog.ShowDialog() == OK)
          _main.Directory = dialog.SelectedPath;
      }
    }

    private void BrowseSapien(object sender, RoutedEventArgs e)
    {
      var dialog = new OpenFileDialog
      {
        Filter = "Sapien executable (*.exe)|*.exe"
      };

      if (dialog.ShowDialog() == true)
        _main.Sapien = dialog.FileName;

      if (File.Exists(_main.Sapien))
        SapienButton.Visibility = Visibility.Visible;
    }

    private async void Save(object sender, RoutedEventArgs e)
    {
      SaveButton.IsEnabled = false;
      SaveButton.Content   = "Saving...";

      await Task.Run(() => { _main.Save(); });

      SaveButton.Content   = "Save";
      SaveButton.IsEnabled = true;
    }

    private void StartSapien(object sender, RoutedEventArgs e)
    {
      _main.StartSapien();
    }

    private void Binaries(object sender, RoutedEventArgs e)
    {
      Process.Start("https://dist.n2.network/placien/");
    }

    private void Source(object sender, RoutedEventArgs e)
    {
      Process.Start("https://cgit.n2.network/placien/");
    }

    private void ApplyPlaceholder(object sender, RoutedEventArgs e)
    {
      ApplyPlaceholderButton.IsEnabled = false;
      ApplyPlaceholderButton.Content   = "Applying...";

      Task.Run(() => { _main.Apply(); });

      ApplyPlaceholderButton.IsEnabled = true;
      ApplyPlaceholderButton.Content   = "Apply placeholder";
    }

    private void RestoreBitmaps(object sender, RoutedEventArgs e)
    {
      Task.Run(() => { _main.Restore(); });
    }
  }
}