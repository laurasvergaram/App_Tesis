using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Report : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI[] ReportErroresPag = null;// cuadro pequegno error log

    // Start is called before the first frame update
    void Start()
    {

     for ( int i = 0; i < Constants.errorsCount.Length; i++ )
     {
        ReportErroresPag[i].text = Constants.errorsCount[i].ToString();
     }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
