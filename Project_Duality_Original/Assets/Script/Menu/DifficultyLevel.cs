using UnityEngine;

public class DifficultyLevel : MonoBehaviour
{
    private static DifficultyLevel _Instance;
    public static DifficultyLevel Instance {
        get {
            if (_Instance == null) 
                _Instance = FindObjectOfType<DifficultyLevel>();

            return _Instance;
        }
    }

    public enum Difficulty {Default, NoStress, Easy, Medium, Hard, Impossible}
    [SerializeField] public Difficulty difficulty;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
