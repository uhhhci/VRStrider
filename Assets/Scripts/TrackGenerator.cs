using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    public List<GameObject> _curves;
    private List<GameObject> _partsToUse;
    private GameObject _lastPart;
    private int _repetitions = 6;
    public List<GameObject> _path;

    // Start is called before the first frame update
    void Start()
    {
        _partsToUse = new List<GameObject>();
        _path = new List<GameObject>();
        _lastPart = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ClearPath();
            GeneratePath();
        }
    }

    private void ClearPath()
    {
        for (int i = 0; i < _path.Count; i++)
        {
            Destroy(_path[i]);
        }
        _path.Clear();
        _lastPart = this.gameObject;
    }

    public void GeneratePath()
    {
        for (int i = 0; i < _curves.Count; i++)
        {
            for (int r = 0; r < _repetitions; r++)
            {
                _partsToUse.Add(_curves[i]);
            }
        }

        while (_partsToUse.Count > 0)
        {
            int index = Random.Range(0, _partsToUse.Count);
            GameObject curve = _partsToUse[index];
            _partsToUse.RemoveAt(index);

            Transform lastAttachmentPoint = _lastPart.transform.GetChild(0).transform;

            GameObject go = Instantiate(curve);
            go.transform.position = lastAttachmentPoint.position;
            go.transform.rotation = lastAttachmentPoint.rotation;
            _path.Add(go);

            if (Random.Range(0, 1f) > 0.5f)
            {
                Vector3 scale = go.transform.localScale;
                scale.x *= -1;
                go.transform.localScale = scale;
            }
            go.transform.SetParent(transform);
            _lastPart = go;
        }
    }
}
