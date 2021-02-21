using System.IO;

namespace BluePointLilac.Methods
{
    public static class DirectoryEx
    {
        public static void CopyTo(string srcDirPath, string dstDirPath)
        {
            DirectoryInfo srcDi = new DirectoryInfo(srcDirPath);
            DirectoryInfo dstDi = new DirectoryInfo(dstDirPath);
            dstDi.Create();
            foreach(FileInfo srcFi in srcDi.GetFiles())
            {
                string dstFilePath = $@"{dstDirPath}\{srcFi.Name}";
                srcFi.CopyTo(dstFilePath, true);
            }
            foreach(DirectoryInfo srcSubDi in srcDi.GetDirectories())
            {
                DirectoryInfo dstSubDi = dstDi.CreateSubdirectory(srcSubDi.Name);
                CopyTo(srcSubDi.FullName, dstSubDi.FullName);
            }
        }
    }
}