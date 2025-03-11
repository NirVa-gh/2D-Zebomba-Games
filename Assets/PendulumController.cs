using UnityEngine;

/// <summary>
/// Класс, отвечающий за управление маятником и сбросом шаров.
/// </summary>
public class PendulumController : MonoBehaviour
{
    [Header("Pendulum Settings")]
    [Tooltip("Префаб круга, который будет сбрасываться маятником.")]
    [SerializeField] private GameObject circlePrefab;

    [Tooltip("Точка, где появляется круг перед сбросом.")]
    [SerializeField] private Transform circleSpawnPoint;

    [Tooltip("Скорость движения маятника.")]
    [Range(50f, 200f)]
    [SerializeField] private float motorSpeed = 100f;

    [Tooltip("Сила движения маятника.")]
    [Range(500f, 1500f)]
    [SerializeField] private float motorForce = 1000f;

    [Tooltip("Сохранять ли импульс при сбросе круга.")]
    [SerializeField] private bool saveImpulse = true;

    [Header("Ball Colors")]
    [Tooltip("Цвета, которые будут применяться к шарам по очереди.")]
    [SerializeField] private Color[] ballColors = { Color.blue, Color.red, Color.green };

    private HingeJoint2D _hingeJoint;
    private GameObject _currentCircle;
    private int _currentColorIndex = 0; // Индекс текущего цвета

    private void Start()
    {
        InitializeComponents();
        ConfigureHingeJoint();
        SpawnCircle();
    }

    private void Update()
    {
        HandleInput();
        UpdateMotorDirection();
    }

    /// <summary>
    /// Инициализирует необходимые компоненты.
    /// </summary>
    private void InitializeComponents()
    {
        _hingeJoint = GetComponent<HingeJoint2D>();
        if (_hingeJoint == null)
        {
            Debug.LogError("HingeJoint2D не найден на объекте.");
        }

        if (circlePrefab == null)
        {
            Debug.LogError("Префаб круга не назначен.");
        }

        if (circleSpawnPoint == null)
        {
            Debug.LogError("Точка спавна круга не назначена.");
        }

        if (ballColors == null || ballColors.Length == 0)
        {
            Debug.LogError("Цвета для шаров не назначены.");
        }
    }

    /// <summary>
    /// Настраивает HingeJoint2D для маятника.
    /// </summary>
    private void ConfigureHingeJoint()
    {
        if (_hingeJoint == null) return;

        JointAngleLimits2D limits = _hingeJoint.limits;
        limits.min = -45;
        limits.max = 45;
        _hingeJoint.limits = limits;
        _hingeJoint.useLimits = true;

        JointMotor2D motor = _hingeJoint.motor;
        motor.motorSpeed = motorSpeed;
        motor.maxMotorTorque = motorForce;
        _hingeJoint.motor = motor;
        _hingeJoint.useMotor = true;
    }

    /// <summary>
    /// Обрабатывает ввод пользователя.
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && _currentCircle != null)
        {
            DropCircle();
        }
    }

    /// <summary>
    /// Обновляет направление движения мотора маятника.
    /// </summary>
    private void UpdateMotorDirection()
    {
        if (_hingeJoint == null) return;

        float currentAngle = _hingeJoint.jointAngle;
        if (currentAngle >= _hingeJoint.limits.max || currentAngle <= _hingeJoint.limits.min)
        {
            JointMotor2D motor = _hingeJoint.motor;
            motor.motorSpeed *= -1;
            _hingeJoint.motor = motor;
        }
    }

    /// <summary>
    /// Создает новый круг в точке спавна.
    /// </summary>
    private void SpawnCircle()
    {
        if (circlePrefab == null || circleSpawnPoint == null) return;

        _currentCircle = Instantiate(circlePrefab, circleSpawnPoint.position, Quaternion.identity);
        Rigidbody2D circleRb = _currentCircle.GetComponent<Rigidbody2D>();
        if (circleRb != null)
        {
            circleRb.isKinematic = true;
        }
        _currentCircle.transform.parent = circleSpawnPoint;

        // Устанавливаем цвет шара
        SetCircleColor(_currentCircle);
    }

    /// <summary>
    /// Устанавливает цвет шара.
    /// </summary>
    /// <param name="circle">Объект шара.</param>
    private void SetCircleColor(GameObject circle)
    {
        SpriteRenderer circleRenderer = circle.GetComponent<SpriteRenderer>();
        if (circleRenderer != null && ballColors.Length > 0)
        {
            circleRenderer.color = ballColors[_currentColorIndex];
            _currentColorIndex = (_currentColorIndex + 1) % ballColors.Length; // Переход к следующему цвету
        }
    }

    /// <summary>
    /// Сбрасывает текущий круг.
    /// </summary>
    private void DropCircle()
    {
        if (_currentCircle == null) return;

        Rigidbody2D circleRb = _currentCircle.GetComponent<Rigidbody2D>();
        if (circleRb != null)
        {
            circleRb.isKinematic = false;
            _currentCircle.transform.parent = null;

            if (saveImpulse)
            {
                circleRb.linearVelocity = GetComponent<Rigidbody2D>().GetPointVelocity(circleSpawnPoint.position);
            }
        }

        _currentCircle = null;
        Invoke(nameof(SpawnCircle), 1f);
    }
}