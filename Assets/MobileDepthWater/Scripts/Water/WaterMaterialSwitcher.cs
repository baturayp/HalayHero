namespace Assets.Scripts.Water
{
    using UnityEngine;

    /// <summary>
    /// This class switchs material of dynamic objects if they enter or exit any water area
    /// After switching to water material it pushes water area properties to dynamic object material
    /// It allows objects to be under the lake or be in the different water
    /// Also switching material to diffuse after exiting the water gives a bit performance
    /// </summary>
    public class WaterMaterialSwitcher : MonoBehaviour
    {
        [SerializeField] private Renderer waterRenderer;
        [SerializeField] private Material waterMaterial;
        [SerializeField] private Material diffuseMaterial;

        private MaterialPropertyBlock defaulPropertyBlock;

        public void Awake()
        {
            defaulPropertyBlock = new MaterialPropertyBlock();
            waterRenderer.GetPropertyBlock(defaulPropertyBlock);
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Water")
            {
                var waterPropertyBlock = collider.GetComponent<WaterArea>().WaterPropertyBlock;

                waterRenderer.sharedMaterial = waterMaterial;
                waterRenderer.SetPropertyBlock(waterPropertyBlock);
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            if (collider.tag == "Water")
            {
                waterRenderer.sharedMaterial = diffuseMaterial;
                waterRenderer.SetPropertyBlock(defaulPropertyBlock);
            }
        }
    }
}
