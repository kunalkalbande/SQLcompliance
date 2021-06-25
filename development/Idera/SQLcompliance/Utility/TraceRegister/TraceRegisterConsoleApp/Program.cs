namespace Idera.SQLcompliance.Utility.TraceRegisterConsoleApp
{
   class Program
   {
      static void Main(string[] args)
      {         
         TraceRegister traceRegister = new TraceRegister(args);
         traceRegister.Register();
         System.Threading.Thread.Sleep(3000);
      }
   }
}
