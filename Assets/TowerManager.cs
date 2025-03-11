using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Класс, отвечающий за управление башнями в зонах.
/// </summary>
public class TowerManager : MonoBehaviour
{
    [Header("Tower Settings")]
    [Tooltip("Радиус круга. Используется для расчета высоты башни.")]
    [SerializeField] private float circleRadius = 1f;

    [Tooltip("Максимальная высота башни.")]
    [Range(1, 5)]
    [SerializeField] private int maxHeight = 3;

    [Header("Zone Texts")]
    [Tooltip("Текстовые объекты для отображения количества мячей в зонах.")]
    [SerializeField] private TextMeshProUGUI[] zoneTexts;

    private int[] _towerHeights = new int[3]; // Высота башни в каждой зоне
    public List<GameObject>[] _towerCircles; // Массив списков для хранения шаров каждой башни

    private void Start()
    {
        InitializeTowerCircles(); // Инициализируем списки для каждой башни
        ValidateZoneTexts();
        UpdateZoneTexts();
    }

    /// <summary>
    /// Инициализирует списки для хранения шаров каждой башни.
    /// </summary>
    private void InitializeTowerCircles()
    {
        _towerCircles = new List<GameObject>[3];
        for (int i = 0; i < _towerCircles.Length; i++)
        {
            _towerCircles[i] = new List<GameObject>();
        }
    }

    /// <summary>
    /// Добавляет круг в башню и возвращает true, если успешно.
    /// </summary>
    /// <param name="zoneIndex">Индекс зоны.</param>
    /// <param name="circle">Объект круга.</param>
    /// <param name="zonePosition">Позиция зоны.</param>
    /// <returns>True, если круг успешно добавлен, иначе false.</returns>
    public bool TryAddCircleToTower(int zoneIndex, GameObject circle, Vector2 zonePosition)
    {
        if (!IsValidZoneIndex(zoneIndex))
        {
            Debug.LogError("Некорректный индекс зоны.");
            return false;
        }

        if (IsTowerFull(zoneIndex))
        {
            Debug.LogWarning("Башня достигла максимальной высоты!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return false;
        }

        Vector2 targetPosition = CalculateTargetPosition(zoneIndex, zonePosition);
        StartCoroutine(MoveCircleToPosition(circle, targetPosition));

        _towerHeights[zoneIndex]++;
        Debug.Log(GetCirclesForTower(zoneIndex).Count);
        UpdateZoneTexts();

        // Добавляем шар в список соответствующей башни
        _towerCircles[zoneIndex].Add(circle);

        return true;
    }

    /// <summary>
    /// Возвращает текущую высоту башни в указанной зоне.
    /// </summary>
    /// <param name="zoneIndex">Индекс зоны.</param>
    /// <returns>Высота башни или -1, если индекс некорректен.</returns>
    public int GetHeight(int zoneIndex)
    {
        if (!IsValidZoneIndex(zoneIndex))
        {
            Debug.LogError("Некорректный индекс зоны.");
            return -1;
        }

        return _towerHeights[zoneIndex];
    }

    /// <summary>
    /// Сбрасывает высоту всех башен.
    /// </summary>
    [ContextMenu("Reset Towers")]
    public void ResetTowers()
    {
        for (int i = 0; i < _towerHeights.Length; i++)
        {
            _towerHeights[i] = 0;
        }
        UpdateZoneTexts();
        ClearAllCircles();
        Debug.Log("Башни сброшены.");
    }

    /// <summary>
    /// Плавно перемещает шар к целевой позиции.
    /// </summary>
    /// <param name="circle">Объект круга.</param>
    /// <param name="targetPosition">Целевая позиция.</param>
    private IEnumerator MoveCircleToPosition(GameObject circle, Vector2 targetPosition)
    {
        Rigidbody2D circleRb = circle.GetComponent<Rigidbody2D>();
        circleRb.isKinematic = true; // Отключаем физику на время перемещения

        float duration = 0.5f; // Длительность перемещения
        float elapsed = 0f;
        Vector2 startPosition = circle.transform.position;

        while (elapsed < duration)
        {
            circle.transform.position = Vector2.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        circle.transform.position = targetPosition; // Убедимся, что шар точно на месте
        circleRb.isKinematic = false; // Включаем физику обратно
    }

    /// <summary>
    /// Обновляет текстовые объекты для отображения количества мячей в зонах.
    /// </summary>
    private void UpdateZoneTexts()
    {
        for (int i = 0; i < zoneTexts.Length; i++)
        {
            if (zoneTexts[i] != null)
            {
                zoneTexts[i].text = $"Zone {i + 1}: {_towerHeights[i]}";
            }
        }
    }

    /// <summary>
    /// Проверяет, заполнена ли башня в указанной зоне.
    /// </summary>
    /// <param name="zoneIndex">Индекс зоны.</param>
    /// <returns>True, если башня заполнена, иначе false.</returns>
    private bool IsTowerFull(int zoneIndex)
    {
        return _towerHeights[zoneIndex] >= maxHeight;
    }

    /// <summary>
    /// Проверяет, является ли индекс зоны корректным.
    /// </summary>
    /// <param name="zoneIndex">Индекс зоны.</param>
    /// <returns>True, если индекс корректен, иначе false.</returns>
    private bool IsValidZoneIndex(int zoneIndex)
    {
        return zoneIndex >= 0 && zoneIndex < _towerHeights.Length;
    }

    /// <summary>
    /// Рассчитывает целевую позицию для круга.
    /// </summary>
    /// <param name="zoneIndex">Индекс зоны.</param>
    /// <param name="zonePosition">Позиция зоны.</param>
    /// <returns>Целевая позиция для круга.</returns>
    private Vector2 CalculateTargetPosition(int zoneIndex, Vector2 zonePosition)
    {
        return new Vector2(zonePosition.x, zonePosition.y + circleRadius * _towerHeights[zoneIndex]);
    }

    /// <summary>
    /// Проверяет, назначены ли текстовые объекты для зон.
    /// </summary>
    private void ValidateZoneTexts()
    {
        if (zoneTexts == null || zoneTexts.Length == 0)
        {
            Debug.LogWarning("Текстовые объекты для зон не назначены.");
        }
    }
    [ContextMenu("Get Circles For Tower")]
    /// <summary>
    /// Возвращает список шаров для указанной башни.
    /// </summary>
    /// <param name="zoneIndex">Индекс зоны.</param>
    /// <returns>Список шаров или null, если индекс некорректен.</returns>
    public List<GameObject> GetCirclesForTower(int zoneIndex)
    {
        if (!IsValidZoneIndex(zoneIndex))
        {
            Debug.LogError("Некорректный индекс зоны.");
            return null;
        }

        return _towerCircles[zoneIndex];
    }

    /// <summary>
    /// Очищает все списки шаров.
    /// </summary>
    public void ClearAllCircles()
    {
        for (int i = 0; i < _towerCircles.Length; i++)
        {
            foreach (var circle in _towerCircles[i])
            {
                if (circle != null)
                {
                    Destroy(circle);
                }
            }
            _towerCircles[i].Clear();
        }
    }
}