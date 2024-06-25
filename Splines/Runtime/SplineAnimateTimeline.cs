using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace ReiBrary.Splines
{
    public class SplineAnimateTimeline : SplineComponent
    {
        [SerializeField] private float _progress = 0f;
        public float Progress
        {
            get => _progress;
            set
            {
                _progress = value;

                UpdateTransform();
            }
        }

        /// <summary>
        /// Describes the different methods that can be used to animated an object along a spline.
        /// </summary>
        public enum Method
        {
            /// <summary> Spline will be traversed based on the given animation curve. </summary>
            Timeline
        }

        /// <summary>
        /// Describes the ways the object can be aligned when animating along the spline.
        /// </summary>
        public enum AlignmentMode
        {
            /// <summary> No aligment is done and object's rotation is unaffected. </summary>
            [InspectorName("None")]
            None,
            /// <summary> The object's forward and up axes align to the spline's tangent and up vectors. </summary>
            [InspectorName("Spline Element")]
            SplineElement,
            /// <summary> The object's forward and up axes align to the spline tranform's z-axis and y-axis. </summary>
            [InspectorName("Spline Object")]
            SplineObject,
            /// <summary> The object's forward and up axes align to to the world's z-axis and y-axis. </summary>
            [InspectorName("World Space")]
            World
        }

        [SerializeField, Tooltip("The method used to animate the GameObject along the spline.\n" +
                                 "Timeline - The spline's traversal is controller by the timeline.")]
        Method m_Method = Method.Timeline;

        [SerializeField, Tooltip("The target spline to follow.")]
        SplineContainer m_Target;

        [SerializeField, Tooltip("The coordinate space that the GameObject's up and forward axes align to.")]
        AlignmentMode m_AlignmentMode = AlignmentMode.SplineElement;

        [SerializeField, Tooltip("Which axis of the GameObject is treated as the forward axis.")]
        AlignAxis m_ObjectForwardAxis = AlignAxis.XAxis;

        [SerializeField, Tooltip("Which axis of the GameObject is treated as the up axis.")]
        AlignAxis m_ObjectUpAxis = AlignAxis.YAxis;

        [SerializeField]
        bool m_lockX = false;

        [SerializeField]
        bool m_lockY = false;

        [SerializeField]
        bool m_lockZ = false;

        [SerializeField, Tooltip("Normalized distance [0;1] offset along the spline at which the GameObject should be placed when the animation begins.")]
        float m_StartOffset;

        [NonSerialized]
        float m_StartOffsetT;

        [SerializeField, Tooltip("The period of time that it takes for the GameObject to complete its animation along the spline.")]
        float m_Duration = 1f;

        float m_SplineLength = -1;
        bool m_Playing;
        float m_NormalizedTime;
        float m_ElapsedTime;

        SplinePath<Spline> m_SplinePath;

        /// <summary>The target container of the splines to follow.</summary>
        [Obsolete("Use Container instead.", false)]
        public SplineContainer splineContainer => Container;
        /// <summary>The target container of the splines to follow.</summary>
        public SplineContainer Container
        {
            get => m_Target;
            set
            {
                m_Target = value;

                if (enabled && m_Target != null && m_Target.Splines != null)
                {
                    for (int i = 0; i < m_Target.Splines.Count; i++)
                        OnSplineChange(m_Target.Splines[i], -1, SplineModification.Default);
                }
            }
        }

        /// <summary> Object space axis that should be considered as the object's forward vector. </summary>
        [Obsolete("Use ObjectForwardAxis instead.", false)]
        public AlignAxis objectForwardAxis => ObjectForwardAxis;
        /// <summary> Object space axis that should be considered as the object's forward vector. </summary>
        public AlignAxis ObjectForwardAxis
        {
            get => m_ObjectForwardAxis;
            set => m_ObjectUpAxis = SetObjectAlignAxis(value, ref m_ObjectForwardAxis, m_ObjectUpAxis);
        }

        /// <summary> Object space axis that should be considered as the object's up vector. </summary>
        [Obsolete("Use ObjectUpAxis instead.", false)]
        public AlignAxis objectUpAxis => ObjectUpAxis;
        /// <summary> Object space axis that should be considered as the object's up vector. </summary>
        public AlignAxis ObjectUpAxis
        {
            get => m_ObjectUpAxis;
            set => m_ObjectForwardAxis = SetObjectAlignAxis(value, ref m_ObjectUpAxis, m_ObjectForwardAxis);
        }


        /// <summary>
        /// Normalized time of the Spline's traversal. The integer part is the number of times the Spline has been traversed.
        /// The fractional part is the % (0-1) of progress in the current loop.
        /// </summary>
        [Obsolete("Use NormalizedTime instead.", false)]
        public float normalizedTime => NormalizedTime;
        /// <summary>
        /// Normalized time of the Spline's traversal. The integer part is the number of times the Spline has been traversed.
        /// The fractional part is the % (0-1) of progress in the current loop.
        /// </summary>
        public float NormalizedTime
        {
            get => m_NormalizedTime;
            set
            {
                m_NormalizedTime = value;
                m_ElapsedTime = m_Duration * m_NormalizedTime;

                UpdateTransform();
            }
        }

        /// <summary> Total time (in seconds) since the start of Spline's traversal. </summary>
        [Obsolete("Use ElapsedTime instead.", false)]
        public float elapsedTime => ElapsedTime;
        /// <summary> Total time (in seconds) since the start of Spline's traversal. </summary>
        public float ElapsedTime
        {
            get => m_ElapsedTime;
            set
            {
                m_ElapsedTime = value;
                CalculateNormalizedTime(0f);
                UpdateTransform();
            }
        }

        /// <summary> Normalized distance [0;1] offset along the spline at which the object should be placed when the animation begins. </summary>
        public float StartOffset
        {
            get => m_StartOffset;
            set
            {
                if (m_SplineLength < 0f)
                    RebuildSplinePath();

                m_StartOffset = Mathf.Clamp01(value);
                UpdateStartOffsetT();
            }
        }

        internal float StartOffsetT => m_StartOffsetT;

        /// <summary> Returns true if object is currently animating along the Spline. </summary>
        [Obsolete("Use IsPlaying instead.", false)]
        public bool isPlaying => IsPlaying;
        /// <summary> Returns true if object is currently animating along the Spline. </summary>
        public bool IsPlaying => m_Playing;

        /// <summary> Invoked each time object's animation along the Spline is updated.</summary>
        [Obsolete("Use Updated instead.", false)]
        public event Action<Vector3, Quaternion> onUpdated;
        /// <summary> Invoked each time object's animation along the Spline is updated.</summary>
        public event Action<Vector3, Quaternion> Updated;

        private bool m_EndReached = false;
        /// <summary> Invoked every time the object's animation reaches the end of the Spline.
        /// In case the animation loops, this event is called at the end of each loop.</summary>
        public event Action Completed;

        void OnEnable()
        {
            RebuildSplinePath();

            Spline.Changed += OnSplineChange;
        }

        void OnDisable()
        {
            Spline.Changed -= OnSplineChange;
        }

        void OnValidate()
        {
            RebuildSplinePath();
        }

        bool IsNullOrEmptyContainer()
        {
            if (m_Target == null || m_Target.Splines.Count == 0)
            {
                if (Application.isPlaying)
                    Debug.LogError("SplineAnimate does not have a valid SplineContainer set.", this);
                return true;
            }

            return false;
        }

        /// <summary> Begin animating object along the Spline. </summary>
        public void Play()
        {
            if (IsNullOrEmptyContainer())
                return;

            m_Playing = true;
        }

        /// <summary> Pause object's animation along the Spline. </summary>
        public void Pause()
        {
            m_Playing = false;
        }

        /// <summary> Stop the animation and place the object at the beginning of the Spline. </summary>
        /// <param name="autoplay"> If true, the animation along the Spline will start over again. </param>
        public void Restart(bool autoplay)
        {
            if (IsNullOrEmptyContainer())
                return;

            Progress = 0f;
        }

        void CalculateNormalizedTime(float deltaTime)
        {
            var previousElapsedTime = m_ElapsedTime;
            m_ElapsedTime += deltaTime;
            var currentDuration = m_Duration;

            var t = Mathf.Min(m_ElapsedTime, currentDuration) / currentDuration;

            // forcing reset to 0 if the m_NormalizedTime reach the end of the spline previously (1).
            m_NormalizedTime = t == 0 ? 0f : Mathf.Floor(m_NormalizedTime) + t;
            if (m_NormalizedTime >= 1f)
            {
                m_EndReached = true;
                m_Playing = false;
            }
        }

        void UpdateEndReached(float previousTime, float currentDuration)
        {
            m_EndReached = Mathf.FloorToInt(previousTime/currentDuration) < Mathf.FloorToInt(m_ElapsedTime/currentDuration);
        }

        void UpdateStartOffsetT()
        {
            if (m_SplinePath != null)
                m_StartOffsetT = m_SplinePath.ConvertIndexUnit(m_StartOffset * m_SplineLength, PathIndexUnit.Distance, PathIndexUnit.Normalized);
        }

        void UpdateTransform()
        {
            if (m_Target == null)
                return;

            EvaluatePositionAndRotation(out var position, out var rotation);

            transform.position = position;
            if (m_AlignmentMode != AlignmentMode.None)
            {
                var oldEuler = transform.rotation.eulerAngles;
                var newEuler = rotation.eulerAngles;

                var finalRotation = new Vector3(
                    m_lockX ? oldEuler.x : newEuler.x,
                    m_lockY ? oldEuler.y : newEuler.y,
                    m_lockZ ? oldEuler.z : newEuler.z);

                transform.rotation = Quaternion.Euler(finalRotation);
            }

            onUpdated?.Invoke(position, rotation);
            Updated?.Invoke(position, rotation);

            if (m_EndReached)
            {
                m_EndReached = false;
                Completed?.Invoke();
            }
        }

        void EvaluatePositionAndRotation(out Vector3 position, out Quaternion rotation)
        {
            var t = _progress;

            position = m_Target.EvaluatePosition(m_SplinePath, t);
            rotation = Quaternion.identity;

            // Correct forward and up vectors based on axis remapping parameters
            var remappedForward = GetAxis(m_ObjectForwardAxis);
            var remappedUp = GetAxis(m_ObjectUpAxis);
            var axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));

            if (m_AlignmentMode != AlignmentMode.None)
            {
                var forward = Vector3.forward;
                var up = Vector3.up;

                switch (m_AlignmentMode)
                {
                    case AlignmentMode.SplineElement:
                        forward = m_Target.EvaluateTangent(m_SplinePath, t);
                        if (Vector3.Magnitude(forward) <= Mathf.Epsilon)
                        {
                            if (t < 1f)
                                forward = m_Target.EvaluateTangent(m_SplinePath, Mathf.Min(1f, t + 0.01f));
                            else
                                forward = m_Target.EvaluateTangent(m_SplinePath, t - 0.01f);
                        }
                        forward.Normalize();
                        up = m_Target.EvaluateUpVector(m_SplinePath, t);
                        break;

                    case AlignmentMode.SplineObject:
                        var objectRotation = m_Target.transform.rotation;
                        forward = objectRotation * forward;
                        up = objectRotation * up;
                        break;

                    default:
                        Debug.Log($"{m_AlignmentMode} animation alignment mode is not supported!", this);
                        break;
                }

                rotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;
            }
            else
            {
                rotation = transform.rotation;
            }
        }

        public void RebuildSplinePath()
        {
            if (m_Target != null)
            {
                m_SplinePath = new SplinePath<Spline>(m_Target.Splines);
                m_SplineLength = m_SplinePath != null ? m_SplinePath.GetLength() : 0f;
            }
        }

        AlignAxis SetObjectAlignAxis(AlignAxis newValue, ref AlignAxis targetAxis, AlignAxis otherAxis)
        {
            // Swap axes if the new value matches that of the other axis
            if (newValue == otherAxis)
            {
                otherAxis = targetAxis;
                targetAxis = newValue;
            }
            // Do not allow configuring object's forward and up axes as opposite
            else if ((int)newValue % 3 != (int)otherAxis % 3)
                targetAxis = newValue;

            return otherAxis;
        }

        void OnSplineChange(Spline spline, int knotIndex, SplineModification modificationType)
        {
            RebuildSplinePath();
        }

        internal float GetLoopInterpolation(bool offset)
        {
            var t = 0f;
            var normalizedTimeWithOffset = NormalizedTime + (offset ? m_StartOffsetT : 0f);
            if (Mathf.Floor(normalizedTimeWithOffset) == normalizedTimeWithOffset)
                t = Mathf.Clamp01(normalizedTimeWithOffset);
            else
                t = normalizedTimeWithOffset % 1f;

            return t;
        }
    }
}
