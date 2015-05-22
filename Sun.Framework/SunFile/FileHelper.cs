using System;
using System.IO;

namespace Sun.Framework.SunFile
{
   public class FileHelper
    {
      public static void CopyDirectory(string srcDir, string tgtDir)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tgtDir);
            CopyDirectory(source,target);
        }
      public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
      {
          if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
          {
              throw new Exception("父目录不能拷贝到子目录！");
          }

          if (!source.Exists)
          {
              return;
          }

          if (!target.Exists)
          {
              target.Create();
          }

          FileInfo[] files = source.GetFiles();

          for (int i = 0; i < files.Length; i++)
          {
              File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
          }
          DirectoryInfo[] dirs = source.GetDirectories();

          for (int j = 0; j < dirs.Length; j++)
          {
              CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
          }
      }
    }
}
