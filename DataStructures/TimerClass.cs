using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DataStructures
{
    public class TimerClass
    {
        private Timer _timer = null;
        private int _interval;

        public Warehouse timerWH { get; set; }

        public TimerClass(int interval)
        {
            _interval = interval;
        }

        public void Start()
        {
            // Do nothing if timer already setup
            if (_timer != null)
                return;

            // Create a timer instance, have to trigger every '_interval' miliseconds
            _timer = new Timer(_interval); // 1 sec = 1000

            // Add 'OnTimerElapsed' to the 'Elapsed' event handler, so it's called whenever the timer is elapsed
            _timer.Elapsed += OnTimeout;

            // Make the timer auto-reset
            _timer.AutoReset = true;

            // Start timer
            _timer.Start();
        }

        public void Stop()
        {
            // No timer, nothing to stop.
            if (_timer == null)
                return;

            // Stop the timer
            _timer.Stop();

            // Dispose the timer to free up memory
            // Always dispose disposable classes when you're done working with them.
            _timer.Dispose();

            // Unset the '_timer' variable
            _timer = null;
        }

        private void OnTimeout(Object caller, ElapsedEventArgs e)
        {
            Warehouse.CheckExpiryDate(timerWH);
        }
    }
}
