using UnityEngine;
using UnityEngine.UI;

namespace Nobi.UiRoundedCorners
{
    [ExecuteInEditMode]								//Required to check the OnEnable function
    [DisallowMultipleComponent]                     //You can only have one of these in every object.
    [RequireComponent(typeof(RectTransform))]
    public class RoundedCornersWithOutline : MonoBehaviour
    {
        private static readonly int WidthHeight = Shader.PropertyToID("_WidthHeight");
        private static readonly int CornerRadius = Shader.PropertyToID("_CornerRadius");
        private static readonly int OutlineThickness = Shader.PropertyToID("_OutlineThickness");
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

        public float radius = 40f;
        public float outlineThickness = 5f;
        public Color outlineColor = Color.black;

        private Material material;
        [HideInInspector, SerializeField] private MaskableGraphic image;

        private void OnValidate()
        {
            Validate();
            Refresh();
        }

        private void OnDestroy()
        {
            if (image != null)
            {
                image.material = null;
            }
            DestroyHelper.Destroy(material);
            image = null;
            material = null;
        }

        private void OnEnable()
        {
            var other = GetComponent<ImageWithIndependentRoundedCorners>();
            if (other != null)
            {
                radius = other.r.x;
                DestroyHelper.Destroy(other);
            }

            Validate();
            Refresh();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (enabled && material != null)
            {
                Refresh();
            }
        }

        public void Validate()
        {
            if (material == null)
            {
                material = new Material(Shader.Find("UI/RoundedCorners/RoundedCornersWithOutline"));
            }

            if (image == null)
            {
                TryGetComponent(out image);
            }

            if (image != null)
            {
                image.material = material;
            }
        }

        public void Refresh()
        {
            var rect = ((RectTransform)transform).rect;
            material.SetVector(WidthHeight, new Vector4(rect.width, rect.height, 0, 0));
            material.SetFloat(CornerRadius, radius * 2f);
            material.SetFloat(OutlineThickness, outlineThickness);
            material.SetColor(OutlineColor, outlineColor);
        }
    }
}