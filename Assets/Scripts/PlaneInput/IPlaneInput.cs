namespace PlaneInput
{
    public interface IPlaneInput
    {
        float GetHorizontal();
        float GetVertical();
        float GetRudder();
        float GetThrust();
    }
}