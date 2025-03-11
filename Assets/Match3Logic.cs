using UnityEngine;

/// <summary>
/// �����, ���������� �� ������ ���������� � ����������� ������.
/// </summary>
public class Match3Logic : MonoBehaviour
{
    [Header("Match Settings")]
    [Tooltip("������ ���������� ��� ���������� ������.")]
    [SerializeField] private ParticleSystem destructionEffect;

    private GameObject[,] _grid = new GameObject[3, 3]; // ����� 3x3 ��� �������� ������

    /// <summary>
    /// ��������� ���� � ����� � ��������� ����������.
    /// </summary>
    /// <param name="zoneIndex">������ ���� (�������������� ����������).</param>
    /// <param name="height">������ (������������ ����������).</param>
    /// <param name="circle">������ �����.</param>
    public void AddCircleToGrid(int zoneIndex, int height, GameObject circle)
    {
        if (!IsValidCoordinate(zoneIndex, height))
        {
            Debug.LogError("������������ ���������� ��� ���������� ����� � �����.");
            return;
        }

        _grid[zoneIndex, height] = circle;
        CheckForMatches();
    }

    /// <summary>
    /// ��������� ��� ��������� ����� �� ���������� ������.
    /// </summary>
    private void CheckForMatches()
    {
        bool[,] matched = new bool[3, 3]; // ������ ��� ������� ����������� ������

        // �������� �������������� �����
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

        // �������� ������������ �����
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

        // �������� ������������ ����� (������-�����)
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

        // �������� ������������ ����� (������-����)
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

        // ����������� ���������� ������
        DestroyMatchedCircles(matched);
    }

    /// <summary>
    /// ��������� ����� �� ���������� ������.
    /// </summary>
    /// <param name="x">��������� ���������� X.</param>
    /// <param name="y">��������� ���������� Y.</param>
    /// <param name="dx">��� �� ��� X.</param>
    /// <param name="dy">��� �� ��� Y.</param>
    /// <returns>True, ���� ����� ���������, ����� false.</returns>
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
    /// �������� ����� �� ���� ����������� ������.
    /// </summary>
    /// <param name="matched">������ ��� ������� ����������� ������.</param>
    /// <param name="x">��������� ���������� X.</param>
    /// <param name="y">��������� ���������� Y.</param>
    /// <param name="dx">��� �� ��� X.</param>
    /// <param name="dy">��� �� ��� Y.</param>
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
    /// ���������� ���������� ����� � ��������� �����.
    /// </summary>
    /// <param name="matched">������ � ����������� �������.</param>
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

        UpdateGrid(); // ��������� ����� ����� ����������� ������
    }

    /// <summary>
    /// ��������� �����, ������� ����� ����.
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
                            _grid[x, y].transform.position = new Vector2(x, y); // ��������� ������� �����
                            break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// ������������� ������ ����������.
    /// </summary>
    /// <param name="position">������� ��� �������.</param>
    private void PlayDestructionEffect(Vector2 position)
    {
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, position, Quaternion.identity);
        }
    }

    /// <summary>
    /// ���������, �������� �� ���������� �����������.
    /// </summary>
    /// <param name="x">���������� X.</param>
    /// <param name="y">���������� Y.</param>
    /// <returns>True, ���� ���������� ���������, ����� false.</returns>
    private bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < 3 && y >= 0 && y < 3;
    }

    [ContextMenu("Reset Grid")]
    private void ResetGrid()
    {
        _grid = new GameObject[3, 3];
        Debug.Log("����� ��������.");
    }
}