using UnityEngine;

public class spawner : MonoBehaviour
{

    public GameObject ennemi;
    private Color SpawnnerColor;
    // Start is called before the first frame update
    void Start()
    {
        SpawnnerColor = this.gameObject.GetComponent<SpriteRenderer>().color;
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(SpawnnerColor.r, SpawnnerColor.g, SpawnnerColor.b, 0);
    }

    private GameObject[] getCount;
    float count;

    // Update is called once per frame
    void Update()
    {
        getCount = GameObject.FindGameObjectsWithTag("ennemi");
        count = getCount.Length;

        if (count < 1)
        {
            Instantiate(ennemi, transform.position, transform.rotation);

        }
    }
}
