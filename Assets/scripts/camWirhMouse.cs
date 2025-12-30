using UnityEngine;

public class SmoothCameraFollowMouse : MonoBehaviour
{
    public float smoothTime = 0.3f;
    public float maxOffset = 3f; // Максимальное смещение от центра
    public float sensitivity = 0.1f; // Чувствительность мыши

    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    private Vector3 initialPosition;

    void Start()
    {
        cam = GetComponent<Camera>();
        initialPosition = transform.position;
    }

    void Update()
    {
        // Получаем позицию мыши в нормализованных координатах (-1 до 1)
        Vector3 mouseViewport = cam.ScreenToViewportPoint(Input.mousePosition);

        // Преобразуем в диапазон -1 до 1 от центра
        Vector3 mouseOffset = new Vector3(
            (mouseViewport.x - 0.5f) * 2f,
            (mouseViewport.y - 0.5f) * 2f,
            0f
        );

        // Ограничиваем максимальное смещение
        mouseOffset = Vector3.ClampMagnitude(mouseOffset, 1f);

        // Применяем чувствительность и максимальное смещение
        Vector3 targetOffset = mouseOffset * maxOffset * sensitivity;

        // Целевая позиция = начальная позиция + смещение
        Vector3 targetPosition = initialPosition + targetOffset;
        targetPosition.z = initialPosition.z; // Сохраняем Z

        // Плавное перемещение
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}