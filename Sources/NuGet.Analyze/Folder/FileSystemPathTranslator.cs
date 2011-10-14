using System.IO;

namespace NuGet.Analyze.Folder
{
    public class FileSystemPathTranslator : IFileSystemPathTranslator
    {
        public FileInfo GetAbsolutePath(FileInfo sourcePath, string relativeTargetPath)
        {
            string targetPath = sourcePath.FullName;
            // trim all leading occurrences of "..\" and navigate a folder up in the source path
            while (relativeTargetPath.StartsWith(@"..\"))
            {
                // make sure the trailing "\" occurrences are removed before navigating up
                targetPath = targetPath.EndsWith(Path.DirectorySeparatorChar.ToString()) 
                    ? targetPath.TrimEnd(new[] { Path.DirectorySeparatorChar }) 
                    : NavigateToParentFolder(targetPath);

                targetPath = NavigateToParentFolder(targetPath);

                // trim off the leading "..\"
                relativeTargetPath =
                    relativeTargetPath.Substring(3, relativeTargetPath.Length - 3);
            }

            // ensure we have a trailing "\" for the folder root
            if (!targetPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                targetPath = targetPath + Path.DirectorySeparatorChar;

            // by now, we should have the root folder path for the packages config path
            // append the relative path to the folder root
            targetPath = targetPath + relativeTargetPath;

            return new FileInfo(targetPath);
        }

        private static string NavigateToParentFolder(string targetPath)
        {
            int lastIndexOfSlash = targetPath.LastIndexOf(Path.DirectorySeparatorChar);
            targetPath = targetPath.Remove(lastIndexOfSlash);
            return targetPath;
        }
    }
}