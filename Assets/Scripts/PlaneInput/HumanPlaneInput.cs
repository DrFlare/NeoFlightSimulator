using UnityStandardAssets.CrossPlatformInput;

namespace PlaneInput
{
    public class HumanPlaneInput : IPlaneInput
    {
        // TODO: Refactor input handling to use a unified input controller

        public float GetHorizontal()
        {
            return CrossPlatformInputManager.GetAxis("Horizontal");
        }

        public float GetVertical()
        {
            return CrossPlatformInputManager.GetAxis("Vertical");
        }

        public float GetRudder()
        {
            return CrossPlatformInputManager.GetAxis("Rudder");
        }

        public float GetThrust()
        {
            return CrossPlatformInputManager.GetAxis("Throttle");
        }
    }
}