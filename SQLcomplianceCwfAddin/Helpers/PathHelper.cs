using System;
using System.IO;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class PathHelper
    {
        public static string Error_InvalidTraceDirectory = "The trace directory must be a valid local directory path on the SQLcompliance Agent Computer, may not include relative pathing, and must be 180 characters or less.";

        //--------------------------------------------------------------------
        // Validate Path
        //--------------------------------------------------------------------
        public static bool ValidateTraceDirectoryPath(string filepath)
        {
            Exception internalException = null;
            string errorMessage = string.Empty;

            // make sure defined and a local path
            if (filepath.Length < 3 || 
                filepath.Length > 180 || 
                filepath[1] != ':' || 
                filepath[2] != '\\' || 
                filepath.IndexOf("..") != -1)
            {
                errorMessage = Error_InvalidTraceDirectory;
            }
            else
            {
                try
                {
                    if (!Path.IsPathRooted(filepath))
                    {
                        errorMessage = Error_InvalidTraceDirectory;
                    }
                        
                }
                catch (Exception ex)
                {
                    internalException = ex;
                    errorMessage = Error_InvalidTraceDirectory;
                }                
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new Exception(errorMessage, internalException);
            }

            return true;
        }
    }
}
