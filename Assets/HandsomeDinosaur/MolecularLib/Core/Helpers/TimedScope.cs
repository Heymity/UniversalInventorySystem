using System;
using System.Diagnostics;

namespace MolecularLib.Helpers
{
    public class TimedScope : IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly string _message;
        
        public TimedScope(string message = "")
        {
            _message = message;
            
            _stopwatch.Start();
        }
        
        public void Dispose()
        {
            _stopwatch.Stop();

            UnityEngine.Debug.Log($"{_message} completed in {_stopwatch.ElapsedMilliseconds}ms");
        }
    }
}