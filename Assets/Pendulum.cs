using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public float swingSpeed = 1.0f;
    public float maxAngle = 45.0f;
    public GameObject ballPrefab; // Префаб круга
    public Transform ballSpawnPoint; // Точка, где будет появляться круг

    private float time = 0.0f;
    private GameObject currentBall; // Текущий круг

    void Start()
    {
        SpawnBall();
    }

    void Update()
    {
        // Движение маятника
        time += Time.deltaTime;
        float angle = maxAngle * Mathf.Sin(time * swingSpeed);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Сброс круга при тапе
        if (Input.GetMouseButtonDown(0)) // Тап (или клик мышью)
        {
            DropBall();
        }
    }

    void SpawnBall()
    {
        // Создаем новый круг
        currentBall = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        currentBall.GetComponent<Rigidbody2D>().isKinematic = true; // Отключаем физику, пока круг на маятнике
        currentBall.transform.parent = this.gameObject.transform;
        ChangeBallColor(currentBall);
    }

    void DropBall()
    {
        if (currentBall != null)
        {
            // Включаем физику для круга
            currentBall.GetComponent<Rigidbody2D>().isKinematic = false;
            currentBall.transform.parent = null; // Отсоединяем от маятника
            currentBall = null;

            // Спавним новый круг через небольшой промежуток времени
            Invoke("SpawnBall", 1.0f);
            
        }
    }

    void ChangeBallColor(GameObject ball)
    {
        Color[] colors = { Color.red, Color.blue, Color.green };
        int randomIndex = Random.Range(0, colors.Length);
        ball.GetComponent<SpriteRenderer>().color = colors[randomIndex];
    }
}