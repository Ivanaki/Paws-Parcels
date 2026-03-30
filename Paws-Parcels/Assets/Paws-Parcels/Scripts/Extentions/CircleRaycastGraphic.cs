using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Extensions
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class CircleRaycastGraphic : Graphic, ICanvasRaycastFilter
    {
        [SerializeField, Min(0f)]
        private float _radiusMultiplier = 1f;

        /// <summary>
        /// 0 = ничего не кликается
        /// 1 = радиус равен половине меньшей стороны RectTransform
        /// >1 = круг выходит за пределы прямоугольника
        /// </summary>
        public float RadiusMultiplier
        {
            get => _radiusMultiplier;
            set => _radiusMultiplier = Mathf.Max(0f, value);
        }

        protected override void Awake()
        {
            base.Awake();

            // Не перетираем пользовательские настройки без необходимости.
            // Но если компонент только что добавлен, Reset это настроит.
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Так как графика ничего не рисует, гарантируем пустую геометрию.
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (!isActiveAndEnabled)
                return false;

            if (_radiusMultiplier <= 0f)
                return false;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform, screenPoint, eventCamera, out Vector2 localPoint))
            {
                return false;
            }

            Rect rect = rectTransform.rect;
            Vector2 center = rect.center;

            float radius = Mathf.Min(rect.width, rect.height) * 0.5f * _radiusMultiplier;
            if (radius <= 0f)
                return false;

            Vector2 delta = localPoint - center;
            return delta.sqrMagnitude <= radius * radius;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            _radiusMultiplier = 1f;

            // Компонент нужен именно как invisible raycast target.
            color = new Color(1f, 1f, 1f, 0f);
            raycastTarget = true;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            _radiusMultiplier = Mathf.Max(0f, _radiusMultiplier);

            // На случай, если в редакторе геометрия/состояние не обновились.
            if (IsActive())
                SetVerticesDirty();
        }

        private void OnDrawGizmosSelected()
        {
            Rect rect = rectTransform.rect;
            Vector2 center = rect.center;
            float radius = Mathf.Min(rect.width, rect.height) * 0.5f * Mathf.Max(0f, _radiusMultiplier);

            if (radius <= 0f)
                return;

            Handles.color = Color.green;

            Matrix4x4 previousMatrix = Handles.matrix;
            Handles.matrix = rectTransform.localToWorldMatrix;

            // Рисуем в локальном пространстве RectTransform,
            // чтобы корректно учитывались позиция, поворот и масштаб.
            Handles.DrawWireDisc(center, Vector3.forward, radius);

            Handles.matrix = previousMatrix;
        }
#endif
    }
}