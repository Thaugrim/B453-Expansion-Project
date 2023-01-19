using UnityEngine;

public class BulletWarningManager : MonoBehaviour
{
    private static BulletWarningManager _Instance;
    public static BulletWarningManager Instance {
        get {
            if (_Instance == null) 
                _Instance = FindObjectOfType<BulletWarningManager>();

            return _Instance;
        }
    }

    [SerializeField] private GameObject bulletWarning;
    public Transform transfCharacter;

    public void InstantiateWarning(Transform transfBullet) {
        GameObject newBulletWarning = Instantiate(bulletWarning, this.transform);

        if (newBulletWarning.TryGetComponent<BulletWarning>(out BulletWarning newWarning)) 
            newWarning.transfBullet = transfBullet;
    }
}
