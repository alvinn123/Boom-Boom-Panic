using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private float _spawnTimer = 3f;

    private ColorChanger _colorChanger;

    private void Awake()
    {
        _colorChanger = GetComponent<ColorChanger>();
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            _colorChanger.SetRandomColor();
            Enemy enemy = Instantiate(_enemyPrefab, transform.position, transform.rotation);
            enemy.Init(_colorChanger.DefaultColor);
            yield return new WaitForSeconds(_spawnTimer);
        }
    }
}
