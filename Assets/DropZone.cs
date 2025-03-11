using TMPro;
using UnityEngine;

/// <summary>
/// �����, ���������� �� ��������� ��������� ����� � ����.
/// </summary>
public class DropZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [Tooltip("������ ���� (0, 1 ��� 2).")]
    [SerializeField] private int zoneIndex;

    [Tooltip("������ ����, � �������� �������� ���������.")]
    [SerializeField] private Transform zoneTransform;

    private TowerManager _towerManager;
    private Match3Logic _match3Logic;

    private void Start()
    {
        InitializeComponents();
    }

    /// <summary>
    /// �������������� ����������� ����������.
    /// </summary>
    private void InitializeComponents()
    {
        if (_towerManager == null)
        {
            _towerManager = FindObjectOfType<TowerManager>();
            if (_towerManager == null)
            {
                Debug.LogError("TowerManager �� ������ �� �����.");
            }
        }

        if (_match3Logic == null)
        {
            _match3Logic = FindObjectOfType<Match3Logic>();
            if (_match3Logic == null)
            {
                Debug.LogError("Match3Logic �� ������ �� �����.");
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
    /// ������������ ������������ ���� � �����.
    /// </summary>
    /// <param name="circle">������ ����, ������� ����� � ����.</param>
    private void HandleBallCollision(GameObject circle)
    {
        if (_towerManager == null || _match3Logic == null)
        {
            Debug.LogError("����������� ���������� �� ����������������.");
            return;
        }

        if (_towerManager.TryAddCircleToTower(zoneIndex, circle, zoneTransform.position))
        {
            int height = _towerManager.GetHeight(zoneIndex) - 1;
            _match3Logic.AddCircleToGrid(zoneIndex, height, circle);
        }
        else
        {
            Debug.Log("����� ���������! ��� �� ��������.");
        }
    }

    /// <summary>
    /// ��������� ��������� ������ ����.
    /// </summary>

}