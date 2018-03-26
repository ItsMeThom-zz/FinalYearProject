namespace Assets.NoiseProviders
{
    public interface INoiseProvider
    {
        float GetValue(float x, float z);
    }
}