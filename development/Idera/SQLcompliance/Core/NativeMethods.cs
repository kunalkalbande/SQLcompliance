using System;
using System.Runtime.InteropServices;

namespace Idera.SQLcompliance.Core
{
	/// <summary>
	/// Native methods container class for SQLsecureCore.
	/// </summary>	
	internal class NativeMethods
	{

		#region Constructor

		/// <summary>
		/// Private constructor; this class cannot be instantiated.
		/// </summary>
		private NativeMethods() {}

		#endregion

#if COMPRESSION

		#region Compression

		#region NRV API

		/// <summary>
		/// The NRV compression API
		/// </summary>
		/// <param name="inBuffer"></param>
		/// <param name="inLength"></param>
		/// <param name="outBuffer"></param>
		/// <param name="outLength"></param>
		/// <param name="workBuffer"></param>
		/// <returns></returns>	
		
		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#14", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_a1_13_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#15", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_a1_14_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#16", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_a1_15_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#17", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_a1_16_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#18", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_a2_13_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#19", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_a2_14_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#20", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_a2_15_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#21", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_a2_16_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#22", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_b1_13_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);
			

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#23", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_b1_14_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);


		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#24", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_b1_15_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#25", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_b1_16_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#26", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_b2_13_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#27", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_b2_14_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#28", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_b2_15_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#29", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_b2_16_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#30", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_c1_13_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#31", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_c1_14_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#32", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_c1_15_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#33", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_1_c1_16_compress(
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);		
			

		/// <summary>
		/// The NRV decompression API
		/// </summary>
		/// <param name="inBuffer"></param>
		/// <param name="inLength"></param>
		/// <param name="outBuffer"></param>
		/// <param name="outLength"></param>
		/// <param name="workBuffer"></param>
		/// <returns></returns>
		
		/*
		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#12", CallingConvention=CallingConvention.Cdecl), 
			System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_decompress (
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);		
			
		*/		

		/// <summary>
		/// The NRV "safe" decompression API
		/// </summary>
		/// <param name="inBuffer"></param>
		/// <param name="inLength"></param>
		/// <param name="outBuffer"></param>
		/// <param name="outLength"></param>
		/// <param name="workBuffer"></param>
		/// <returns></returns>
		[DllImport(CoreConstants.DllName_NRV, EntryPoint="#13", CallingConvention=CallingConvention.Cdecl), 
		System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int nrv_lzo1x_decompress_safe (
			[In] byte* inBuffer, 
			[In] int inLength,
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* workBuffer);

		#endregion

		#region Bzip2 API

		/// <summary>
		/// The Bzip2 compression API
		/// </summary>
		/// <param name="outBuffer"></param>
		/// <param name="outLength"></param>
		/// <param name="inBuffer"></param>
		/// <param name="inLength"></param>
		/// <param name="blockSize100K"></param>
		/// <param name="verbosity"></param>
		/// <param name="workFactor"></param>
		/// <returns></returns>
		[DllImport(CoreConstants.DllName_Bzip2, EntryPoint="#36"), System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int BZ2_bzBuffToBuffCompress(
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* inBuffer, 
			[In] int inLength,
			[In] int blockSize100K,
			[In] int verbosity,
			[In] int workFactor);
		
		/// <summary>
		/// The Bzip2 decompression API
		/// </summary>
		/// <param name="outBuffer"></param>
		/// <param name="outLength"></param>
		/// <param name="inBuffer"></param>
		/// <param name="inLength"></param>
		/// <param name="small"></param>
		/// <param name="verbosity"></param>
		/// <returns></returns>
		[DllImport (CoreConstants.DllName_Bzip2, EntryPoint="#37"), System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int BZ2_bzBuffToBuffDecompress(
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* inBuffer, 
			[In] int inLength,
			[In] int small,
			[In] int verbosity);

		#endregion

		#region Zlib API

		/// <summary>
		/// The Zlib compression API	
		/// </summary>
		/// <param name="inBuffer"></param>
		/// <param name="inLength"></param>
		/// <param name="outBuffer"></param>
		/// <param name="outLength"></param>
		/// <param name="workBuffer"></param>
		/// <returns></returns>
		[DllImport(CoreConstants.DllName_Zlib), System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int compress2(
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* inBuffer,
			[In] int inLength,
			[In] int level);		

		/// <summary>
		/// The Zlib decompression API
		/// </summary>
		/// <param name="inBuffer"></param>
		/// <param name="inLength"></param>
		/// <param name="outBuffer"></param>
		/// <param name="outLength"></param>
		/// <param name="workBuffer"></param>
		/// <returns></returns>
		[DllImport(CoreConstants.DllName_Zlib), System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern unsafe int uncompress (
			[In, Out] byte* outBuffer, 
			[In, Out] int* outLength,
			[In] byte* inBuffer,
			[In] int inLength);			

		#endregion

		#endregion

#endif

		#region Win32 authentication methods

		[DllImport("advapi32.dll", SetLastError=true), System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, 
			int dwLogonType, int dwLogonProvider, ref IntPtr phToken);	

		[DllImport("kernel32.dll", CharSet=CharSet.Auto), System.Security.SuppressUnmanagedCodeSecurity]
		internal extern static bool CloseHandle(IntPtr handle);

		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true), System.Security.SuppressUnmanagedCodeSecurity]
		internal extern static bool DuplicateToken(IntPtr ExistingTokenHandle, 
			int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

		[DllImport("kernel32.dll", CharSet=System.Runtime.InteropServices.CharSet.Auto), System.Security.SuppressUnmanagedCodeSecurity]
		internal unsafe static extern int FormatMessage(int dwFlags, ref IntPtr lpSource, 
			int dwMessageId, int dwLanguageId, ref String lpBuffer, int nSize, IntPtr *Arguments);

		internal const int LOGON32_PROVIDER_DEFAULT = 0;
		//This parameter causes LogonUser to create a primary token.
		internal const int LOGON32_LOGON_INTERACTIVE = 2;
		internal const int LOGON32_LOGON_NETWORK = 3;
		internal const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
		internal const int SECURITY_IMPERSONATION = 2;

		/// <summary>
		/// Formats and returns an error message corresponding to the input errorCode.
		/// </summary>
		/// <param name="errorCode"></param>
		/// <returns></returns>
		internal unsafe static string GetErrorMessage(int errorCode) {
			int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
			int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
			int FORMAT_MESSAGE_FROM_SYSTEM  = 0x00001000;

			int messageSize = 255;
			String lpMsgBuf = "";
			int dwFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS;

			IntPtr ptrlpSource = IntPtr.Zero;
			IntPtr prtArguments = IntPtr.Zero;
        
			NativeMethods.FormatMessage(dwFlags, ref ptrlpSource, errorCode, 0, ref lpMsgBuf, messageSize, &prtArguments);			

			return lpMsgBuf;
		}
		
		#endregion

      #region System Error
		[DllImport("kernel32.dll")]
		internal static extern uint GetLastError();
      #endregion

		#region File I/O

		/// <summary>
		/// CreateFile constants
		/// </summary>
		internal const uint FILE_SHARE_READ = 0x00000001;
		internal const uint FILE_SHARE_WRITE = 0x00000002;
		internal const uint FILE_SHARE_DELETE = 0x00000004;

		internal const int FILE_ATTRIBUTE_NORMAL = 0x00000080;

		internal const uint CREATE_ALWAYS = 2;
		internal const uint OPEN_EXISTING = 3;
		internal const uint OPEN_ALWAYS = 4;

		internal const uint GENERIC_READ = 0x80000000;
		internal const uint GENERIC_WRITE = 0x40000000;

		internal const uint FILE_FLAG_OVERLAPPED = 0x40000000;
		internal const uint FILE_FLAG_NO_BUFFERING = 0x20000000;	
		internal const uint FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000;
		internal const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;
		internal const uint FILE_ATTRIBUTE_TEMPORARY = 0x00000100;

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr CreateFile(
			string lpFileName,
			uint dwDesiredAccess,
			uint dwShareMode,
			IntPtr lpSecurityAttributes,
			uint dwCreationDisposition,
			uint dwFlagsAndAttributes,
			IntPtr hTemplateFile);

		[DllImport("kernel32.dll")]
		internal static extern bool GetDiskFreeSpace(
			[In]string rootPathName,
			[Out]out int sectorsPerCluster,
			[Out]out int bytesPerSector,
			[Out]out int numberOfFreeClusters,
			[Out]out int totalNumberOfClusters);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern void FlushFileBuffers(IntPtr handle);

		#endregion

		#region Performance counters

		[DllImport("KERNEL32"), System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		[DllImport("kernel32.dll"), System.Security.SuppressUnmanagedCodeSecurity]
		internal static extern bool QueryPerformanceFrequency(out long lpFrequency);

		#endregion

		#region COM

		[DllImport("ole32.dll", PreserveSig=false), System.Security.SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		internal static extern object CoCreateInstance(
			[MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
			[MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
			uint dwClsContext,
			[MarshalAs(UnmanagedType.LPStruct)] Guid riid); 

		#endregion		
		
		#region Unsafe functions
		
      public static unsafe int GetHashCode(string str)
      {
         int length = str.Length;
         fixed (char* c = str)
         {
            char* cc = c;
            char* end = cc + str.Length - 1;
            int h = 5381;
            for (; cc <= end; cc++)
            {
               h = ((h << 5) + h) ^ *cc;
            }
            return h;
         }
      }
      #endregion


		
	}
}
