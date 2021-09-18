using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Cannon references
    public SpriteRenderer cannonSprite;
    public Transform firePoint;

    public GameObject bulletPrefab;

    //Path
    public GameObject pathObject;
    public GameObject point;
    GameObject[] points;
    public int numberOfPoints;
    public float spaceBetweenPoints;
    bool pathState;

    //Attributes
    public float minPower;
    public float maxPower;
    [SerializeField] private float currentPower;
    Vector2 direction;

    private void Start()
    {
        currentPower = 0;

        points = new GameObject[numberOfPoints];

        for(int i = 0; i < numberOfPoints; i++)
        {
            points[i] = Instantiate(point, firePoint.position, Quaternion.identity);
            points[i].transform.SetParent(pathObject.transform);
        }

        IsPathActive(false);
    }

    private void Update()
    {
        //Get firePoint and mouse position in the game world
        Vector2 firePointPosition = firePoint.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Calculate direction and rotate cannonSprite
        direction = mousePosition - firePointPosition;
        cannonSprite.transform.right = direction;

        //Calculate currentPower
        if (Input.GetMouseButton(0))
        {
            currentPower += 5 * Time.deltaTime;
            currentPower = Mathf.Clamp(currentPower, minPower, maxPower);
            IsPathActive(true);
        }

        //Shoot
        if (Input.GetMouseButtonUp(0))
            Shoot();

        //Create bullet's path
        if (pathState)
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                points[i].transform.position = PointPosition(i * spaceBetweenPoints);
            }
        }
    }

    void Shoot()
    {
        //Shoot bullet
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, cannonSprite.transform.rotation);
        bulletObject.GetComponent<Rigidbody2D>().velocity =  bulletObject.transform.right * currentPower;

        currentPower = 0;

        IsPathActive(false);
    }

    Vector2 PointPosition(float t)
    {
        //Starting position + velocity + 0,5 * accelleration force * t square
        Vector2 position = (Vector2)firePoint.position + (direction.normalized * currentPower * t) + 0.5f * Physics2D.gravity * (t * t);
        return position;
    }

    void IsPathActive(bool _value)
    {
        pathObject.SetActive(_value);
        pathState = _value;
    }
}
