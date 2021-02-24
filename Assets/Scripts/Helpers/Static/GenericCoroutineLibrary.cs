using System;
using System.Collections;
using UnityEngine;

public static class GenericCoroutineLibrary
{
    public static IEnumerator Timer(float timerDuration, Action<float> tickAction = null, Action timeUpAction = null)
    {
        float timePassed = 0.0f;

        while (timePassed < timerDuration)
        {
            timePassed += Time.deltaTime;
            tickAction?.Invoke(timePassed);
            yield return null;
        }

        timeUpAction?.Invoke();
    }
    
    public static IEnumerator Timer(float timerDuration, float tickTime, Action<float> tickAction = null, Action timeUpAction = null)
    {
        float timePassed = 0.0f;
        var delay = new WaitForSeconds(tickTime);

        while (timePassed < timerDuration)
        {
            timePassed += tickTime;
            tickAction?.Invoke(timePassed);
            yield return delay;
        }

        timeUpAction?.Invoke();
    }
}
