using UnityStandardAssets.CrossPlatformInput;

namespace PlaneInput
{
    public class HumanPlaneInput : IPlaneInput
    {
        // TODO: Refactor input handling to use a unified input controller

        public float getHorizontal()
        {
            return CrossPlatformInputManager.GetAxis("Horizontal");
        }

        public float getVertical()
        {
            return CrossPlatformInputManager.GetAxis("Vertical");
        }

        public float getRudder()
        {
            return CrossPlatformInputManager.GetAxis("Rudder");
        }

        public float getThrust()
        {
            return CrossPlatformInputManager.GetAxis("Throttle");
        }
    }
}