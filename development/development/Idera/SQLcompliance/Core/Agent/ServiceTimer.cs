using System ;
using System.Threading ;

namespace Idera.SQLcompliance.Core.Agent
{
   /// <summary>
   /// Timer class to be used in services.  Uses System.Threading.Timer
   /// </summary>
   public class ServiceTimer : IDisposable
   {
      #region Protected Fields

      // Track whether Dispose has been called.
      private bool _isDisposed = false ;

      private Timer _timer ;
      private TimerCallback _handler ;
      private int _interval ;  // Interval in milliseconds
      private int _state ;
      private object _stateLock ;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the interval at which to raise the Elapsed event. 
      /// </summary>
      public int Interval
      {
         get { return _interval ; }
         set
         {
            if(_interval <= 0)
               _interval = Timeout.Infinite ;
            else
            _interval = value ;

            lock(_stateLock)
            {
               // Only apply the change if the timer is currently running
               if(_state == 1 && _timer != null)
                  _timer.Change(0, _interval) ;
            }
         }
      }

      public TimerCallback TimerCallbackHandler
      {
         get { return _handler ; }
         set { _handler = value ; }
      }

      #endregion

      #region Constructors

      /// <summary>
      /// Constructor for the ServiceTime class.  We do not start the timer
      /// at construction - this must be done via the Start method.
      /// </summary>
      /// <param name="callback"></param>
      /// <param name="interval"></param>
      public ServiceTimer(TimerCallback callback, int interval)
      {
         _stateLock = new object() ;
         _interval = interval ;
         _handler = callback ;
         _timer = new Timer(new TimerCallback(TimerCallbackMethod), null, Timeout.Infinite, interval) ;
         _state = 0 ;
      }

      public ServiceTimer(TimerCallback callback) : this(callback, Timeout.Infinite) { }

      public ServiceTimer(int interval) : this(null, interval) { }

      #endregion

      #region Public Methods

      public virtual void Start() 
      {
         Start(0) ;
      }

      public virtual void Start(int msDelay)
      {
         lock(_stateLock)
         {
            if(_state > 0)
               return ;
            _state = 1; 
         }

         if(_timer == null)
         {
            _timer = new Timer(new TimerCallback(TimerCallbackMethod), null, msDelay, _interval) ;
         }
         else
         {
            _timer.Change(msDelay, _interval) ;
         }
      }

      public virtual void Stop()
      {
         lock(_stateLock)
         {
            _state = 0 ;
            if(_timer != null)
            {
               _timer.Change(Timeout.Infinite, Timeout.Infinite) ;
            }
         }
      }

      private void TimerCallbackMethod(object state)
      {
         if(_handler != null)
         {
            _handler(state) ;
         }
      }

      #endregion

      #region IDisposable Members

      // Implement IDisposable.
      // Do not make this method virtual.
      // A derived class should not be able to override this method.
      public void Dispose()
      {
         Dispose(true) ;
         // This object will be cleaned up by the Dispose method.
         // Therefore, you should call GC.SupressFinalize to
         // take this object off the finalization queue 
         // and prevent finalization code for this object
         // from executing a second time.
         GC.SuppressFinalize(this) ;
      }

      // Dispose(bool disposing) executes in two distinct scenarios.
      // If disposing equals true, the method has been called directly
      // or indirectly by a user's code. Managed and unmanaged resources
      // can be disposed.
      // If disposing equals false, the method has been called by the 
      // runtime from inside the finalizer and you should not reference 
      // other objects. Only unmanaged resources can be disposed.
      private void Dispose(bool disposing)
      {
         // Check to see if Dispose has already been called.
         if(!_isDisposed)
         {
            // If disposing equals true, dispose all managed 
            // and unmanaged resources.
            if(disposing)
            {
               // Dispose managed resources.
               if(_timer != null)
                  _timer.Dispose() ;
            }

            // Call the appropriate methods to clean up 
            // unmanaged resources here.
            // If disposing is false, 
            // only the following code is executed.
         }
         _isDisposed = true ;
      }

      // Use C# destructor syntax for finalization code.
      // This destructor will run only if the Dispose method 
      // does not get called.
      // It gives your base class the opportunity to finalize.
      // Do not provide destructors in types derived from this class.
      ~ServiceTimer()
      {
         // Do not re-create Dispose clean-up code here.
         // Calling Dispose(false) is optimal in terms of
         // readability and maintainability.
         Dispose(false) ;
      }

      #endregion
   }
}