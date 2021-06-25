using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Idera.SQLcompliance.Core.Data
{
    public static class CachedObjectRepositoryConnectionFactory
    {
        public delegate SqlConnection ConnectionFactoryMethod();

        private static ConnectionFactoryMethod connectionDelegate;

        /// <summary>
        /// Sets the method that will be called to get a connection to the 
        /// SQLdm database.
        /// </summary>
        public static ConnectionFactoryMethod ConnectionFactory
        {
            get { return connectionDelegate; }
            set { connectionDelegate = value; }
        }

        /// <summary>
        /// Gets a connection to the SQLdm repository.
        /// </summary>
        /// <returns></returns>
        public static SqlConnection GetRepositoryConnection()
        {
            if (connectionDelegate == null)
                return null;
            return connectionDelegate.Invoke();
        }
    }
}