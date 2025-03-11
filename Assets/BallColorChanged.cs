using UnityEngine;

public class BallColorChanger : MonoBehaviour
{
    public Color[] colors = { Color.red, Color.blue, Color.green };
    private int currentColorIndex = 0;

    void Start()
    {
        ChangeColor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            ChangeColor();
        }
    }

    void ChangeColor()
    {
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        GetComponent<SpriteRenderer>().color = colors[currentColorIndex];
    }
}
