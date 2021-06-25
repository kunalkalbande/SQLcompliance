using System.Data.SqlClient;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Web;
using PluginAddInView;
using PluginCommon;
using SQLcomplianceCwfAddin;
using SQLcomplianceCwfAddin.Helpers;
using TracerX;
using System;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Idera.SQLcompliance.Core.Security;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Idera.SQLcompliance.Core.Configuration;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, MaxItemsInObjectGraph = int.MaxValue)]
    public partial class RestService: IRestService
    {
        #region members

        private readonly HostObject _dashboardHost;
        private readonly Logger _logger;

        #endregion

        #region constructor \ destructor

        public RestService(HostObject host)
        {
            _logger = Logger.GetLogger("RestService");
            _logger.BinaryFileTraceLevel = TracerX.TraceLevel.Info;

            _logger.Info("Setting host object for dashboard.");
            _dashboardHost = host;
        }

        #endregion

        #region properties

        public SqlConnection GetConnection()
        {
            var prinicpal = GetPrincipalFromRequest();
            var connectionCredentials = GetConnectionCredentials(prinicpal);
            var connection = QueryRouter.Instance.GetSqlCmRepositoryConnection(connectionCredentials);
            return connection;
        }

        #endregion

        #region public methods

        public ConnectionCredentials GetConnectionCredentials(IPrincipal principal)
        {
            if (WebOperationContext.Current == null)
                return null;

            var absolutePath = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.AbsolutePath;
            var parts = absolutePath.Split('/');
            var productId = parts[3];
            var credentials = _dashboardHost.GetConnectionCredentialsOfProductInstance(productId, principal);
            credentials.ConnectionPassword = SQLcomplianceCwfAddin.Helpers.EncryptionHelper.QuickDecrypt(credentials.ConnectionPassword);
            return credentials;
        }

        public string GetProductIdFromrequest(IPrincipal principal)
        {
            if (WebOperationContext.Current == null)
                return null;

            var absolutePath = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.AbsolutePath;
            var parts = absolutePath.Split('/');
            var productId = parts[3];

            return productId;
        }

        public IPrincipal GetPrincipalFromRequest()
        {
            if (WebOperationContext.Current == null)
                return null;

            var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");

            //Added by Abhay(AJ) :: BASIC Authentication for LM Utility
            string userName, password;
            _logger.Info("Authentication and Repo Credentials");
            if (header != null && header.StartsWith("BASIC ", StringComparison.InvariantCultureIgnoreCase))
            {
                string authEncodedToken = System.Text.RegularExpressions.Regex.Replace(header, "Basic", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                byte[] arrEncoded = Convert.FromBase64String(authEncodedToken);
                string authDecodedToken = Encoding.UTF8.GetString(arrEncoded);
                if (authDecodedToken != null && !String.IsNullOrWhiteSpace(authDecodedToken))
                {
                    string[] splittedAuthToken = authDecodedToken.Split(':');
                    if (splittedAuthToken.Length >= 0)
                    {
                        userName = splittedAuthToken[0];
                        password = splittedAuthToken[1];
                        return Validate(userName, password);
                    }
                }
                return null;
            }//END Added by Abhay(AJ) :: BASIC Authentication for LM Utility

            else
            {
                var principal = _dashboardHost.GetPrincipal(header);
                if (principal == null)
                    throw new WebFaultException(HttpStatusCode.Forbidden);

                return principal;
            }
        }

        #endregion

        HostObject _myHost = null;

        class UserCredentialsForRESTAPI
        {
            public string UserName { get; set; }

            public string Password { get; set; }
        }

        private UserCredentialsForRESTAPI GetUserCredentialsFromHeader()
        {
            var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
            UserCredentialsForRESTAPI creds = new UserCredentialsForRESTAPI();

            if (header != null && header.StartsWith("BASIC ", StringComparison.InvariantCultureIgnoreCase))
            {
                string authEncodedToken = System.Text.RegularExpressions.Regex.Replace(header, "BASIC", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                byte[] arrEncoded = Convert.FromBase64String(authEncodedToken);
                string authDecodedToken = Encoding.UTF8.GetString(arrEncoded);
                if (authDecodedToken != null && !String.IsNullOrWhiteSpace(authDecodedToken))
                {
                    string[] splittedAuthToken = authDecodedToken.Split(':');
                    if (splittedAuthToken.Length >= 0)
                    {
                        creds.UserName = null != splittedAuthToken[0] ? splittedAuthToken[0] : string.Empty;
                        creds.Password = null != splittedAuthToken[1] ? splittedAuthToken[1] : string.Empty;

                    }
                }
            }
            return creds;
        }

        /// <summary>
        /// SQLcm : setting the repo connection credentials object from CWF host
        /// </summary>
        private void SetConnectionCredentiaslFromCWFHost()
        {
            IPrincipal userInfo = GetPrincipalFromRequest(); //getting the principal from cwf

            if (userInfo == null)
                throw new WebFaultException(HttpStatusCode.Forbidden);


            string productId = GetProductIdFromrequest(userInfo);
            string[] repositoryInformation = null;
            string repositoryHost = string.Empty;
            string repositoryDatabase = string.Empty;
            UserCredentialsForRESTAPI repoCreds = GetUserCredentialsFromHeader();

            ConnectionCredentials credentials = _dashboardHost.GetConnectionCredentialsOfProductInstance(productId, userInfo);
            ConnectionCredentials deepCopyCredentials = new ConnectionCredentials();

            deepCopyCredentials.ConnectionPassword = repoCreds.Password; //unencrypted password
            deepCopyCredentials.ConnectionUser = repoCreds.UserName;
            deepCopyCredentials.Location = credentials.Location;
            
            repositoryInformation = deepCopyCredentials.Location.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            repositoryHost = repositoryInformation[0];
            repositoryDatabase = repositoryInformation[1];
            SetInstanceName(repositoryHost);

            SqlConnectionInfo repoConnInfo = new SqlConnectionInfo(repositoryHost, repositoryDatabase, deepCopyCredentials.ConnectionUser, deepCopyCredentials.ConnectionPassword);
            repoConnInfo.UseIntegratedSecurity = true;//since CWF supports only windows credentials for now

            RestServiceConfiguration.SetRepositoryConnectInfo(repoConnInfo); //resetting the connection credentials

            RefreshUserToken(repoConnInfo);
        }

        //private static ReaderWriterLock syncRoot;
        private static string instanceName;
        public static void SetInstanceName(string value)
        {
            Debug.Assert(value != null);
            //syncRoot.AcquireWriterLock(-1);
            instanceName = value;
            //syncRoot.ReleaseWriterLock();
        }

        public static void SetRepositoryConnectInfo(SqlConnectionInfo sqlConnectInfo)
        {
        }

        /// <summary>
       /// </summary>
        public void RefreshUserToken(SqlConnectionInfo repoConnInfo)
        {
            using (LOG.DebugCall("RefreshUserToken"))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                try
                {
                    string domain;
                    string userName;
                    string password = repoConnInfo.Password;
                    var splittedUserName = repoConnInfo.UserName.Split('\\');
                    if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password is not specified.");
                    if (splittedUserName.Length != 2) throw new ArgumentException("Incorrect user name format");
                    userName = splittedUserName[1];
                    domain = splittedUserName[0];

                    if (repoConnInfo.UseIntegratedSecurity)
                    {

                        using (var ctx = new Idera.SQLcompliance.Core.Security.ImpersonationContext(domain, userName, password))
                        {
                            // Validate password.
                            ctx.LogonUser();

                            // Get native windows groups by doing query through the repository SQL Server.
                            // It is quite likely that the user will not have permission to access the repository,
                            // in which case we need to get the native groups some other way.    For now we are
                            // saying there are no windows groups retrieved so ignoring them.
                            try
                            {
                                ctx.RunAs(() =>
                                {
                                    userToken.Refresh(repoConnInfo.ConnectionString);
                                });
                            }
                            catch
                            {
                                // Ignore this exception, we don't have windows groups at this point.
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error updating user token from the repository. ", e);
                }
                finally
                {
                    stopWatch.Stop();
                    LOG.InfoFormat("RefreshPermissionToken took {0} milliseconds.",
                                   stopWatch.ElapsedMilliseconds);
                }
            }
        }
        //Added by Abhay(AJ) :: BASIC Authentication for LM Utility
        public IPrincipal Validate(string userName, string password)
        {
            string[] nativeGroups = null;
            IPrincipal principal = null;
            string domain = null;
            const char DOMAIN_SPLIT_CHARACTER = '\\';
            var parts = userName.Split(DOMAIN_SPLIT_CHARACTER);
            if (parts.Length != 1)
            {
                if (parts.Length != 2) throw new ArgumentException("Incorrect user name format");
                if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password is not specified.");

                userName = parts[1];
                domain = parts[0];
                _logger.ErrorCall("UserName: " + userName);
                _logger.ErrorCall("Domain" + domain);
                using (var ctx = new  Idera.SQLcompliance.Core.Security.ImpersonationContext(domain, userName, password))
                {

                    // Validate password.
                    ctx.LogonUser();
                    // Get native windows groups by doing query through the repository SQL Server.
                    // It is quite likely that the user will not have permission to access the repository,
                    // in which case we need to get the native groups some other way.    For now we are
                    // saying there are no windows groups retrieved so ignoring them.
                    try
                    {

                        ctx.RunAs(() =>
                        {
                            nativeGroups = GetGroups(WindowsIdentity.GetCurrent().Token).ToArray();
                        });
                    }
                    catch (Exception ex)
                    {
                        // Ignore this exception, we don't have windows groups at this point.
                        _logger.ErrorCall("Something went wrong getting the groups for this user: " + ex.Message);
                    }

                    // Build the return prinicipal.
                    var wi = new GenericIdentity(userName, "Windows");

                    _logger.InfoCall("\n\nGroups for this user: " + string.Join(",", nativeGroups));
                    ReadOnlyCollection<User> groupUsers = getGroupUsers(nativeGroups, domain + DOMAIN_SPLIT_CHARACTER + userName);
                    principal = new PluginCommon.Authentication.SimplePrincipal(wi, nativeGroups, groupUsers);
                    return principal;
                }

            }
            else
            {
                // We assume domain name will always be present, if the same is absent, we are considering this
                // to be invalid user.
                _logger.InfoCall("\n\n Attempt to login without passing the domain name.");
                return null;
            }
        }

        // Get all groups.
        private List<string> GetGroups(IntPtr userToken)
        {
            List<string> result = new List<string>();
            try
            {
                WindowsIdentity wi = new WindowsIdentity(userToken);
                foreach (IdentityReference group in wi.Groups)
                {
                    result.Add(group.Translate(typeof(NTAccount)).ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorCall("Unable to get the groups for this user." + ex.Message);
            }

            result.Sort();
            return result;
        }

        private ReadOnlyCollection<User> getGroupUsers(string[] nativeGroups, string userName)
        {

            List<User> groupUsers = new List<User>();
            User _user = new User(userName, "Windows", true);
            groupUsers.Add(_user);
            return new ReadOnlyCollection<User>(groupUsers);
        }
        //End Added by Abhay(AJ) :: BASIC Authentication for LM Utility
    }
}