using SolAR.Datastructure;
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

            layoutId = Shader.PropertyToID("_Layout");
        }

        protected void OnEnable()
        {
            solARManager.OnCalibrate += OnCalibrate;
            solARManager.OnFrame += OnFrame;

            quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.SetParent(transform);
            MoveVideoPlane();

            var renderer = quad.GetComponent<Renderer>();
            Assert.IsNotNull(renderer);
            var shader = Shader.Find("SolAR/UnlitShader");
            Assert.IsNotNull(shader);
            material = new Material(shader)
            {
                mainTextureOffset = new Vector2(0, 1),
                mainTextureScale = new Vector2(1, -1),
            };
            material.SetInt(layoutId, 2);
            renderer.sharedMaterial = material;
        }

        protected void OnDisable()
        {
            solARManager.OnCalibrate -= OnCalibrate;
            solARManager.OnFrame -= OnFrame;
            Destroy(quad);
            Destroy(material);
        }

        private void OnCalibrate(Sizei resolution, Matrix3x3f intrinsic, Vector5Df distorsion)
        {
            /*
            var m = new Matrix4x4();
            for (int r = 0; r < 3; ++r)
            {
                for (int c = 0; c < 3; ++c)
                {
                    m[r, c] = intrinsic.coeff(r, c);
                }
            }
            Debug.Log(m);
            Debug.Log(resolution.width);
            Debug.Log(resolution.height);
            */
            var fy = intrinsic.coeff(1, 1);
            var fov = Mathf.Atan2(resolution.height / 2, fy) * Mathf.Rad2Deg * 2;
            camera.fieldOfView = fov;

            MoveVideoPlane();
        }

        void MoveVideoPlane()
        {
            var z = (camera.nearClipPlane + camera.farClipPlane) / 2;
            quad.transform.SetPositionAndRotation(new Vector3(0, 0, z), Quaternion.identity);
            float aspect = (float)camera.pixelWidth / camera.pixelHeight;
            quad.transform.localScale = z * Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2) * 2 * new Vector3(aspect, 1, 1);
        }

        void OnFrame(Texture texture)
        {
            material.mainTexture = texture;
        }
    }
}
