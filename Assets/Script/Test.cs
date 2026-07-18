using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class Test : MonoBehaviour
{
    private int number = 0;

    private float timer = 0f;
    void Awake()
    {
        Debug.Log("Awake called");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start called");
    }

    // Update is called once per frame
    async Task Update()
    {
        timer += Time.deltaTime;
        number++;
        if (timer >= 1f)
        {
            Debug.Log("Timer reached 1 second: " + number);
            timer = 0f; // Reset the timer
            number = 0;
        }
    }
}
