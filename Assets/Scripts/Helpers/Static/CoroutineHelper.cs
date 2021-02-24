﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Helpers
{
    public static class CoroutineHelper
    {
        /// <summary>
        /// Checking if coroutine coroutineVariable executing, stopping if it do.
        /// Then starting coroutine passed in ""coroutine" variable.
        /// </summary>
        /// <param name="coroutineVariable">Coroutine variable from caller script.</param>
        /// <param name="coroutine">IEnumerator type variable were all coroutine params passed.</param>
        /// <param name="coroutineOwner">MonoBehaviour from which this method called.</param>
        public static Coroutine RestartCoroutine(ref Coroutine coroutineVariable, IEnumerator coroutine,
            MonoBehaviour coroutineOwner)
        {
            StopCoroutine(ref coroutineVariable, coroutineOwner);

            if (!coroutineOwner) return coroutineVariable = null;
            
            if(coroutineOwner.gameObject.activeInHierarchy)
            {
                return coroutineVariable = coroutineOwner.StartCoroutine(coroutine);
            }

            return coroutineVariable = null;
        }
        
        public static Coroutine RestartCoroutine<T>(ref Coroutine coroutineVariable, IEnumerator<T> coroutine,
                                                 MonoBehaviour coroutineOwner) where T : Object
        {
            StopCoroutine(ref coroutineVariable, coroutineOwner);

            return  coroutineVariable = coroutineOwner.StartCoroutine(coroutine);
        }

        /// <summary>
        /// Stopping coroutine coroutineVariable, on passed MonoBehaviour 
        /// </summary>
        /// <param name="coroutineVariable"></param>
        /// <param name="owner"></param>
        public static void StopCoroutine(ref Coroutine coroutineVariable, MonoBehaviour owner)
        {
            if (coroutineVariable != null)
            {
                owner.StopCoroutine(coroutineVariable);
                coroutineVariable = null;
            }
        }
    }
}