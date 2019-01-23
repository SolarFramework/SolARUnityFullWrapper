using UnityEngine;
using UnityEngine.Assertions;

namespace SolAR
{
    /// Captures camera input and sets it as the background of your scene.
    [RequireComponent(typeof(Camera))]
    public class SolARVideoController : MonoBehaviour
    {
        [SerializeField] protected PipelineManager solARManager;
        new Camera camera;
        GameObject quad;
        Material material;
        int layoutId;

        protected void Awake()
        {
            Assert.IsNotNull(solARManager);
            camera = GetComponent<Camera>();
        }

        protected void OnEnable()
        {
            solARManager.OnFrame += OnFrame;
            quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.SetParent(transform);
            var z = (camera.nearClipPlane + camera.farClipPlane) / 2;
            quad.transform.SetPositionAndRotation(new Vector3(0, 0, z), Quaternion.identity);
            float aspect = (float)camera.pixelWidth / camera.pixelHeight;
            quad.transform.localScale = z * Mathf.Tan(camera.fieldOfView) * 4 * new Vector3(aspect, 1, 1);
            var renderer = quad.GetComponent<Renderer>();
            Assert.IsNotNull(renderer);
            var shader = Shader.Find("SolAR/UnlitShader");
            Assert.IsNotNull(shader);
            material = new Material(shader);
            renderer.sharedMaterial = material;

            layoutId = Shader.PropertyToID("_Layout");
        }

        protected void OnDisable()
        {
            solARManager.OnFrame -= OnFrame;
            Destroy(quad);
            Destroy(material);
        }

        void OnFrame(Texture texture)
        {
            material.mainTexture = texture;
            material.SetInt(layoutId, 2);
        }
    }
}
