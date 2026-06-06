using System;
using _Data;
using UnityEditor;
using UnityEngine;

namespace MyEditor {
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(SingleEnumAttribute))]
    public class SingleEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
        
            // Получаем реальный тип (учитывая, что это может быть массив или список)
            Type enumType = GetElementType(fieldInfo.FieldType);
        
            // Защита от дурака: если вдруг атрибут повесили не на Enum, рисуем стандартно
            if (enumType == null || !enumType.IsEnum)
            {
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.EndProperty();
                return;
            }

            // Конвертируем текущее int значение свойства в реальный Enum
            var currentValue = (Enum)Enum.ToObject(enumType, property.intValue);

            // Отрисовываем обычный EnumPopup, который игнорирует [Flags]
            var selectedValue = EditorGUI.EnumPopup(position, label, currentValue);

            // Сохраняем выбранный результат обратно в property как int
            property.intValue = Convert.ToInt32(selectedValue);
        
            EditorGUI.EndProperty();
        }

        // Вспомогательный метод для извлечения типа из List<T> или T[]
        private Type GetElementType(Type type)
        {
            // Если это массив (например, ItemType[])
            if (type.IsArray)
                return type.GetElementType();
        
            // Если это список (например, List<ItemType>)
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return type.GetGenericArguments()[0];
        
            // Если это обычное одиночное поле (ItemType)
            return type;
        }
    }


}
