using System;
using System.Threading;
using System.Timers;
using Timer=System.Timers.Timer;

namespace Idera.SQLcompliance.Core.Agent
{
   //-----------------------------------------------------------------
   // *********    Warning    *******************
   // Do not use this class.  System.Timers.Timer stops firing elapsed
   // event when used in Windows services.  Use ServiceTimer instead.
   //-----------------------------------------------------------------
	/// <summary>
	/// TimerState is for handling periodic tasks.  A timer raises an
	/// event at specified intervals and the event is handled by  the
	/// event handler.
	/// </summary>
	//-----------------------------------------------------------------
   // Note that we don't derive this class from System.Timers.Timer or
   // other Timer classes to control the behavior.
   //-----------------------------------------------------------------
	public class TimerState : IDisposable
	{
      #region Protected Fields
	   // Track whether Dispose has been called.
	   private bool disposed = false;
	   
	   protected Timer                timer;
      protected int                  counter;
      protected ElapsedEventHandler  eventHandler;
      protected int                  state;
      protected int                  interval;

      #endregion

      #region Properties
      public Timer Timer
      {
         get { return timer; }
         //set { timer = value; }
      }

      public int Counter
      {
         get { return counter; }
      }

      // We only support one event handler right now.
      public ElapsedEventHandler EventHandler
      {
         get { return eventHandler; }
         set { eventHandler = value; }
      }

      public int State
      {
         get { return state; }
      }

      /// <summary>
      /// Gets or sets the interval at which to raise the Elapsed event. 
      /// </summary>
      public int Interval
      {
         get { return interval; }
         set 
         { 
            interval = value;
            if( timer != null )
               timer.Interval = interval;
         }
      }

      #endregion

      #region Constructors
		public TimerState() : this( 0 )
		{
		} 

      public TimerState (
         int interval
         )
      {
         this.interval = interval;
         state = 0;
      }

      #endregion

      #region Public Methods
      
      public virtual void 
         Start ()
      {
         if( state > 0 )
            return;

         if( timer == null )
            timer = new System.Timers.Timer( );
         timer.Interval = interval;
         timer.Elapsed += eventHandler;
         timer.AutoReset = true;
         timer.Enabled = true;
         state = 1;
      }

      public virtual void
         Stop()
      {
         state = 0;
         if( timer == null )
         {
            return;
         }

         timer.Elapsed -= eventHandler;
         timer.Enabled = false;
      }

      public virtual void
         Pause()
      {
         timer.Enabled = false;
      }

      public virtual void
         Continue()
      {
         timer.Enabled = true;
      }

      public virtual void
         Reset()
      {
         if( state > 0 )
            Stop();
         counter = 0;
         eventHandler = null;
         interval = 0;
         state = 0;
      }

      public virtual int
         IncrementCounter()
      {
         return ++counter;
      }
      #endregion

	   #region IDisposable Members

	   // Implement IDisposable.
	   // Do not make this method virtual.
	   // A derived class should not be able to override this method.
	   public void Dispose()
	   {
		   Dispose(true);
		   // This object will be cleaned up by the Dispose method.
		   // Therefore, you should call GC.SupressFinalize to
		   // take this object off the finalization queue 
		   // and prevent finalization code for this object
		   // from executing a second time.
		   GC.SuppressFinalize(this);
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
		   if(!disposed)
		   {
			   // If disposing equals true, dispose all managed 
			   // and unmanaged resources.
			   if(disposing)
			   {
				   // Dispose managed resources.
				   if(timer != null)
					   timer.Dispose() ;
			   }
             
			   // Call the appropriate methods to clean up 
			   // unmanaged resources here.
			   // If disposing is false, 
			   // only the following code is executed.
		   }
		   disposed = true;         
	   }

	   // Use C# destructor syntax for finalization code.
	   // This destructor will run only if the Dispose method 
	   // does not get called.
	   // It gives your base class the opportunity to finalize.
	   // Do not provide destructors in types derived from this class.
	   ~TimerState()      
	   {
		   // Do not re-create Dispose clean-up code here.
		   // Calling Dispose(false) is optimal in terms of
		   // readability and maintainability.
		   Dispose(false);
	   }

	   #endregion
   }

   
}
