namespace FlightSimulator.AI
{
    public interface Layer
    {
        public float[] forward(float[] input);

        public float[] backward(float[] grads);
    }
}