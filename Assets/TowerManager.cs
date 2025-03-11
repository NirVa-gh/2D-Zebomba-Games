using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �����, ���������� �� ���������� ������� � �����.
/// </summary>
public class TowerManager : MonoBehaviour
{
    [Header("Tower Settings")]
    [Tooltip("������ �����. ������������ ��� ������� ������ �����.")]
    [SerializeField] private float circleRadius = 1f;

    [Tooltip("������������ ������ �����.")]
    [Range(1, 5)]
    [SerializeField] private int maxHeight = 3;

    [Header("Zone Texts")]
    [Tooltip("��������� ������� ��� ����������� ���������� ����� � �����.")]
    [SerializeField] private TextMeshProUGUI[] zoneTexts;

    private int[] _towerHeights = new int[3]; // ������ ����� � ������ ����
    public List<GameObject>[] _towerCircles; // ������ ������� ��� �������� ����� ������ �����

    private void Start()
    {
        InitializeTowerCircles(); // �������������� ������ ��� ������ �����
        ValidateZoneTexts();
        UpdateZoneTexts();
    }

    /// <summary>
    /// �������������� ������ ��� �������� ����� ������ �����.
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
    /// ��������� ���� � ����� � ���������� true, ���� �������.
    /// </summary>
    /// <param name="zoneIndex">������ ����.</param>
    /// <param name="circle">������ �����.</param>
    /// <param name="zonePosition">������� ����.</param>
    /// <returns>True, ���� ���� ������� ��������, ����� false.</returns>
    public bool TryAddCircleToTower(int zoneIndex, GameObject circle, Vector2 zonePosition)
    {
        if (!IsValidZoneIndex(zoneIndex))
        {
            Debug.LogError("������������ ������ ����.");
            return false;
        }

        if (IsTowerFull(zoneIndex))
        {
            Debug.LogWarning("����� �������� ������������ ������!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return false;
        }

        Vector2 targetPosition = CalculateTargetPosition(zoneIndex, zonePosition);
        StartCoroutine(MoveCircleToPosition(circle, targetPosition));

        _towerHeights[zoneIndex]++;
        Debug.Log(GetCirclesForTower(zoneIndex).Count);
        UpdateZoneTexts();

        // ��������� ��� � ������ ��������������� �����
        _towerCircles[zoneIndex].Add(circle);

        return true;
    }

    /// <summary>
    /// ���������� ������� ������ ����� � ��������� ����.
    /// </summary>
    /// <param name="zoneIndex">������ ����.</param>
    /// <returns>������ ����� ��� -1, ���� ������ �����������.</returns>
    public int GetHeight(int zoneIndex)
    {
        if (!IsValidZoneIndex(zoneIndex))
        {
            Debug.LogError("������������ ������ ����.");
            return -1;
        }

        return _towerHeights[zoneIndex];
    }

    /// <summary>
    /// ���������� ������ ���� �����.
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
        Debug.Log("����� ��������.");
    }

    /// <summary>
    /// ������ ���������� ��� � ������� �������.
    /// </summary>
    /// <param name="circle">������ �����.</param>
    /// <param name="targetPosition">������� �������.</param>
    private IEnumerator MoveCircleToPosition(GameObject circle, Vector2 targetPosition)
    {
        Rigidbody2D circleRb = circle.GetComponent<Rigidbody2D>();
        circleRb.isKinematic = true; // ��������� ������ �� ����� �����������

        float duration = 0.5f; // ������������ �����������
        float elapsed = 0f;
        Vector2 startPosition = circle.transform.position;

        while (elapsed < duration)
        {
            circle.transform.position = Vector2.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        circle.transform.position = targetPosition; // ��������, ��� ��� ����� �� �����
        circleRb.isKinematic = false; // �������� ������ �������
    }

    /// <summary>
    /// ��������� ��������� ������� ��� ����������� ���������� ����� � �����.
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
    /// ���������, ��������� �� ����� � ��������� ����.
    /// </summary>
    /// <param name="zoneIndex">������ ����.</param>
    /// <returns>True, ���� ����� ���������, ����� false.</returns>
    private bool IsTowerFull(int zoneIndex)
    {
        return _towerHeights[zoneIndex] >= maxHeight;
    }

    /// <summary>
    /// ���������, �������� �� ������ ���� ����������.
    /// </summary>
    /// <param name="zoneIndex">������ ����.</param>
    /// <returns>True, ���� ������ ���������, ����� false.</returns>
    private bool IsValidZoneIndex(int zoneIndex)
    {
        return zoneIndex >= 0 && zoneIndex < _towerHeights.Length;
    }

    /// <summary>
    /// ������������ ������� ������� ��� �����.
    /// </summary>
    /// <param name="zoneIndex">������ ����.</param>
    /// <param name="zonePosition">������� ����.</param>
    /// <returns>������� ������� ��� �����.</returns>
    private Vector2 CalculateTargetPosition(int zoneIndex, Vector2 zonePosition)
    {
        return new Vector2(zonePosition.x, zonePosition.y + circleRadius * _towerHeights[zoneIndex]);
    }

    /// <summary>
    /// ���������, ��������� �� ��������� ������� ��� ���.
    /// </summary>
    private void ValidateZoneTexts()
    {
        if (zoneTexts == null || zoneTexts.Length == 0)
        {
            Debug.LogWarning("��������� ������� ��� ��� �� ���������.");
        }
    }
    [ContextMenu("Get Circles For Tower")]
    /// <summary>
    /// ���������� ������ ����� ��� ��������� �����.
    /// </summary>
    /// <param name="zoneIndex">������ ����.</param>
    /// <returns>������ ����� ��� null, ���� ������ �����������.</returns>
    public List<GameObject> GetCirclesForTower(int zoneIndex)
    {
        if (!IsValidZoneIndex(zoneIndex))
        {
            Debug.LogError("������������ ������ ����.");
            return null;
        }

        return _towerCircles[zoneIndex];
    }

    /// <summary>
    /// ������� ��� ������ �����.
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