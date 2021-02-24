using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Common.Helpers
{
    /// <summary>
    /// Class that contains all base UI animation coroutines.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class UIAnimationHelper
    {
        #region Lumers Animation Helper

        private static readonly YieldInstruction WaitForFixedUpdate               = new WaitForFixedUpdate();
        private const  float            StartAnimationBoost = 6f;
        private const  float            InterpolationStep   = 0.01f;

        // anchors of the RectTransform
        private struct Anchors
        {
            public readonly Vector2 AnchorsMin;
            public readonly Vector2 AnchorsMax;

            public Anchors(Vector2 anchorsMin, Vector2 anchorsMax)
            {
                AnchorsMin = anchorsMin;
                AnchorsMax = anchorsMax;
            }
        }

        public static Coroutine SetAnchors(MonoBehaviour mb, RectTransform rc, Vector2 anchorsMin, Vector2 anchorsMax,
                                           float         timeInSeconds = 1.0f)
        {
            var targetAnchors = new Anchors(anchorsMin, anchorsMax);
            return mb.StartCoroutine(AnchorMovingProcess(rc, targetAnchors, timeInSeconds));
        }

        public static Coroutine SetAnchorsUnscaled(MonoBehaviour mb, RectTransform rc, Vector2 anchorsMin, Vector2 anchorsMax,
                                                   float         speed = 1.0f)
        {
            var targetAnchors = new Anchors(anchorsMin, anchorsMax);
            return mb.StartCoroutine(AnchorMovingProcessUnscaled(rc, targetAnchors, speed));
        }

        public static Coroutine SetPosition(MonoBehaviour mb, RectTransform rc, Vector3 localPosition,
                                            float         timeInSeconds = 1.0f)
        {
            return mb.StartCoroutine(ChangePositionProcess(rc, localPosition, timeInSeconds));
        }

        public static Coroutine SetPositionUnscaled(MonoBehaviour mb, Transform transform, Vector3 targetPosition, float speed = 1.0f)
        {
            return mb.StartCoroutine(VectorLerpProcessUnscaled(result => transform.position = result, transform.position, targetPosition, speed));
        }

        public static Coroutine SetRotation(MonoBehaviour mb, RectTransform rc, Vector3 localRotation,
                                            float         timeInSeconds = 1.0f)
        {
            return mb.StartCoroutine(ChangeRotationProcess(rc, localRotation, timeInSeconds));
        }

        public static Coroutine SetRotationUnscaled(MonoBehaviour mb, RectTransform rc, Vector3 localRotation,
                                                    float         timeInSeconds = 1.0f)
        {
            return mb.StartCoroutine(ChangeRotationProcessUnscaled(rc, localRotation, timeInSeconds));
        }

        public static Coroutine SetScale(MonoBehaviour mb, Transform transform, Vector3 localScale,
                                         float         timeInSeconds = 1.0f)
        {
            return mb.StartCoroutine(ChangeScaleProcess(transform, localScale, timeInSeconds));
        }

        public static Coroutine SetAlpha(MonoBehaviour mb, MaskableGraphic graphic, float alpha,
                                         float         timeInSeconds = 1.0f)
        {
            return mb.StartCoroutine(AlphaChangingProcess(graphic, alpha, timeInSeconds));
        }
        
        public static IEnumerator SetAlphaIE(MonoBehaviour mb, CanvasGroup cg, float alpha, float         timeInSeconds = 1.0f)
        {
            yield return SetAlpha(mb, cg, alpha, timeInSeconds);
        }

        public static Coroutine SetAlphaUnscaled(MonoBehaviour mb, MaskableGraphic graphic, float alpha,
                                                 float         speed = 1.0f)
        {
            return mb.StartCoroutine(AlphaChangingProcessUnscaled(graphic, alpha, speed));
        }

        public static Coroutine SetAlpha(MonoBehaviour mb, CanvasGroup cg, float alpha, float timeInSeconds = 1.0f)
        {
            return mb.StartCoroutine(AlphaChangingProcess(cg, alpha, timeInSeconds));
        }

        public static Coroutine SetAlphaUnscaled(MonoBehaviour mb, CanvasGroup cg, float alpha, float speed = 1.0f)
        {
            return mb.StartCoroutine(AlphaChangingProcessUnscaled(cg, alpha, speed));
        }

        private static IEnumerator ChangePositionProcess(RectTransform rc, Vector3 targetLocalPosition,
                                                         float         timeInSeconds)
        {
            var startPosition = rc.localPosition;
            float   timePassed    = 0;
            while (timePassed < timeInSeconds)
            {
                rc.localPosition =  Vector3.Lerp(startPosition, targetLocalPosition, timePassed / timeInSeconds);
                timePassed       += Time.deltaTime;
                yield return WaitForFixedUpdate;
            }

            rc.localPosition = targetLocalPosition;
            yield return WaitForFixedUpdate;
        }

        private static IEnumerator ChangeRotationProcess(RectTransform rc, Vector3 targetLocalRotation,
                                                         float         timeInSeconds)
        {
            var startRotation = rc.localEulerAngles;
            float   timePassed    = 0;
            while (timePassed < timeInSeconds)
            {
                rc.localRotation = Quaternion.Lerp(Quaternion.Euler(startRotation),
                                                   Quaternion.Euler(targetLocalRotation), timePassed / timeInSeconds);
                timePassed += Time.deltaTime;
                yield return WaitForFixedUpdate;
            }

            rc.localEulerAngles = targetLocalRotation;
            yield return WaitForFixedUpdate;
        }

        private static IEnumerator ChangeRotationProcessUnscaled(RectTransform rc, Vector3 targetLocalRotation,
                                                                 float         speed)
        {
            var startRotation = rc.localEulerAngles;
            var   boost         = StartAnimationBoost;
            var   interpolate   = InterpolationStep * StartAnimationBoost * speed;
            while (interpolate <= 1)
            {
                rc.localRotation = Quaternion.Lerp(Quaternion.Euler(startRotation),
                                                   Quaternion.Euler(targetLocalRotation), interpolate);
                interpolate += Time.deltaTime * speed * boost;
                if (boost > 1) boost -= Time.deltaTime * speed;
                yield return WaitForFixedUpdate;
            }

            rc.localEulerAngles = targetLocalRotation;
            yield return WaitForFixedUpdate;
        }

        private static IEnumerator ChangeScaleProcess(Transform transform, Vector3 targetLocalScale,
                                                      float     timeInSeconds)
        {
            var startScale = transform.localScale;
            var   timePassed = 0.0f;
            while (timePassed < timeInSeconds)
            {
                transform.localScale =  Vector3.Lerp(startScale, targetLocalScale, timePassed / timeInSeconds);
                timePassed           += Time.deltaTime;
                yield return null;
            }

            transform.localScale = targetLocalScale;
            yield return WaitForFixedUpdate;
        }

        private static IEnumerator AnchorMovingProcess(RectTransform rc, Anchors anchors, float timeInSeconds)
        {
            var timePassed = 0.0f;

            while (timePassed < timeInSeconds)
            {
                var newMinX = Mathf.Lerp(rc.anchorMin.x, anchors.AnchorsMin.x, timePassed / timeInSeconds);
                var newMaxX = Mathf.Lerp(rc.anchorMax.x, anchors.AnchorsMax.x, timePassed / timeInSeconds);
                var newMinY = Mathf.Lerp(rc.anchorMin.y, anchors.AnchorsMin.y, timePassed / timeInSeconds);
                var newMaxY = Mathf.Lerp(rc.anchorMax.y, anchors.AnchorsMax.y, timePassed / timeInSeconds);

                rc.anchorMin = new Vector2(newMinX, newMinY);
                rc.anchorMax = new Vector2(newMaxX, newMaxY);

                // smoothly increase interpolator value
                timePassed += Time.deltaTime;

                yield return WaitForFixedUpdate;
            }

            rc.anchorMax = new Vector2(anchors.AnchorsMax.x, anchors.AnchorsMax.y);
            rc.anchorMin = new Vector2(anchors.AnchorsMin.x, anchors.AnchorsMin.y);
        }

        private static IEnumerator AnchorMovingProcessUnscaled(RectTransform rc, Anchors anchors, float speed)
        {
            var boost       = StartAnimationBoost;
            var interpolate = InterpolationStep * boost * speed;
            while (interpolate < 1)
            {
                var newMinX = Mathf.Lerp(rc.anchorMin.x, anchors.AnchorsMin.x, interpolate);
                var newMaxX = Mathf.Lerp(rc.anchorMax.x, anchors.AnchorsMax.x, interpolate);
                var newMinY = Mathf.Lerp(rc.anchorMin.y, anchors.AnchorsMin.y, interpolate);
                var newMaxY = Mathf.Lerp(rc.anchorMax.y, anchors.AnchorsMax.y, interpolate);

                rc.anchorMin = new Vector2(newMinX, newMinY);
                rc.anchorMax = new Vector2(newMaxX, newMaxY);

                // smoothly increase interpolator value
                interpolate += Time.deltaTime * speed * boost;
                if (boost > 1) boost -= Time.deltaTime * speed;

                yield return WaitForFixedUpdate;
            }

            rc.anchorMax = new Vector2(anchors.AnchorsMax.x, anchors.AnchorsMax.y);
            rc.anchorMin = new Vector2(anchors.AnchorsMin.x, anchors.AnchorsMin.y);
        }

        private static IEnumerator AlphaChangingProcess(CanvasGroup cg, float targetAlpha, float timeInSeconds)
        {
            var timePassed = 0.0f;
            while (timePassed < timeInSeconds)
            {
                cg.alpha   =  Mathf.Lerp(cg.alpha, targetAlpha, timePassed / timeInSeconds);
                timePassed += Time.deltaTime;
                yield return WaitForFixedUpdate;
            }

            cg.alpha = targetAlpha;
            yield return WaitForFixedUpdate;
        }

        private static IEnumerator AlphaChangingProcess(MaskableGraphic mg, float targetAlpha, float timeInSeconds)
        {
            var timePassed = 0.0f;
            var alpha      = mg.color.a;
            while (timePassed < timeInSeconds)
            {
                alpha      =  Mathf.Lerp(alpha, targetAlpha, timePassed / timeInSeconds);
                
                mg.color = mg.color.ChangeAlpha(alpha);
                
                timePassed += Time.deltaTime;
                yield return WaitForFixedUpdate;
            }

            alpha    = targetAlpha;
            
            mg.color = mg.color.ChangeAlpha(alpha);
            
            yield return WaitForFixedUpdate;
        }

        private static IEnumerator AlphaChangingProcessUnscaled(CanvasGroup cg, float targetAlpha, float speed)
        {
            var boost       = StartAnimationBoost;
            var interpolate = InterpolationStep * speed * boost;
            if (Math.Abs(targetAlpha - cg.alpha) > double.Epsilon)
            {
                var startAlpha = cg.alpha;
                while (interpolate <= 1f)
                {
                    cg.alpha    =  Mathf.Lerp(startAlpha, targetAlpha, interpolate);
                    interpolate += Time.deltaTime * speed * boost;
                    if (boost > 1) boost -= Time.deltaTime * speed;
                    yield return WaitForFixedUpdate;
                }
            }

            cg.alpha = targetAlpha;
            yield return WaitForFixedUpdate;
        }

        private static IEnumerator AlphaChangingProcessUnscaled(MaskableGraphic mg, float targetAlpha, float speed)
        {
            var boost       = StartAnimationBoost;
            var interpolate = InterpolationStep * speed * boost;
            var startAlpha  = mg.color.a;
            if (Math.Abs(targetAlpha - mg.color.a) > double.Epsilon)
                while (interpolate <= 1)
                {
                    var newAlpha = Mathf.Lerp(startAlpha, targetAlpha, interpolate);

                    mg.color = mg.color.ChangeAlpha(newAlpha);

                    interpolate += Time.deltaTime * speed * boost;
                    if (boost > 1) boost -= Time.deltaTime * speed;
                    yield return WaitForFixedUpdate;
                }

            mg.color = mg.color.ChangeAlpha(targetAlpha);
            yield return null;
        }

        private static Color ChangeAlpha(this Color originalColor, float newAlpha)
        {
            return new Color(originalColor.r, originalColor.b, originalColor.g, newAlpha);
        }

        private static IEnumerator FloatLerpProcessUnscaled(Action<float> variable, float startValue, float targetValue, float speed)
        {
            var boost       = StartAnimationBoost;
            var interpolate = InterpolationStep * speed * boost;
            if (Math.Abs(startValue - targetValue) > double.Epsilon)
            {
                while (interpolate <= 1)
                {
                    var interpolatedValue = Mathf.Lerp(startValue, targetValue, interpolate);
                    variable(interpolatedValue);
                    interpolate += Time.deltaTime * speed * boost;
                    if (boost > 1) boost -= Time.deltaTime * speed;
                    yield return WaitForFixedUpdate;
                }
            }

            variable(targetValue);
            yield return WaitForFixedUpdate;
        }

        private static IEnumerator VectorLerpProcessUnscaled(Action<Vector3> variable, Vector3 startVector, Vector3 targetVector, float speed)
        {
            var boost       = StartAnimationBoost;
            var interpolate = InterpolationStep * speed * boost;
            if (!Equals(startVector, targetVector))
            {
                while (interpolate <= 1)
                {
                    var interpolatedValue = Vector3.Lerp(startVector, targetVector, interpolate);
                    variable(interpolatedValue);

                    interpolate += Time.deltaTime * speed * boost;
                    if (boost > 1) boost -= Time.deltaTime * speed;
                    yield return WaitForFixedUpdate;
                }
            }

            variable(targetVector);
            yield return WaitForFixedUpdate;
        }

        #endregion

        #region [TBringerOHW Animation Helper]

        public static IEnumerator AnimateImgFill(Image targetObject,  float  targetValue,
                                                 float animTime = 1f, Action callback = null)
        {
            var startFillValue = targetObject.fillAmount;
            float timePassed     = 0;

            while (Math.Abs(targetObject.fillAmount - targetValue) > 0.00001f && timePassed < animTime)
            {
                targetObject.fillAmount = Mathf.Lerp(startFillValue, targetValue, timePassed / animTime);

                timePassed += Time.deltaTime;

                yield return null;
            }

            targetObject.fillAmount = targetValue;

            callback?.Invoke();
        }

        public static IEnumerator AnimateSliderFill(Slider targetObject,  float  targetValue,
                                                    float  animTime = 1f, Action callback = null)
        {
            var startSliderValue = targetObject.value;
            float timePassed       = 0;

            while (Math.Abs(targetObject.value - targetValue) > 0.00001f && timePassed < animTime)
            {
                targetObject.value = Mathf.Lerp(startSliderValue, targetValue, timePassed / animTime);

                timePassed += Time.deltaTime;

                yield return null;
            }

            targetObject.value = targetValue;

            callback?.Invoke();
        }

        public static IEnumerator RescaleText(Text targetObject, int targetValue, float animTime = 1f)
        {
            var   startFillValue = targetObject.fontSize;
            float timePassed     = 0;

            while (targetObject.fontSize != targetValue)
            {
                targetObject.fontSize = (int) Mathf.Lerp(startFillValue, targetValue, timePassed / animTime);

                timePassed += Time.deltaTime;

                yield return null;
            }

            targetObject.fontSize = targetValue;
        }


        public static IEnumerator RescaleElement(Transform targetObject, float targetValue, float animTime = 1f)
        {
            var startFillValue    = targetObject.localScale;
            var targetVectorValue = new Vector3(targetValue, targetValue, startFillValue.z);
            float   timePassed        = 0;

            while (Math.Abs(targetObject.localScale.x - targetValue) > float.Epsilon)
            {
                targetObject.localScale = Vector3.Lerp(startFillValue, targetVectorValue, timePassed / animTime);

                timePassed += Time.deltaTime;

                yield return null;
            }

            targetObject.localScale = targetVectorValue;
        }


        public static IEnumerator FlipElement(RectTransform target, Vector3 rotationAxis, MonoBehaviour owner, float animationTime = 1f, Action action = null, IEnumerator coroutineAction = null)
        {
            yield return SetRotation(owner, target, rotationAxis * 90.0f, animationTime * 0.5f);

            action?.Invoke();

            if (coroutineAction != null)
            {
                Coroutine coroutineRef = null;
                yield return CoroutineHelper.RestartCoroutine(ref coroutineRef, coroutineAction, owner);
            }

            yield return SetRotation(owner, target, rotationAxis * 0.0f, animationTime * 0.5f);
        }


        public static IEnumerator TransformPulse(Transform targetObject, float targetValue, MonoBehaviour owner, float animTime = 1f)
        {
            yield return owner.StartCoroutine(RescaleElement(targetObject, targetValue, animTime));

            yield return new WaitForSeconds(animTime * 0.2f);

            yield return owner.StartCoroutine(RescaleElement(targetObject, 1f, animTime));
        }

        public static IEnumerator EndlessTransformPulsing(Transform targetObject, float targetValue, MonoBehaviour owner, float animTime = 1f,
                                                          float     pauseTime = 1f)
        {
            YieldInstruction wfs = new WaitForSeconds(pauseTime);

            Coroutine pulseCoroutine = null;

            while (true)
            {
                yield return CoroutineHelper.RestartCoroutine(ref pulseCoroutine,
                                                              TransformPulse(targetObject, targetValue, owner, animTime),
                                                              owner);
                yield return wfs;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public static IEnumerator ImageBlink<T>(T     targetObject,  Color targetValue,      MonoBehaviour owner,
                                                float animTime = 1f, float delayTime = 0.2f, Color         startColor = new Color()) where T : MaskableGraphic
        {
            var colorBuffer = startColor == new Color() ? targetObject.color : startColor;

            yield return owner.StartCoroutine(AnimateImageColor(targetObject, targetValue, animTime));

            yield return new WaitForSeconds(animTime * delayTime);

            yield return owner.StartCoroutine(AnimateImageColor(targetObject, colorBuffer, animTime));
        }

        public static IEnumerator MaterialBlink(Material targetObject,           Color targetValue,      MonoBehaviour owner,
                                                float    animTime          = 1f, float delayTime = 0.2f, Color         startColor = new Color(),
                                                string   colorPropertyName = "_Color")
        {
            var colorBuffer = startColor == new Color() ? targetObject.color : startColor;

            yield return owner.StartCoroutine(AnimateMaterialColor(targetObject, colorPropertyName, targetValue, animTime));

            yield return new WaitForSeconds(animTime * delayTime);

            yield return owner.StartCoroutine(AnimateMaterialColor(targetObject, colorPropertyName, colorBuffer, animTime));
        }

        public static IEnumerator EndlessBlinking<T>(T     targetObject, Color targetValue, MonoBehaviour owner,
                                                     float animTime,     float pauseTime) where T : MaskableGraphic
        {
            YieldInstruction wfs = new WaitForSeconds(pauseTime);

            Coroutine blinkCoroutine = null;

            while (true)
            {
                yield return CoroutineHelper.RestartCoroutine(ref blinkCoroutine,
                                                              ImageBlink(targetObject, targetValue, owner, animTime),
                                                              owner);

                yield return wfs;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public static IEnumerator AnimateImageColor<T>(T targetObject, Color targetValue, float animTime = 1f)
            where T : MaskableGraphic
        {
            var   startValue = targetObject.color;
            float timePassed = 0;

            while (timePassed <= animTime)
            {
                targetObject.color = Color.Lerp(startValue, targetValue, timePassed / animTime);

                timePassed += Time.deltaTime;

                yield return null;
            }

            targetObject.color = targetValue;
        }

        public static IEnumerator AnimateMaterialColor(Material targetObject, string propertyName, Color targetValue, float animTime = 1f)
        {
            var   startValue = targetObject.GetColor(propertyName);
            float timePassed = 0;

            while (timePassed <= animTime)
            {
                targetObject.SetColor(propertyName, Color.Lerp(startValue, targetValue, timePassed / animTime));

                timePassed += Time.deltaTime;

                yield return null;
            }

            targetObject.color = targetValue;
        }

        #endregion
    }
}