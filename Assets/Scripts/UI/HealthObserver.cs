public interface IHealthObserver
{
    void OnHealthChange(float change, float newHealth)
    {
    }

    void OnDeath()
    {
    }
}
