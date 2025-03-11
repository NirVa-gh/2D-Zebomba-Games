using UnityEngine;

/// <summary>
/// Класс, отвечающий за логику совпадения и уничтожения кругов.
/// </summary>
public class Match3Logic : MonoBehaviour
{
    [Header("Match Settings")]
    [Tooltip("Эффект разрушения при совпадении цветов.")]
    [SerializeField] private ParticleSystem destructionEffect;

    private GameObject[,] _grid = new GameObject[3, 3]; // Сетка 3x3 для хранения кругов

    /// <summary>
    /// Добавляет круг в сетку и проверяет совпадения.
    /// </summary>
    /// <param name="zoneIndex">Индекс зоны (горизонтальная координата).</param>
    /// <param name="height">Высота (вертикальная координата).</param>
    /// <param name="circle">Объект круга.</param>
    public void AddCircleToGrid(int zoneIndex, int height, GameObject circle)
    {
        if (!IsValidCoordinate(zoneIndex, height))
        {
            Debug.LogError("Некорректные координаты для добавления круга в сетку.");
            return;
        }

        _grid[zoneIndex, height] = circle;
        CheckForMatches();
    }

    /// <summary>
    /// Проверяет все возможные линии на совпадение цветов.
    /// </summary>
    private void CheckForMatches()
    {
        bool[,] matched = new bool[3, 3]; // Массив для отметки совпадающих кругов

        // Проверка горизонтальных линий
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (CheckLine(x, y, 1, 0))
                {
                    MarkLine(matched, x, y, 1, 0);
                }
            }
        }

        // Проверка вертикальных линий
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (CheckLine(x, y, 0, 1))
                {
                    MarkLine(matched, x, y, 0, 1);
                }
            }
        }

        // Проверка диагональных линий (вправо-вверх)
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (CheckLine(x, y, 1, 1))
                {
                    MarkLine(matched, x, y, 1, 1);
                }
            }
        }

        // Проверка диагональных линий (вправо-вниз)
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (CheckLine(x, y, 1, -1))
                {
                    MarkLine(matched, x, y, 1, -1);
                }
            }
        }

        // Уничтожение отмеченных кругов
        DestroyMatchedCircles(matched);
    }

    /// <summary>
    /// Проверяет линию на совпадение цветов.
    /// </summary>
    /// <param name="x">Начальная координата X.</param>
    /// <param name="y">Начальная координата Y.</param>
    /// <param name="dx">Шаг по оси X.</param>
    /// <param name="dy">Шаг по оси Y.</param>
    /// <returns>True, если линия совпадает, иначе false.</returns>
    private bool CheckLine(int x, int y, int dx, int dy)
    {
        GameObject startCircle = _grid[x, y];
        if (startCircle == null) return false;

        Color startColor = startCircle.GetComponent<SpriteRenderer>().color;

        for (int i = 1; i < 3; i++)
        {
            int newX = x + dx * i;
            int newY = y + dy * i;

            if (!IsValidCoordinate(newX, newY))
                return false;

            GameObject nextCircle = _grid[newX, newY];
            if (nextCircle == null || nextCircle.GetComponent<SpriteRenderer>().color != startColor)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Отмечает линию из трех совпадающих кругов.
    /// </summary>
    /// <param name="matched">Массив для отметки совпадающих кругов.</param>
    /// <param name="x">Начальная координата X.</param>
    /// <param name="y">Начальная координата Y.</param>
    /// <param name="dx">Шаг по оси X.</param>
    /// <param name="dy">Шаг по оси Y.</param>
    private void MarkLine(bool[,] matched, int x, int y, int dx, int dy)
    {
        for (int i = 0; i < 3; i++)
        {
            int newX = x + dx * i;
            int newY = y + dy * i;

            if (IsValidCoordinate(newX, newY))
            {
                matched[newX, newY] = true;
            }
        }
    }

    /// <summary>
    /// Уничтожает отмеченные круги и обновляет сетку.
    /// </summary>
    /// <param name="matched">Массив с отмеченными кругами.</param>
    private void DestroyMatchedCircles(bool[,] matched)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (matched[x, y] && _grid[x, y] != null)
                {
                    PlayDestructionEffect(_grid[x, y].transform.position);
                    Destroy(_grid[x, y]);
                    _grid[x, y] = null;
                }
            }
        }

        UpdateGrid(); // Обновляем сетку после уничтожения кругов
    }

    /// <summary>
    /// Обновляет сетку, сдвигая круги вниз.
    /// </summary>
    private void UpdateGrid()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (_grid[x, y] == null)
                {
                    for (int i = y + 1; i < 3; i++)
                    {
                        if (_grid[x, i] != null)
                        {
                            _grid[x, y] = _grid[x, i];
                            _grid[x, i] = null;
                            _grid[x, y].transform.position = new Vector2(x, y); // Обновляем позицию круга
                            break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Воспроизводит эффект разрушения.
    /// </summary>
    /// <param name="position">Позиция для эффекта.</param>
    private void PlayDestructionEffect(Vector2 position)
    {
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Проверяет, являются ли координаты корректными.
    /// </summary>
    /// <param name="x">Координата X.</param>
    /// <param name="y">Координата Y.</param>
    /// <returns>True, если координаты корректны, иначе false.</returns>
    private bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < 3 && y >= 0 && y < 3;
    }

    [ContextMenu("Reset Grid")]
    private void ResetGrid()
    {
        _grid = new GameObject[3, 3];
        Debug.Log("Сетка сброшена.");
    }
}