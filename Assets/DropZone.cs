using TMPro;
using UnityEngine;

/// <summary>
/// Класс, отвечающий за обработку попадания шаров в зону.
/// </summary>
public class DropZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [Tooltip("Индекс зоны (0, 1 или 2).")]
    [SerializeField] private int zoneIndex;

    [Tooltip("Объект зоны, к которому привязан коллайдер.")]
    [SerializeField] private Transform zoneTransform;

    private TowerManager _towerManager;
    private Match3Logic _match3Logic;

    private void Start()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Инициализирует необходимые компоненты.
    /// </summary>
    private void InitializeComponents()
    {
        if (_towerManager == null)
        {
            _towerManager = FindObjectOfType<TowerManager>();
            if (_towerManager == null)
            {
                Debug.LogError("TowerManager не найден на сцене.");
            }
        }

        if (_match3Logic == null)
        {
            _match3Logic = FindObjectOfType<Match3Logic>();
            if (_match3Logic == null)
            {
                Debug.LogError("Match3Logic не найден на сцене.");
            }
        }

        if (zoneTransform == null)
        {
            zoneTransform = transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Circle"))
        {
            HandleBallCollision(other.gameObject);
        }
    }

    /// <summary>
    /// Обрабатывает столкновение шара с зоной.
    /// </summary>
    /// <param name="circle">Объект шара, который попал в зону.</param>
    private void HandleBallCollision(GameObject circle)
    {
        if (_towerManager == null || _match3Logic == null)
        {
            Debug.LogError("Необходимые компоненты не инициализированы.");
            return;
        }

        if (_towerManager.TryAddCircleToTower(zoneIndex, circle, zoneTransform.position))
        {
            int height = _towerManager.GetHeight(zoneIndex) - 1;
            _match3Logic.AddCircleToGrid(zoneIndex, height, circle);
        }
        else
        {
            Debug.Log("Башня заполнена! Шар не добавлен.");
        }
    }

    /// <summary>
    /// Обновляет текстовый объект зоны.
    /// </summary>

}