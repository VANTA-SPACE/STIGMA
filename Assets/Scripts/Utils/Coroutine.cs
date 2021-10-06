using System;
using System.Collections;
using UnityEngine;

namespace Utils {
    public class CoroutineResult<T> {
        public T Value;
        public Action<T> Callback;
        public CoroutineResult(Action<T> callback = null) {
            Callback = callback;
        }
    }
    
    public static class Coroutine {
        public static IAsyncResult RunAsync(this Action routine, AsyncCallback callback) {
            return routine.BeginInvoke(callback, null);
        }
        
        public static IAsyncResult RunAsync<T>(this Func<T> routine, AsyncCallback callback, CoroutineResult<T> result) {
            Func<T> task = () => {
                var r = routine();
                result.ReturnValue(r);
                return r;
            };
            
            return task.BeginInvoke(callback, null);
        }
        
        public static IEnumerator RunAsyncCo(this Action routine, AsyncCallback callback) {
            var ar = routine.BeginInvoke(callback, null);
            yield return new WaitUntil(() => ar.IsCompleted);
        }
        
        public static IEnumerator RunAsyncCo<T>(this Func<T> routine, AsyncCallback callback, CoroutineResult<T> result) {
            Func<T> task = () => {
                var r = routine();
                result.ReturnValue(r);
                return r;
            };
            
            var ar = task.BeginInvoke(callback, null);
            yield return new WaitUntil(() => ar.IsCompleted);
        }
    }
}