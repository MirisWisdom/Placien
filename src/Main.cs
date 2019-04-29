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

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Placien.Annotations;
using static System.Environment;
using static System.Environment.SpecialFolder;
using static System.IO.Directory;
using static System.IO.FileAccess;
using static System.IO.FileMode;
using static System.IO.Path;
using static System.Text.Encoding;

namespace Placien
{
  public class Main : INotifyPropertyChanged
  {
    private static readonly string File = Combine(GetFolderPath(ApplicationData), "Placien", "main.bin");

    private string _directory   = string.Empty;
    private string _filter      = string.Empty;
    private string _placeholder = string.Empty;
    private string _sapien      = string.Empty;

    public string Directory
    {
      get => _directory;
      set
      {
        if (value == _directory) return;
        _directory = value;
        OnPropertyChanged();
      }
    }

    public string Filter
    {
      get => _filter;
      set
      {
        if (value == _filter) return;
        _filter = value;
        OnPropertyChanged();
      }
    }

    public string Placeholder
    {
      get => _placeholder;
      set
      {
        if (value == _placeholder) return;
        _placeholder = value;
        OnPropertyChanged();
      }
    }

    public string Sapien
    {
      get => _sapien;
      set
      {
        if (value == _sapien) return;
        _sapien = value;
        OnPropertyChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void Apply()
    {
      HXE.Placeholder.Commit(Placeholder, Directory, Filter);
    }

    public void Restore()
    {
      HXE.Placeholder.Revert(Directory);
    }

    public void Save()
    {
      CreateDirectory(Combine(GetFolderPath(ApplicationData), "Placien"));

      using (var fs = new FileStream(File, Create, Write))
      using (var ms = new MemoryStream(2048))
      using (var bw = new BinaryWriter(ms))
      {
        ms.Position = 0;

        var signature   = Unicode.GetBytes("~yumiris");
        var placeholder = UTF8.GetBytes(Placeholder);
        var directory   = UTF8.GetBytes(Directory);
        var filter      = UTF8.GetBytes(Filter);
        var sapien      = UTF8.GetBytes(Sapien);

        bw.Write(signature);                           /* vanity signature */
        bw.Write(new byte[0032 - signature.Length]);   /* padding */
        bw.Write(placeholder);                         /* placeholder */
        bw.Write(new byte[0256 - placeholder.Length]); /* padding */
        bw.Write(directory);                           /* directory */
        bw.Write(new byte[0256 - directory.Length]);   /* padding */
        bw.Write(filter);                              /* filter */
        bw.Write(new byte[0064 - filter.Length]);      /* padding */
        bw.Write(sapien);                              /* sapien */
        bw.Write(new byte[0256 - sapien.Length]);      /* padding */
        bw.Write(new byte[2048 - ms.Position]);        /* padding */

        ms.Position = 0;
        ms.CopyTo(fs);
      }
    }

    public void Load()
    {
      if (!System.IO.File.Exists(File))
        Save();

      using (var fs = new FileStream(File, Open, Read))
      using (var ms = new MemoryStream(2048))
      using (var br = new BinaryReader(ms))
      {
        fs.CopyTo(ms);

        ms.Position = 32;
        Placeholder = UTF8.GetString(br.ReadBytes(256)).TrimEnd('\0');
        Directory   = UTF8.GetString(br.ReadBytes(256)).TrimEnd('\0');
        Filter      = UTF8.GetString(br.ReadBytes(064)).TrimEnd('\0');
        Sapien      = UTF8.GetString(br.ReadBytes(256)).TrimEnd('\0');
      }
    }

    public void StartSapien()
    {
      Process.Start(Sapien);
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}