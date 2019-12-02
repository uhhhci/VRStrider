using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringLaw : MonoBehaviour
{

    public TrackGenerator _trackGenerator;
    private List<Collider> _pathColliders;
    public Transform _player;
    public float _distanceToPerfectPath;

    // Start is called before the first frame update
    void Start()
    {
        _trackGenerator.GeneratePath();
        GenerateColliderList();
    }

    private void GenerateColliderList()
    {
        _pathColliders = new List<Collider>();
        List<GameObject> path = _trackGenerator._path;

        foreach (GameObject go in path)
        {
            Transform centerPath = go.transform.GetChild(1);
            Collider[] cols = centerPath.GetComponentsInChildren<Collider>();
            _pathColliders.AddRange(cols);
        }
        print(_pathColliders.Count);
    }

    private void CalculateDistanceToPerfectPath()
    {
        float closestPoint = 1000;
        foreach (Collider c in _pathColliders)
        {
            Vector3 closestPointOnCollider = c.ClosestPoint(_player.position);
            float distance = (_player.position - closestPointOnCollider).magnitude;
            if (distance < closestPoint)
            {
                closestPoint = distance;
            }
        }
        _distanceToPerfectPath = closestPoint;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDistanceToPerfectPath();
    }
}
