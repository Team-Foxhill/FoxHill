using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnTester : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private float _range;
    [SerializeField] private float _waitTime;
    [SerializeField] private int _endIndex;
    private WaitForSecondsRealtime _interval;
    private float _positionX;
    private float _positionY;
    private int _index = 0;

    private void Awake()
    {
        _interval = new WaitForSecondsRealtime(_waitTime);
        StartCoroutine(SpawnObstacle());
    }

    private IEnumerator SpawnObstacle()
    {
        while (_index < _endIndex)
        {
            
            _positionX = transform.position.x + Random.Range(-_range, _range);
            _positionY = transform.position.y + Random.Range(-_range, _range);
            GameObject newOne = Instantiate(_gameObject);
            newOne.transform.position = new Vector2(_positionX, _positionY);
            newOne.name = $"Obstacle_{_index}";
            _index++;

            yield return _interval;
        }

    }
}
