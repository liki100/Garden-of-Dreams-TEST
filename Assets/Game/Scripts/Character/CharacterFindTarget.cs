using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterInputController), typeof(CharacterRotate))]
public class CharacterFindTarget : MonoBehaviour
{
    [SerializeField] private LayerMask _searchLayerMask;
    [SerializeField] private Transform _overlapStartPoint;
    [SerializeField, Min(0)] private float _circleRadius = 1f;

    private readonly Collider2D[] _overlapResults = new Collider2D[32];

    private CharacterInputController _controller;
    private CharacterRotate _rotate;
    
    private void Start()
    {
        _controller = GetComponent<CharacterInputController>();
        _rotate = GetComponent<CharacterRotate>();
    }

    private void Update()
    {
        var nearestTarget = FindTarget();

        if (nearestTarget != null)
        {
            var direction = nearestTarget.transform.position - _overlapStartPoint.position;

            Debug.Log(nearestTarget.name);
            
            _rotate.ToTarget(direction);
        }
        else
        {
            if (_controller.GetDirection() == Vector3.zero)
                return;
            
            _rotate.ToTarget(_controller.GetDirection());
        }
    }

    private Collider2D FindTarget()
    {
        var hitCount = Physics2D.OverlapCircleNonAlloc(_overlapStartPoint.position, _circleRadius, _overlapResults, _searchLayerMask);
        if (hitCount == 0)
            return null;

        Collider2D nearest = null;
        var minDistanceSquared = Mathf.Infinity;

        for (var i = 0; i < hitCount; i++)
        {
            if (_overlapResults[i].TryGetComponent(out IDamageable damageable) == false)
                continue;
            
            var distanceSquared = ((Vector2)transform.position - (Vector2)_overlapResults[i].transform.position).sqrMagnitude;
            if (distanceSquared < minDistanceSquared)
            {
                minDistanceSquared = distanceSquared;
                nearest = _overlapResults[i];
            }
        }

        return nearest;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(_overlapStartPoint.position, _circleRadius);
    }
}