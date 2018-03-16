using System.Numerics;

namespace GameExercise
{
    public class Camera
    {
        private float fieldOfView = MathUtils.Radians(45.0f);
        private float nearDistance = 0.1f;
        private float farDistance = 1000f;

        private Vector3 position = new Vector3(4, 3, 3);
        private Vector3 lookAt = new Vector3(0, 0, 0);

        private float windowWidth;
        private float windowHeight;

        private Matrix4x4 viewMatrix;
        private Matrix4x4 projectionMatrix;

        public Camera(float width, float height)
        {
            this.windowWidth = width;
            this.windowHeight = height;

            this.UpdatePerspectiveMatrix(width, height);
            this.UpdateViewMatrix();
        }

        public Matrix4x4 ViewMatrix => this.viewMatrix;
        public Matrix4x4 ProjectionMatrix => this.projectionMatrix;

        public Vector3 Position
        {
            get => this.position;
            set
            {
              this.position = value;
              this.UpdateViewMatrix();
            }
        }
        public Vector3 LookAt => this.lookAt;

        public float FarDistance => this.farDistance;

        public float FieldOfView => this.fieldOfView;

        public float NearDistance => this.nearDistance;

        public float AspectRatio => this.windowWidth / this.windowHeight;

        public void WindowResized(float width, float height)
        {
            this.windowWidth = width;
            this.windowHeight = height;
            this.UpdatePerspectiveMatrix(width, height);
        }

        private void UpdatePerspectiveMatrix(float width, float height)
        {
            this.projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                this.fieldOfView, width / height,
                this.nearDistance,
                this.farDistance);
        }

        private void UpdateViewMatrix()
        {
            this.viewMatrix = Matrix4x4.CreateLookAt(this.position, this.lookAt, Vector3.UnitY);
        }
    }
}
