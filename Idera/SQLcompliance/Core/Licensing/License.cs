using System;
using System.Text;
using System.Reflection;

namespace Idera.SQLcompliance.Core.Licensing
{
   #region IderaLicense Class
   
	/// <summary>
	/// An object encapsulation of Idera license parts
	/// </summary>
	public sealed class IderaLicense
	{
		#region Properties
		
		private bool m_loaded = false;

		private string server;
		/// <summary>
		/// Server license applies to
		/// </summary>
		public string Server {
			get {
			   if (!m_loaded) return "";
				return server;
			}
		}

		private Product product;
		private string productString;
		/// <summary>
		/// Product license applies to
		/// </summary>
		public Product Product {
			get {
			   if (!m_loaded) return null;
				return product;
			}
		}

		private string key;
		/// <summary>
		/// License key (generated hash)
		/// </summary>
		public string Key {
			get {
			   if (!m_loaded) return "";
				return key;
			}
		}

		private bool isEnterprise;
		/// <summary>
		/// Flag indicating whether this license is an enterprise license
		/// </summary>
		public bool IsEnterprise {
			get {
				return isEnterprise;
			}
		}

		#endregion

		#region Constructor

      //-------------------------------------------------------------------
      // Constructor
      //-------------------------------------------------------------------
		public
		   IderaLicense(
		      string            licenseString
         )
		{
		   if ( licenseString != "" )
		   {
		      try
		      {
			      // Break the license string into its parts
			      ParseLicense(licenseString);

			      // Set the enterprise flag
			      if (product.Name != null 
				      && String.Compare(product.Name, CoreConstants.ProductName_Enterprise, true) == 0)
			      {
				      isEnterprise = true;
			      }
   			   
			      m_loaded = true;
			   }
			   catch (Exception )
			   {
			      // we dont care about exceptions - they just mean we have an invalid license
			   }
			}
		}

		#endregion

		#region Public methods

      //-------------------------------------------------------------------
      // ToString
      //-------------------------------------------------------------------
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(server);
			builder.Append(CoreConstants.LicenseSeparator);
			builder.Append(product.ToString());
			builder.Append(CoreConstants.LicenseSeparator);
			builder.Append(key);
			return builder.ToString();
		}

      //-------------------------------------------------------------------
      // IsValid
      //-------------------------------------------------------------------
		public bool
		   IsValid()
		{
		   if ( !m_loaded ) return false;
		   
			// Check the product name
			if (!isEnterprise 
				&& (product.Name == null || String.Compare(product.Name, CoreConstants.ProductName, true) != 0))
   		{
				return false;
			}		

			// Check the key to ensure that it is valid for this server and product
			string hashedKey = HashKey(server, productString);
			if ( hashedKey.Equals(key))
			{
				return true;
			}
			else
			{
				return false;
			}

		}
		
      //-------------------------------------------------------------------
      // IsExpired
      //-------------------------------------------------------------------
		public bool
		   IsExpired()
		{
		   if ( !m_loaded ) return true;
			return product.HasExpired();
		}

      //-------------------------------------------------------------------
      // GenerateTrialLicenseString
      //-------------------------------------------------------------------
		public static string
		   GenerateTrialLicenseString(
		      string                serverInstanceName
	      )
		{
			// Build the product
			StringBuilder productString = new StringBuilder();
			productString.Append(CoreConstants.ProductName);
			productString.Append(CoreConstants.ProductSeparator);
			productString.Append(Product.GetProductVersion());
			productString.Append(CoreConstants.ProductSeparator);
			productString.Append(Product.GetProductDate(DateTime.Now.AddDays(CoreConstants.ProductExpirationDaysLimit)));
			productString.Append(CoreConstants.ProductSeparator);
			productString.Append(CoreConstants.ProductTrialNumberSqlServerInstances.ToString());

			StringBuilder licenseString = new StringBuilder();		
			
			if ( serverInstanceName.IndexOf(CoreConstants.LicenseInstancePrefix) == -1 )
			{ 	
			   licenseString.Append(CoreConstants.LicenseInstancePrefix);
		   }
			licenseString.Append(serverInstanceName);
			licenseString.Append(CoreConstants.LicenseSeparator);
			licenseString.Append(productString.ToString());
			licenseString.Append(CoreConstants.LicenseSeparator);
			licenseString.Append(HashKey(serverInstanceName, productString.ToString()));
			
			return licenseString.ToString();
		}

      //-------------------------------------------------------------------
      // IsTrialLicense
      //-------------------------------------------------------------------
		public bool
		   IsTrialLicense()
	   {
	      if ( ! m_loaded ) return false;
	      
			TimeSpan diff = product.ExpirationDate - DateTime.Now;
			if (diff.Days <= CoreConstants.ProductExpirationDaysLimit)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

      //-------------------------------------------------------------------
      // TrialLicenseExpirationText
      //-------------------------------------------------------------------
		public string
		   TrialLicenseExpirationText()
		{
	      if ( ! m_loaded ) return "";

		   if ( IsTrialLicense() )
		   {
			   TimeSpan diff = product.ExpirationDate - DateTime.Now;
			   
			   if ((diff.Days + 1) > 7)
			   {
			      return "";
			   }
			   else if ((diff.Days + 1) > 0)
			   {
				   // License has not expired yet - warn if in the last week
				   return String.Format( CoreConstants.Info_TrialLicenseWillExpire,
				                         diff.Days + 1,
				                         product.ExpirationDate.ToShortDateString());
			   }
			   else
			   {
				   // License has expired
				   return CoreConstants.Info_TrialLicenseHasExpired;
			   }
			}
			else
			{
			   return "";
			}
		}
		
      //-------------------------------------------------------------------
      // DaysUntilExpiration
      //-------------------------------------------------------------------
		public int
		   DaysUntilExpiration()
		{
	      if ( ! m_loaded ) return 999;

	      TimeSpan diff = product.ExpirationDate - DateTime.Now;
			return (diff.Days + 1);
		}

		#endregion	

		#region Private methods

      //-------------------------------------------------------------------
      // ParseLicense
      //-------------------------------------------------------------------
		private void ParseLicense(string license)
		{
			// Server
			server = license.Substring(0, license.IndexOf(CoreConstants.LicenseSeparator));
			server = server.Substring(server.IndexOf("=") + 1);

			// Product
			string remaining = license.Substring(license.IndexOf(CoreConstants.LicenseSeparator) + 1);
			productString = remaining.Substring(0, remaining.IndexOf(CoreConstants.LicenseSeparator));
			productString = productString.Substring(productString.IndexOf("=") + 1);
			product = new Product(productString);

			// Key
			key = remaining.Substring(remaining.IndexOf(CoreConstants.LicenseSeparator) + 1);
			key = key.Substring(key.IndexOf("=") + 1);
		}

      #endregion
      
      #region Hash stuff
      
		private static string HashKey(string key, string saltPrecursor) {
													
			int salt = GetSalt(key, saltPrecursor);

			string theBase = GetBase(salt);
			string strOut = "";

			char[] keyChars = key.ToCharArray();
			char[] baseChars = theBase.ToCharArray();
 
			for (int i = 0; i < keyChars.Length; i++) {

				char c = keyChars[i];
				int j = theBase.IndexOf(c);
 
				if (j >= 0) {

					j += 35;
 
					if (j >= 70) {
						j -= 70;
					}
 
					strOut += baseChars[j];

				} else {
					strOut += c;
				}

			}

			return strOut;

		}

		private static int GetSalt(string key, string saltPrecursor) {

			int j = 0;
			int pId = 0;

			char[] dChars = key.ToCharArray();
			char[] pChars = saltPrecursor.ToCharArray();
 
			for (int i = 0; i < dChars.Length; i++) {
				j += dChars[i];
			}
 
			for (int i = 0; i < pChars.Length; i++) {
				pId += pChars[i];
			}
 
			int rtn = j * pId + pId;
			rtn = rtn % 40;
 
			if (rtn < 2) {
				rtn = 2;
			}
 
			return rtn;
		}

		private static string GetBase(int salt) {
 
			if (salt < 2) {
				salt = 2;
			}
 
			if (salt > 40) {
				salt = 40;
			}
 
			string theBase = "";
 
			for (int i = 48; i <= 57; i++) {
				theBase += (char)i;
			}
			for (int i = 64; i <= 90; i++) {
				theBase += (char)i;
			}
			for (int i = 97; i <= 122; i++) {
				theBase += (char)i;
			}
			theBase += (char)35;
			theBase += (char)36;
			theBase += (char)37;
			theBase += (char)45;
			theBase += (char)46;
			theBase += (char)42;
			theBase += '^';
 
			string strMd = "";
 
			for (int i = 0; i < salt; i++) {
				for (int j = 0; j < theBase.Length; j++) {
					if ((j % salt) == i) {
						strMd += theBase.Substring(j, 1);
					}
				}
			}

			return strMd;

		}

		#endregion

	}
	
	#endregion

	#region Product Class

	public class Product {

		#region Properties

		private string name;
		/// <summary>
		/// Product name
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}

		private string version;
		/// <summary>
		/// Product version
		/// </summary>
		public string Version {
			get {
				return version;
			}
		}

		private int licensedInstances;
		/// <summary>
		/// Server license applies to
		/// </summary>
		public int LicensedInstances {
			get {
				return licensedInstances;
			}
		}

		private DateTime expirationDate;
		/// <summary>
		/// Expiration date
		/// </summary>
		public DateTime ExpirationDate {
			get {
				return expirationDate;
			}
		}

		#endregion

		#region Constructor

		public Product(string productString) {

			// Name
			name = productString.Substring(0, productString.IndexOf(CoreConstants.ProductSeparator));
			string remaining = productString.Substring(productString.IndexOf(CoreConstants.ProductSeparator) + 1);

			// Version
			version = remaining.Substring(0, remaining.IndexOf(CoreConstants.ProductSeparator));
			remaining = remaining.Substring(remaining.IndexOf(CoreConstants.ProductSeparator) + 1);

			// Expiration date
			string productDate = remaining.Substring(0, remaining.IndexOf(CoreConstants.ProductSeparator));
			expirationDate = GetDateTimeFromProductDate(productDate);
			remaining = remaining.Substring(remaining.IndexOf(CoreConstants.ProductSeparator) + 1);
			
			// LicenseInstances
			licensedInstances = Int32.Parse(remaining);
		}

		#endregion

		#region public methods

		public static string GetProductVersion() {

			StringBuilder builder = new StringBuilder();

			// Get the assembly version using reflection
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			if (version.Major < 10) {
				builder.Append("0");
			}
			builder.Append(version.Major);
			builder.Append(version.Minor);
			builder.Append(version.Build);
			
			return builder.ToString();

		}

		public static string GetProductDate(DateTime date) {

			StringBuilder productDate = new StringBuilder();
			productDate.Append(date.Year);
			if (date.Month < 10) {
				productDate.Append("0");
			}
			productDate.Append(date.Month);
			if (date.Day < 10) {
				productDate.Append("0");
			}
			productDate.Append(date.Day);
			return productDate.ToString();

		}

		public static DateTime GetDateTimeFromProductDate(string productDate) {

			string year = productDate.Substring(0, 4);
			string month = productDate.Substring(4, 2);
			string day = productDate.Substring(6, 2);
			return new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));

		}

		public bool HasExpired() {

			if (expirationDate.CompareTo(DateTime.Now) < 0) {
				return true;
			} else {
				return false;
			}

		}

		#endregion

		#region Public methods

		public override string ToString() {
			
			StringBuilder builder = new StringBuilder();
			builder.Append(name);
			builder.Append(CoreConstants.ProductSeparator);
			builder.Append(version);
			builder.Append(CoreConstants.ProductSeparator);
			builder.Append(GetProductDate(expirationDate));
			builder.Append(CoreConstants.ProductSeparator);
			builder.Append(licensedInstances.ToString());
			return builder.ToString();
		}	
	
		#endregion

	}

	#endregion
}
