public enum TeamType
{
    Hero,
    Enemy,
    Neutral,
    Building
}

public interface IDamageable
{
    int GetCurrentHealth();
    void TakeDamage(int amount);
    TeamType GetTeam();
}
