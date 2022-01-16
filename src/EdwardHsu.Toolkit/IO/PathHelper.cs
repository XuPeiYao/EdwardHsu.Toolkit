using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdwardHsu.Toolkit.IO
{
    public static class PathHelper
    {
        /// <summary>
        /// Get Path.GetTempFileName without create
        /// </summary>
        /// <returns></returns>
        public static string GetTempFileName()
        {
            return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }
    }
}
