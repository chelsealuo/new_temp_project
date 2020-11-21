using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBehavior : MonoBehaviour
{
    [SerializeField]
    GameObject canvas;
    [SerializeField]
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            //canvas.SetActive(true);
            //text.GetComponent<TextMesh>().text = "Congrats, you just made the world a cleaner place. \nEach year, more than 8 million tons of plastic is dumped into our oceans!";
        }

        if ((name == "RecycleCollider") &&(other.gameObject.CompareTag("Recycle"))){
            canvas.SetActive(true);
            text.text = "Congrats, you just made the world a cleaner place. \nEach year, more than 8 million tons of plastic is dumped into our oceans!";
        }
        else if ((name == "BiohazardCollider") && (other.gameObject.CompareTag("Biohazard")))
        {
            canvas.SetActive(true);
            text.text = "Congrats, you just made the world a cleaner place. \nEach year, over 180 million tons of Toxic Waste is dumped into our Oceans, Rivers, and Lakes!";
        }else if ((name == "WasteCollider") && (other.gameObject.CompareTag("Waste")))
        {
            canvas.SetActive(true);
            text.text = "Congrats, you just made the world a cleaner place. \nEach year, about 100 million marine animals die from human waste dumped in the ocean";
        }
    }

}