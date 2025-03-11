using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public float swingSpeed = 1.0f;
    public float maxAngle = 45.0f;
    public GameObject ballPrefab; // ������ �����
    public Transform ballSpawnPoint; // �����, ��� ����� ���������� ����

    private float time = 0.0f;
    private GameObject currentBall; // ������� ����

    void Start()
    {
        SpawnBall();
    }

    void Update()
    {
        // �������� ��������
        time += Time.deltaTime;
        float angle = maxAngle * Mathf.Sin(time * swingSpeed);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // ����� ����� ��� ����
        if (Input.GetMouseButtonDown(0)) // ��� (��� ���� �����)
        {
            DropBall();
        }
    }

    void SpawnBall()
    {
        // ������� ����� ����
        currentBall = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        currentBall.GetComponent<Rigidbody2D>().isKinematic = true; // ��������� ������, ���� ���� �� ��������
        currentBall.transform.parent = this.gameObject.transform;
        ChangeBallColor(currentBall);
    }

    void DropBall()
    {
        if (currentBall != null)
        {
            // �������� ������ ��� �����
            currentBall.GetComponent<Rigidbody2D>().isKinematic = false;
            currentBall.transform.parent = null; // ����������� �� ��������
            currentBall = null;

            // ������� ����� ���� ����� ��������� ���������� �������
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