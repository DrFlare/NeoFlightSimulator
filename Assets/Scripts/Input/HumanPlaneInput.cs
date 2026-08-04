using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace FlightSimulator
{
    public class HumanPlaneInput : PlaneInput
    {
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