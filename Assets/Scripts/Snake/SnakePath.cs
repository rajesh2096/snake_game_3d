using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame3D.Snake
{
    /// <summary>
    /// Records the head's world-space position history along its trail.
    /// Allows body segments to query exact continuous positions behind the head.
    /// </summary>
    public class SnakePath
    {
        public struct PathPoint
        {
            public Vector3 Position;
            public Quaternion Rotation;

            public PathPoint(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }

        private readonly List<PathPoint> _points = new List<PathPoint>();
        private readonly float _minRecordDistance;

        public SnakePath(float minRecordDistance = 0.05f)
        {
            _minRecordDistance = minRecordDistance;
        }

        /// <summary>
        /// Clears all recorded points and seeds the initial point.
        /// </summary>
        public void Reset(Vector3 initialPosition, Quaternion initialRotation)
        {
            _points.Clear();
            _points.Add(new PathPoint(initialPosition, initialRotation));
        }

        /// <summary>
        /// Updates the head's latest position. Adds a new point when distance threshold is reached.
        /// </summary>
        public void UpdateHeadPosition(Vector3 headPosition, Quaternion headRotation, float maxRequiredDistance)
        {
            if (_points.Count == 0)
            {
                _points.Add(new PathPoint(headPosition, headRotation));
                return;
            }

            // If head moved far enough from last recorded point, add a new point
            float distFromLast = Vector3.Distance(headPosition, _points[0].Position);
            if (distFromLast >= _minRecordDistance)
            {
                _points.Insert(0, new PathPoint(headPosition, headRotation));
            }
            else
            {
                // Update the current leading point
                _points[0] = new PathPoint(headPosition, headRotation);
            }

            // Prune points that are well beyond the max required body length
            PruneOldPoints(maxRequiredDistance + 5.0f);
        }

        /// <summary>
        /// Samples an interpolated position and rotation along the recorded path at distanceBehindHead.
        /// </summary>
        public bool SamplePathAtDistance(float distanceBehindHead, out Vector3 position, out Quaternion rotation)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;

            if (_points.Count == 0)
            {
                return false;
            }

            if (_points.Count == 1 || distanceBehindHead <= 0f)
            {
                position = _points[0].Position;
                rotation = _points[0].Rotation;
                return true;
            }

            float accumulatedDist = 0f;

            for (int i = 0; i < _points.Count - 1; i++)
            {
                float segmentDist = Vector3.Distance(_points[i].Position, _points[i + 1].Position);

                if (accumulatedDist + segmentDist >= distanceBehindHead)
                {
                    float remainingDist = distanceBehindHead - accumulatedDist;
                    float t = segmentDist > 0.0001f ? remainingDist / segmentDist : 0f;

                    position = Vector3.Lerp(_points[i].Position, _points[i + 1].Position, t);
                    rotation = Quaternion.Slerp(_points[i].Rotation, _points[i + 1].Rotation, t);
                    return true;
                }

                accumulatedDist += segmentDist;
            }

            // If requested distance exceeds recorded history, fallback to the oldest recorded point
            position = _points[_points.Count - 1].Position;
            rotation = _points[_points.Count - 1].Rotation;
            return true;
        }

        private void PruneOldPoints(float maxHistoryDistance)
        {
            float totalDist = 0f;
            for (int i = 0; i < _points.Count - 1; i++)
            {
                totalDist += Vector3.Distance(_points[i].Position, _points[i + 1].Position);
                if (totalDist > maxHistoryDistance)
                {
                    // Remove all points past this index
                    int removeCount = _points.Count - (i + 2);
                    if (removeCount > 0)
                    {
                        _points.RemoveRange(i + 2, removeCount);
                    }
                    break;
                }
            }
        }
    }
}
