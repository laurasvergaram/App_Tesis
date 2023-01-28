using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class Constants : MonoBehaviour
{

    public static int errorsCount = 0; // contador errores
    public static bool[] paginasCuento = new bool[8]; // arreglo para mantener estado de completado de paginas anteriores

    [SerializeField] private TextMeshProUGUI DebugCont = null;// cuadro pequegno error log


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DebugCont.text = errorsCount.ToString();
    }
}
