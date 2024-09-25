using FoxHill.Core.Test;
using FoxHill.Player.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Serializable]
    public class IndexedPrefab
    {
        public int index; // 타워 프리팹이 가지고 있는 인덱스.
        public GameObject prefab; // 타워 프리팹
    }
    [SerializeField] private List<IndexedPrefab> indexedPrefabs = new List<IndexedPrefab>(); // 원래 의도 => IndexedPrefab 클래스를 이용해서 인덱스와 프리팹을 함께 저장, 인덱스에 접근하여 프리팹 인스턴시에이트.
    [SerializeField] private PlayerInventory _playerInventory; // 생성 취소하면 아이템을 다시 생성해줄 수 있게 PlayerInventory를 들고 있게 하려고 했음.
    [SerializeField] private SpriteRenderer _spriteRenderer; // 같은 게임오브젝트의 스프라이트 렌더러를 통해 미리보기 스프라이트를 설정해주려 함.

    private Sprite[] _sprites; // 사용하지 않으려고 변경중이었음. => Line41 IndexedPrefab의 프리팹에서 스프라이트를 GetComponent 하도록 수정하려고 생각했음.
    private int _arrayIndex; // 사용하지 않으려고 변경중이었음. => Line41
    private Vector3 _spawnPosition; // 이 게임오브젝트의 트랜스폼.

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void PrefabIndexHandler(int i)
    {
    }


    /// <summary>
    /// 프리팹이 생성되기 전, 미리보기로 화면에 호버링할 스프라이트.
    /// </summary>
    /// <param name="isRender"></param>
    public void ToggleSprite(bool isRender)
    {
        _spriteRenderer.enabled = isRender;
        _spriteRenderer.sprite = _sprites[_arrayIndex]; // IndexedPrefab의 프리팹에서 스프라이트를 GetComponent 하도록 수정하려고 생각했음.
        transform.localPosition = Vector3.zero;
    }

    public void ConfirmSpawn()
    {
        transform.position = _spawnPosition;
        //var newPrefab = Instantiate(_spawnPrefabs[_arrayIndex], _spawnPosition, Quaternion.identity);
        //newPrefab.name = _spawnPrefabs[_arrayIndex].name;
    }

    /// <summary>
    /// 입력 값을 받아 이동하도록 해주는 메서드.
    /// </summary>
    /// <param name="input">방향키가 입력되면 플레이어 매니저에서 알아서 전달해주는 값.</param>
    public void OnMovePrefabSpawn(Vector2 input)
    {
        _spawnPosition = input;
        transform.position += _spawnPosition;
    }
}
