using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLcompliance.Core.Settings
{
    /// <summary>
    /// Denotes the Repository Information so that any upgrade or certain operations based on the repository version could be carried out
    /// </summary>
    public class RepositoryInfo
    {
        /// <summary>
        /// Name should be unique
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Could be 0, 1, -1, etc
        /// </summary>
        public int InternalValue { get; set; }

        /// <summary>
        /// Define values in type string
        /// </summary>
        public string CharacterValue { get; set; }
    }
}
