using UnityEngine;

public class BulletWarning : MonoBehaviour
{
    public Transform transfBullet;
    private Transform transfCharacter;
    [SerializeField] private Vector2 offset;

    private void Start()
    {
        transfCharacter = BulletWarningManager.Instance.transfCharacter;
        transform.position = transform.position - (Vector3)offset;
    }

    private void Update()
    {
        if (transfBullet == null) Destroy(this.gameObject);
        else {
            Vector3 lookPos = transfBullet.position - transfCharacter.position;
            Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.forward);
            float eulerZ = lookRot.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, 0, eulerZ);
            transform.rotation = rotation;

            DestroyOnBulletNearby();
        }
    }

    void DestroyOnBulletNearby() {
        if (Vector3.Distance(transfBullet.position, transfCharacter.position) < 10) {
            print ("ta perto vo morre"); 
            Destroy(this.gameObject);
        }
    }
}
