using System;
using System.Collections;
using System.Threading.Tasks;
using MolecularLib.Helpers;
using UnityEngine;

namespace MolecularLib.Timers
{
    public class Timer
    {
        public float StartTime { get; private set; }
        public bool Repeat { get; set; }
        
        public bool IsStopped { get; private set; }
        
        public float DurationInSeconds { get; private set; }

        public event Action OnComplete;
        
        public float ElapsedSeconds => Time.time - StartTime;
        public bool HasFinished => ElapsedSeconds >= DurationInSeconds;
        
        public IEnumerator TimerCoroutine { get; set; }
        
        public static Timer Create(float duration, Action callback, bool repeat = false)
        {
            var timer = new Timer
            {
                StartTime = Time.time,
                DurationInSeconds = duration,
                Repeat = repeat
            };

            timer.OnComplete += callback;

            timer.TimerCoroutine = timer.StartTimer();
            TimerManager.Current.AddTimer(timer);

            return timer;
        }

        public void RestartTimer()
        {
            IsStopped = false;
            
            StartTime = Time.time;
            TimerCoroutine = StartTimer();
   
            if (!TimerManager.Current.HasTimer(this))
                TimerManager.Current.AddTimer(this);
        }
        
        public void StopTimer()
        {
            TimerManager.Current.RemoveTimer(this);
            IsStopped = true;
        }

        public void ResumeTimer()
        {
            TimerCoroutine = StartTimer();
            TimerManager.Current.AddTimer(this);
        }
        
        public IEnumerator StartTimer()
        {
            IsStopped = false;
            yield return new WaitForSeconds(DurationInSeconds);
            if (IsStopped) yield break;
            
            IsStopped = true;
            OnComplete?.Invoke();
        }
        
        #region Async Timers

        /// <summary>
        /// Makes a timer using the await Task.Delay() method, not needing to create a MonoBehaviour
        /// </summary>
        /// <param name="seconds">The seconds to wait until call the callback</param>
        /// <param name="callback">The function to be called when the timer expires</param>
        public static async void TimerAsync(float seconds, Action callback)
        {
            await Task.Delay((int) (seconds * 1000));
            
            if (!PlayStatus.IsPlaying)
                return;
            
            callback?.Invoke();
        }
        
        private static async void TimerAsyncReference(TimerReference timerReference, Action repeatCallback)
        {
            await Task.Delay(timerReference.DurationInMilliseconds);
            
            if (!PlayStatus.IsPlaying)
                return;
            
            timerReference.OnFinish?.Invoke();
            repeatCallback?.Invoke();
        }

        /// <summary>
        /// Makes a timer using the await Task.Delay() method, not needing to create a MonoBehaviour and returning a TimerReference, allowing to get the elapsed time, the Start Time, to make repeatable timers and reassign the callback
        /// </summary>
        /// <param name="seconds">The seconds to wait until call the callback</param>
        /// <param name="repeat">Whether the timer should repeat after conclusion or not</param>
        public static TimerReference TimerAsyncReference(float seconds, bool repeat = false)
        {
            var reference = new TimerReference
            {
                DurationInMilliseconds = (int) seconds * 1000,
                StartTime = Time.time,
                Repeat = repeat
            };

            TimerAsyncReference(reference, !reference.Repeat ? reference.OnFinish : HandleRepeat);
            
            return reference;

            void HandleRepeat()
            {
                if (!PlayStatus.IsPlaying)
                    return;

                if (!reference.Repeat) return;
                
                reference.OnFinish?.Invoke();
                reference.StartTime = Time.time;
                TimerAsync(seconds, HandleRepeat);
            }
        }

        #endregion
    }
    
    public class TimerReference
    {
        public bool Repeat { get; internal set; }
        public float StartTime { get; internal set; }
        public int DurationInMilliseconds { get; internal set; }
        public bool HasFinished => ElapsedMilliseconds >= DurationInMilliseconds;
        public Action OnFinish;
        public float ElapsedSeconds => Time.time - StartTime;
        public int ElapsedMilliseconds => (int)ElapsedSeconds * 1000;
            
        public void StopOnNextCycle() => Repeat = false;
    }
}
