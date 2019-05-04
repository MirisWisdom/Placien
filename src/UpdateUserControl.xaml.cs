using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Environment;
using static System.Environment.SpecialFolder;
using static System.IO.File;
using static System.IO.Path;

namespace Placien
{
  public partial class UpdateUserControl : UserControl
  {
    private const string Header  = "HEADER.txt";
    private const string Address = "https://dist.n2.network/placien/";

    private int _version;

    public UpdateUserControl()
    {
      InitializeComponent();
      InitialiseUpdate();
      DownloadButton.Visibility = Visibility.Collapsed;
    }

    private async void InitialiseUpdate()
    {
      await Task.Run(() =>
      {
        try
        {
          using (var response = (HttpWebResponse) WebRequest.Create(Address + Header).GetResponse())
          using (var stream = response.GetResponseStream())
          using (var reader = new StreamReader(stream ?? throw new Exception("Response stream is null.")))
          {
            var serverVersion = int.Parse(reader.ReadLine()?.TrimEnd()
                                          ?? throw new Exception("Could not infer server-side version."));

            var clientVersion = Assembly.GetEntryAssembly().GetName().Version.Major;

            if (serverVersion <= clientVersion) return;

            _version = serverVersion;
          }
        }
        catch (Exception e)
        {
          var log = Combine(GetFolderPath(ApplicationData), "Placien", "exception.log");

          WriteAllText(log, e.ToString());
        }
      });

      if (_version <= 0) return;

      DownloadButton.Content    = $"Update available (build-{_version:D4}). Click to download!";
      DownloadButton.Visibility = Visibility.Visible;
    }

    private void Download(object sender, RoutedEventArgs e)
    {
      Process.Start($"https://dist.n2.network/placien/{_version:D4}/bin.zip");
    }
  }
}