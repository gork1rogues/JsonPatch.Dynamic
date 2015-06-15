﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marvin.JsonPatch.Dynamic.Helpers
{
    internal static class PathHelpers
    {
        internal static CheckPathResult CheckPath(string pathToCheck)
        {
           string adjustedPath = pathToCheck;

            // check for most common path errors on create.  This is not
            // absolutely necessary, but it allows us to already catch mistakes
            // on creation of the patch document rather than on execute.

           if (pathToCheck.Contains(".") || pathToCheck.Contains("//")
               || pathToCheck.Contains(" ") || pathToCheck.Contains("\\")
             )
           {
               return new CheckPathResult(false, adjustedPath);
           }

           if (!(pathToCheck.StartsWith("/")))
           {
               adjustedPath = "/" + adjustedPath;
           }

           return new CheckPathResult(true, adjustedPath);
        }
    }
}