using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace ReiBrary.Extensions
{
    public static class Extensions
    {
        #region Transform

        public static Transform GetChildTransform(this Transform transform, string name)
        {
            foreach(Transform t in transform)
                if (t.name == name)
                    return t;
            return null;
        }
        
        public static Transform GetChildTransform(this GameObject gameObject, string name)
        {
            return GetChildTransform(gameObject.transform, name);
        }

        public static void Enable(this Transform transform) => transform.gameObject.Enable();
        public static void Disable(this Transform transform) => transform.gameObject.Disable();

        #endregion
        
        #region Monobehaviour

        public static void Invoke(this MonoBehaviour monoBehaviour, Action action, float delay)
        {
            monoBehaviour.StartCoroutine(InvokeRoutine(action, delay));
        }

        private static IEnumerator InvokeRoutine(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        public static void InvokeWhen(this MonoBehaviour monoBehaviour, Action action, Func<bool> when, float checkDelay = 0)
        {
            monoBehaviour.StartCoroutine(InvokeRoutineWhen(action, when, checkDelay));
        }
        
        private static IEnumerator InvokeRoutineWhen(Action invoke, Func<bool> when, float checkDelay)
        {
            while (true)
            {
                if (when())
                {
                    invoke();
                    yield break;
                }
                yield return new WaitForSeconds(checkDelay);
            }
        }
        
        #endregion

        #region Object
        
        public static T GetComponent<T>(this Object obj) where T : class
        {
            var gameObject = obj as GameObject;
            return gameObject != null ? gameObject.GetComponent<T>() : null;
        }
        
        #endregion
        
        #region GameObject

        public static T GetComponent<T>(this GameObject gameObject, bool getFromHierarchyIfNull)
        { 
            return gameObject.GetComponent<T>() ?? gameObject.GetComponentInParent<T>() ?? gameObject.GetComponentInChildren<T>();
        }
        
        public static void Enable(this GameObject gameObject) => gameObject.SetActive(true);
        public static void Disable(this GameObject gameObject) => gameObject.SetActive(false);
        public static bool IsEnabled(this GameObject gameObject) => gameObject.activeInHierarchy;

        public static GameObject GetChildObject(this Transform transform, string name)
        {
            foreach(Transform t in transform)
                if (t.name == name)
                    return t.gameObject;
            return null;
        }
        
        public static GameObject GetChildObject(this GameObject gameObject, string name)
        {
            return GetChildObject(gameObject.transform, name);
        }
        
        public static IEnumerable<Transform> GetChildTransforms(this GameObject obj)
        {
            return obj.transform.GetChildTransforms();
        }
        
        public static IEnumerable<Transform> GetChildTransforms(this Transform transform)
        {
            return transform.GetComponentsInChildren<Transform>().Where(t => t != transform);
        }

        #endregion

        #region Vector2

        public static Vector2 GetRotated(this Vector2 vector, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
            
            return new Vector2(cos*vector.x - sin*vector.y, sin*vector.x + cos*vector.y);
        }
        
        public static Vector2 GetRotated(this Vector3 vector, float degrees)
        {
            return GetRotated((Vector2)vector, degrees);
        }

        public static float ToDegrees(this Vector3 vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }
        
        public static float ToDegrees(this Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }

        public static Vector2 ToVector(this float angle)
        {
            double radians = angle * Mathf.Deg2Rad;
            return new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));
        }

        #endregion

        #region Vector3
        
        public static Vector3 GetXZ(this Vector3 v) => new(v.x, 0, v.z);
        public static Vector3 SetXZ(this Vector3 v, float x, float z) => new(x, v.y, z);

        public static Vector3 SetX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
        public static Vector3 SetY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
        public static Vector3 SetZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

        public static Vector3 Abs(this Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

        #endregion

        #region RectTransform

        public static float GetWidth(this RectTransform rt) =>
            rt.rect.width;

        public static void SetWidth(this RectTransform rt, float w) =>
            // rt.sizeDelta = new Vector2(w, rt.sizeDelta.y);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);

        public static float GetHeight(this RectTransform rt) =>
            rt.rect.height;

        public static void SetHeight(this RectTransform rt, float h) =>
            // rt.sizeDelta = new Vector2(rt.sizeDelta.x, h);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);

        public static float GetLeft(this RectTransform rt) =>
            rt.offsetMin.x;

        public static void SetLeft(this RectTransform rt, float left) =>
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);

        public static float GetRight(this RectTransform rt) =>
            -rt.offsetMax.x;

        public static void SetRight(this RectTransform rt, float right) =>
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);

        public static float GetTop(this RectTransform rt) =>
            -rt.offsetMax.y;

        public static void SetTop(this RectTransform rt, float top) =>
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        
        public static float GetBottom(this RectTransform rt) =>
            rt.offsetMin.y;

        public static void SetBottom(this RectTransform rt, float bottom) =>
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);

        public static Vector2 GetCentre(this RectTransform rt) =>
            rt.pivot * new Vector2(rt.rect.width, -rt.rect.height);

        public static Vector3 TransformPointTo(this RectTransform from, Vector3 point, RectTransform to) =>
            to.InverseTransformPoint(from.TransformPoint(point));

        #endregion

        #region String

        public static string RemoveWhitespace(this string str) =>
            string.Concat(str.Where(c => !char.IsWhiteSpace(c)));

        public static bool IsNullOrEmpty([CanBeNull] this string str) => string.IsNullOrEmpty(str);

        #endregion

        #region float

        public static bool IsZero(this float x, double precision = 0.000001) => Mathf.Abs(x) <= precision;

        #endregion

        #region Array

        public static int GetWrappedIndex<T>(this T[] array, int index)
        {
            return (index % array.Length + array.Length) % array.Length; ;
        }

        public static T GetWrappedElement<T>(this T[] array, int index)
        {
            return array[array.GetWrappedIndex(index)];
        }

        #endregion

        #region IEnumerable

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null) return true;

            var collection = enumerable as ICollection<T>;
            if (collection != null)
                return collection.Count < 1;

            return !enumerable.Any();
        }

        public static T GetRandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ElementAtOrDefault(UnityEngine.Random.Range(0, enumerable.Count()));
        }

        #endregion

        #region Animator

        public static bool HasParam(this Animator animator, string name) => animator.parameters.Any(x => x.name == name);

        public static void TrySetBool(this Animator animator, string name, bool value)
        {
            if (animator.HasParam(name)) animator.SetBool(name, value);
        }

        public static void TrySetFloat(this Animator animator, string name, float value)
        {
            if (animator.HasParam(name)) animator.SetFloat(name, value);
        }

        public static void TrySetTrigger(this Animator animator, string name)
        {
            if (animator.HasParam(name)) animator.SetTrigger(name);
        }

        #endregion

        #region BoxCollider

        public static Vector3 GetHalfExtents(this BoxCollider collider) => collider.size / 2;

        #endregion

        #region NavMeshPath

        /// <summary>
        /// Calculates the total distance along the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static float GetPathLength(this NavMeshPath path)
        {
            var dist = 0f;
            for (var i = 1; i < path.corners.Length; i++)
                dist += Vector3.Distance(path.corners[i - 1], path.corners[i]);

            return dist;
        }

        #endregion
    }
}