using UnityEngine;

public class FloatingMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float floatHeight = 1f;        // Высота плавания
    public float floatSpeed = 1f;         // Скорость плавания

    [Header("Rotation Settings")]
    public bool enableRotation = true;    // Включить вращение
    public float rotationAngle = 10f;     // Угол вращения

    private Vector3 startPosition;
    private float timer;

    void Start()
    {
        // Сохраняем начальную позицию
        startPosition = transform.position;
    }

    void Update()
    {
        // Обновляем таймер с использованием синуса для плавного движения вверх-вниз
        timer += Time.deltaTime * floatSpeed;

        // Используем синус для плавного движения вверх-вниз
        // Sin дает значения от -1 до 1, преобразуем в 0-1 для высоты
        float verticalMovement = Mathf.Sin(timer);

        // Вычисляем новую позицию
        Vector3 newPosition = startPosition;
        newPosition.y += verticalMovement * floatHeight;

        // Применяем позицию
        transform.position = newPosition;

        // Применяем вращение, если включено
        if (enableRotation)
        {
            // Используем косинус для сдвига фазы вращения относительно движения
            float rotationValue = Mathf.Cos(timer);
            transform.rotation = Quaternion.Euler(0, 0, rotationValue * rotationAngle);
        }
    }

    // Метод для изменения высоты во время выполнения
    public void SetFloatHeight(float newHeight)
    {
        floatHeight = newHeight;
    }

    // Метод для изменения скорости во время выполнения
    public void SetFloatSpeed(float newSpeed)
    {
        floatSpeed = newSpeed;
    }

    // Метод для сброса позиции
    public void ResetPosition()
    {
        timer = 0f;
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
    }
}