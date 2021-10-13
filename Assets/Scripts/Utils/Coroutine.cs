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
        public static IAsyncResult RunAsyncCatch(this Action routine, AsyncCallback callback = null, Action onError = null) {
            Action tryRoutine = () => {
                try {
                    routine();
                } catch {
                    onError?.Invoke();
                }
            };
            return tryRoutine.BeginInvoke(callback, null);
        }
        
        public static IAsyncResult RunAsync(this Action routine, AsyncCallback callback = null) {
            return routine.BeginInvoke(callback, null);
        }
        
        public static IAsyncResult RunAsync<T>(this Func<T> routine, AsyncCallback callback, CoroutineResult<T> result = null) {
            Func<T> task = () => {
                var r = routine();
                result.ReturnValue(r);
                return r;
            };
            
            return task.BeginInvoke(callback, null);
        }
        
        public static IEnumerator RunAsyncCo(this Action routine, AsyncCallback callback = null) {
            var ar = routine.BeginInvoke(callback, null);
            yield return new WaitUntil(() => ar.IsCompleted);
        }
        
        public static IEnumerator RunAsyncCo<T>(this Func<T> routine, AsyncCallback callback, CoroutineResult<T> result = null) {
            Func<T> task = () => {
                var r = routine();
                result.ReturnValue(r);
                return r;
            };
            
            var ar = task.BeginInvoke(callback, null);
            yield return new WaitUntil(() => ar.IsCompleted);
        }

        public static IEnumerator MakeCoroutine(Action action, int frameDelay = 1) {
            for (int i = 0; i < frameDelay; i++) {
                yield return null;
            }
            action();
            yield break;
        }
    }
}