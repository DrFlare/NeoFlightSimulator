namespace FlightSimulator
{
    public interface PlaneInput
    {
        float getHorizontal();
        float getVertical();
        float getRudder();
        float getThrust();
    }
}