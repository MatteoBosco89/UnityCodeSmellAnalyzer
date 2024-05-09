using UnityEngine;

public class RotateCube : MonoBehaviour
{
    [SerializeField] protected GameObject cubeRef;
    [SerializeField] protected GameObject myPrefab;
    protected Rigidbody rb;

    void Update()
    {
        cubeRef = GameObject.Find("Cube");
        cubeRef.transform.Rotate(0, 20, 0);
        GameObject a = Instantiate(myPrefab);
    }
}
