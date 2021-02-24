using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Helpers.AnimationHelper
{
    /// <summary>
    /// Class for helping animating using animation curve.
    /// Curve must start with (0, 0) end end with (1, 1) for proper interpolation reason,
    /// but intermediate values can be below 0 and above 1, if you want to do "bounce" effects and e.t.c.
    /// </summary>
    public static class CurveAnimationHelper
    {
        private static readonly YieldInstruction Wait = new WaitForEndOfFrame();
        public static readonly AnimationCurve DefaultCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        /// <summary>
        /// Moves object to the <paramref name="targetPosition"/> with specified <paramref name="speed"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// from <see cref="AnimationCurve"/>.
        /// <para></para>
        /// You can change global or local position, defining <paramref name="isLocalPosition"/>
        /// </summary>
        public static Coroutine Move(MonoBehaviour mb, AnimationCurve animationCurve, Transform transform, Vector3 targetPosition,
            float speed = 2.5f, bool isLocalPosition = false)
        {
            CheckCurve(animationCurve);
            if (isLocalPosition)
            {
                return mb.StartCoroutine
                (
                    LerpVectorByCurve
                    (
                        result => transform.localPosition = result,
                        animationCurve, transform.localPosition, targetPosition, speed
                    )
                );
            }
            else
            {
                return mb.StartCoroutine
                (
                    LerpVectorByCurve
                    (
                        result => transform.position = result,
                        animationCurve, transform.position, targetPosition, speed
                    )
                );
            }
        }
        
        /// <summary>
        /// Rotates object to the <paramref name="targetQuaternion"/> with specified <paramref name="speed"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// from <see cref="AnimationCurve"/>.
        /// <para></para>
        /// You can change global or local rotation, defining <paramref name="isLocalRotation"/>
        /// </summary>
        public static Coroutine Rotate(MonoBehaviour mb, AnimationCurve animationCurve, Transform transform, Quaternion targetQuaternion,
            float speed = 2.5f, bool isLocalRotation = false)
        {
            CheckCurve(animationCurve);
            if (isLocalRotation)
            {
                return mb.StartCoroutine
                (
                    LerpQuarternionByCurve
                    (
                        result => transform.localRotation = result,
                        animationCurve, transform.localRotation, targetQuaternion, speed
                    )
                );
            }
            else
            {
                return mb.StartCoroutine
                (
                    LerpQuarternionByCurve
                    (
                        result => transform.rotation = result,
                        animationCurve, transform.rotation, targetQuaternion, speed
                    )
                );
            }
        }

        /// <summary>
        /// Changes objects local scale to the <paramref name="targetScale"/> with specified <paramref name="speed"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// from <see cref="AnimationCurve"/>.
        /// </summary>
        public static Coroutine Scale(MonoBehaviour mb, AnimationCurve animationCurve, Transform transform,
            Vector3 targetScale, float speed)
        {
            CheckCurve(animationCurve);
            return mb.StartCoroutine
            (LerpVectorByCurve
                (
                    result => transform.localScale = result, animationCurve, transform.localScale, targetScale, speed
                )
            );
        }

        /// <summary>
        /// Changes graphic's alpha to the <paramref name="targetAlpha"/> with specified <paramref name="speed"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// </summary>
        public static Coroutine Fade(MonoBehaviour mb, AnimationCurve animationCurve, MaskableGraphic graphic,
            float targetAlpha, float speed = 2.5f)
        {
            CheckCurve(animationCurve);
            return mb.StartCoroutine
            (
                LerpFloatByCurve
                (
                    result =>
                    {
                        Color originalColor = graphic.color;
                        graphic.color = new Color(originalColor.r, originalColor.g, originalColor.b, result);
                    },
                    animationCurve, graphic.color.a, targetAlpha, speed
                )
            );
        }

        /// <summary>
        /// Changes graphic's color to the <paramref name="targetColor"/> with specified <paramref name="speed"/>
        /// by starting coroutine on specified <see cref="MonoBehaviour"/>, using interpolating values
        /// </summary>
        public static Coroutine Recolor(MonoBehaviour mb, AnimationCurve animationCurve, MaskableGraphic graphic,
            Color targetColor, float speed = 2.5f)
        {
            CheckCurve(animationCurve);
            return mb.StartCoroutine
            (
                LerpColorByCurve
                (
                    result => graphic.color = result, animationCurve, graphic.color, targetColor, speed
                )
            );
        }

        public static IEnumerator LerpFloatByCurve(Action<float> variable, AnimationCurve curve, float startValue, float targetValue, float speed = 2.5f)
        {
            float step = 0;
            if (Math.Abs(startValue - targetValue) > double.Epsilon)
            {
                while (step <= 1)
                {
                    float interpolatedValue = Mathf.LerpUnclamped(startValue, targetValue, curve.Evaluate(step));
                    variable(interpolatedValue);
                    step += Time.deltaTime * speed;
                    yield return Wait;
                }
            }
            variable(targetValue);
            yield return Wait;
        }
        
        private static IEnumerator LerpVectorByCurve(Action<Vector3> variable,  AnimationCurve curve, Vector3 startVector, Vector3 targetVector, float speed = 2.5f)
        {
            float step = 0;
            if (!Equals(startVector, targetVector))
            {
                while (step <= 1)
                {
                    Vector3 interpolatedValue = Vector3.LerpUnclamped(startVector, targetVector, curve.Evaluate(step));
                    variable(interpolatedValue);
                    step += Time.deltaTime * speed;
                    yield return Wait;
                }
            }
            variable(targetVector);
            yield return Wait;
        }

        private static IEnumerator LerpQuarternionByCurve(Action<Quaternion> variable, AnimationCurve curve,
            Quaternion startQuaternion, Quaternion targetQuaternion, float speed = 2.5f)
        {
            float step = 0;
            if (!Equals(startQuaternion, targetQuaternion))
            {
                while (step <= 1)
                {
                    Quaternion interpolatedValue = Quaternion.LerpUnclamped(startQuaternion, targetQuaternion, curve.Evaluate(step));
                    variable(interpolatedValue);
                    step += Time.deltaTime * speed;
                    yield return Wait;
                }
            }
            variable(targetQuaternion);
            yield return Wait;
        }

        private static IEnumerator LerpColorByCurve(Action<Color> variable, AnimationCurve curve,
            Color startColor, Color targetColor, float speed = 2.5f)
        {
            float step = 0;
            if (!Equals(startColor, targetColor))
            {
                while (step <= 1)
                {
                    Color interpolatedValue = Color.LerpUnclamped(startColor, targetColor, curve.Evaluate(step));
                    variable(interpolatedValue);
                    step += Time.deltaTime * speed;
                    yield return Wait;
                }
            }
            variable(targetColor);
            yield return Wait;
        }

        /// <summary>
        /// Checks whether the curve fits or not.
        /// Fitting curve must start with (0, 0) end end with (1, 1).
        /// </summary>
        private static void CheckCurve(AnimationCurve curve)
        {
            if (curve[0].time > 0 || curve[0].value > 0)
            {
                throw new ArgumentException("[CurveAnimatorHelper] Curve must start with (0, 0) end end with (1, 1)!");
            }

            if (curve[curve.length-1].time > 1 || curve[curve.length-1].time < 1 || curve[curve.length-1].value > 1 ||
                curve[curve.length-1].time < 1)
            {
                throw new ArgumentException("[CurveAnimatorHelper] Curve must start with (0, 0) end end with (1, 1)!");
            }
        }
    }
}