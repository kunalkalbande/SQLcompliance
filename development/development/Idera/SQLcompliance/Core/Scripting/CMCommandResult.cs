using System;
using System.Collections ;
using System.Runtime.Serialization ;

namespace Idera.SQLcompliance.Core.Scripting
{
   public enum ResultCode : int
   {
      Error = -1,
      Success = 0,
      InvalidArguments = 1,
      ExceptionThrown = 2,
      ConnectionFailed = 3,
      InvalidArgumentFormat = 4,
   }
	/// <summary>
	/// Summary description for CMCommandResult.
	/// </summary>
	[Serializable()]
	public class CMCommandResult : ISerializable
	{
      private ResultCode _resultCode ;
      private ArrayList _resultStrings ;
      private Exception _exception ;
      private int _classVersion = CoreConstants.SerializationVersion;

      public static readonly string GeneralError = "General Error" ;
      public static readonly string Success = "Success" ;
      public static readonly string InvalidArguments = "Invalid Arguments" ;
      public static readonly string InvalidArgumentFormat = "Invalid Argument Format: {0}" ;
      public static readonly string ExceptionThrown = "Error: {0}" ;
      public static readonly string ConnectionFailed = "Unable to connect to the specified host: {0}:{1}" ; 

      public CMCommandResult(Exception e) : this(ResultCode.ExceptionThrown)
      {
         _exception = e ;
      }

      public CMCommandResult(ResultCode code)
      {
         _resultCode = code ;
         _resultStrings = new ArrayList() ;
         _exception = null ;
      }

      public CMCommandResult() : this(ResultCode.Success)
      {
      }

	   public ResultCode ResultCode
	   {
	      get { return _resultCode ; }
	      set { _resultCode = value ; }
	   }

	   public string ResultString
	   {
	      get 
         {
            if(_resultStrings == null || _resultStrings.Count == 0)
            {
               switch(_resultCode)
               {
                  case Scripting.ResultCode.Error:
                     return GeneralError ;
                  case Scripting.ResultCode.Success:
                     return Success ;
                  case Scripting.ResultCode.InvalidArguments:
                     return InvalidArguments ;
                  case Scripting.ResultCode.ExceptionThrown:
                     if(_exception == null)
                        return String.Format(ExceptionThrown, "null") ;
                     else
                        return String.Format(ExceptionThrown, _exception.Message) ;
                  default:
                     return "" ;
               }
            }
            string retVal = "" ;
            foreach(string s in _resultStrings)
               retVal += String.Format("{0}\n", s) ;

            return retVal ; 
         }
	   }

	   public Exception Exception
	   {
	      get { return _exception ; }
	      set { _exception = value ; }
	   }

      public void AddResultString(string s)
      {
         _resultStrings.Add(s) ;
      }
      #region ISerializable Members

      public CMCommandResult(SerializationInfo info, StreamingContext context)
      {
         try
         {
            _resultStrings = new ArrayList() ;
            _resultCode =(ResultCode)info.GetInt32("resultCode") ;
            _exception = (Exception)info.GetValue("exception", typeof(Exception)) ;
            _classVersion = info.GetInt32("classVersion") ;
            int i = info.GetInt32("resultStringCount") ;
            for(int j = 0 ; j < i ; j++)
            {
               string s = info.GetString(String.Format("resultString{0}", j)) ;
               _resultStrings.Add(s) ;
            }
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowDeserializationException( e, this.GetType());
         }
      }

      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         try
         {
            info.AddValue("resultCode", (int)_resultCode) ;
            info.AddValue("exception", _exception) ;
            info.AddValue("classVersion", _classVersion) ;
            if(_resultStrings == null)
               info.AddValue("resultStringCount", 0) ;
            else
            {
               info.AddValue("resultStringCount", _resultStrings.Count) ;
               for(int i = 0 ; i < _resultStrings.Count ; i++)
                  info.AddValue(String.Format("resultString{0}", i), _resultStrings[i]) ;
            }
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowSerializationException( e, this.GetType());
         }

      }

      #endregion
   }
}
