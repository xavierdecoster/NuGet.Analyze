namespace NuGet.Analyze.Tfs
{
    internal class TfsServerPathTranslator : ITfsServerPathTranslator
    {
        public string GetServerPath(string baseServerPath, string relativeLocalPath)
        {
            // trim all leading occurrences of "..\" and navigate a folder up in the server path
            while (relativeLocalPath.StartsWith(@"..\"))
            {
                // make sure the trailing "\" occurrences are removed before navigating up
                if (baseServerPath.EndsWith("/"))
                    baseServerPath = baseServerPath.TrimEnd(new[] { '/' });

                int lastIndexOfSlash = baseServerPath.LastIndexOf('/');
                baseServerPath = baseServerPath.Remove(lastIndexOfSlash);

                // trim off the leading "..\"
                relativeLocalPath =
                    relativeLocalPath.Substring(3, relativeLocalPath.Length - 3);
            }

            // ensure we have a trailing "/" for the folder root
            if (!baseServerPath.EndsWith("/"))
                baseServerPath = baseServerPath + "/";

            // by now, we should have the root folder path for the packages config path
            relativeLocalPath = relativeLocalPath.Replace(@"\", "/");

            // append the relative path to the folder root
            baseServerPath = baseServerPath + relativeLocalPath;

            return baseServerPath;
        }
    }
}
