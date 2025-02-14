using UnityEngine;

public class BodyMaterialSelector : MonoBehaviour
{
    [System.Serializable]
    public class ObjectToColor
    {
        public Renderer bodyRenderer; // Ссылка на Renderer объекта
        public int materialIndex; // Индекс изменяемого материала
    }

    public ObjectToColor[] objectsToColor; // Массив объектов, которые нужно перекрашивать
    public Color[] bodyColors; // Массив возможных цветов
    public Material[] bodyMaterials; // Массив возможных материалов

    void Start()
    {
        if (objectsToColor.Length == 0)
        {
            Debug.LogWarning("Массив объектов для перекрашивания пуст!");
            return;
        }

        // Выбираем случайный цвет и случайный материал (если массивы не пустые)
        Color selectedColor = (bodyColors.Length > 0) ? bodyColors[Random.Range(0, bodyColors.Length)] : Color.white;
        Material selectedMaterial = (bodyMaterials.Length > 0) ? bodyMaterials[Random.Range(0, bodyMaterials.Length)] : null;

        // Применяем выбранный цвет и материал ко всем объектам
        foreach (ObjectToColor obj in objectsToColor)
        {
            if (obj.bodyRenderer == null)
                continue;

            // Создаем копию массива материалов
            Material[] instances = obj.bodyRenderer.materials;

            // Проверяем, что индекс не выходит за пределы массива
            if (obj.materialIndex < 0 || obj.materialIndex >= instances.Length)
            {
                Debug.LogWarning($"Индекс материала {obj.materialIndex} выходит за пределы массива для объекта {obj.bodyRenderer.gameObject.name}");
                continue;
            }

            // Получаем текущий материал
            Material newMaterial = instances[obj.materialIndex];

            // Применяем выбранный цвет
            if (bodyColors.Length > 0)
            {
                newMaterial.SetColor("_Color", selectedColor);
            }

            // Применяем выбранный материал (если указан)
            if (selectedMaterial != null)
            {
                newMaterial = selectedMaterial;
            }

            // Применяем изменения
            instances[obj.materialIndex] = newMaterial;
            obj.bodyRenderer.materials = instances;
        }
    }
}
