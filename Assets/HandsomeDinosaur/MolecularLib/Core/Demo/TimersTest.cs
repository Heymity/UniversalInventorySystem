using MolecularLib.Timers;
using UnityEngine;

namespace MolecularLib.Demo
{
    public class TimersTest : MonoBehaviour
    {
        private TimerReference _timerReference;
        private Timer _timer;

        [ContextMenu("Timer Tests/TestTimerAsync")]
        private void TestTimerAsync()
        {
            Timer.TimerAsync(5, () => Debug.Log("Timer (5s) Async Finished"));
        }
        
        [ContextMenu("Timer Tests/TestTimerReferenceAsync")]
        private void TestTimerReferenceAsync()
        {
            _timerReference = Timer.TimerAsyncReference(6);
            _timerReference.OnFinish = () => Debug.Log("Timer Async (6s) Reference Finished");
        }
        
        [ContextMenu("Timer Tests/TestTimerReference REPEAT Async")]
        private void TestTimerReferenceRepeatAsync()
        {
            _timerReference = Timer.TimerAsyncReference(2, true);
            _timerReference.OnFinish = () => Debug.Log("Timer Async (2s) Reference Repeat Finished");
        }
        
        [ContextMenu("Timer Tests/Stop repeat")]
        private void TestTimerReferenceStopRepeatAsync()
        {
            _timerReference.StopOnNextCycle();
        }
        
        [ContextMenu("Timer Tests/Instance Timer Test")]
        private void InstanceTimerTest()
        {
            _timer = Timer.Create(5, () => Debug.Log("Timer (5s) Instance Finished"));
        }

        private void Update()
        {
            if (_timerReference is { HasFinished: false })
            {
                Debug.Log($"Async timer reference elapsed seconds: {_timerReference.ElapsedSeconds}");
            }
            
            if (_timer is { HasFinished: false })
            {
                Debug.Log($"Instance timer elapsed seconds: {_timer.ElapsedSeconds}");
            }
        }
    }
}
