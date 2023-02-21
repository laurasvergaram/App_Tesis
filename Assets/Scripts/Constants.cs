using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class Constants : MonoBehaviour
{

    public static int[] errorsCount = new int[9]; // arreglo de contador errores por paginas
    public static bool[] paginasCuento = new bool[9]; // arreglo para mantener estado de completado de paginas anteriores
    public int pagina = 0;

    [SerializeField] private TextMeshProUGUI DebugCont = null;// cuadro pequegno error log


    // Start is called before the first frame update
    void Start()
    {
        if(this.pagina < 0 ) // Condicional para evitar que se crashee al revisar las paginas completadas
        {
            this.pagina = 0;
        }
        else if(this.pagina > paginasCuento.Length - 1)
        {
            this.pagina = paginasCuento.Length - 1;
        }

        // errorsCount[0]=3;
        // errorsCount[1]=6;
        // errorsCount[2]=2;
        // errorsCount[3]=4;
        // errorsCount[4]=8;
        // errorsCount[5]=3;
        // errorsCount[6]=1;
        // errorsCount[7]=5;
        // errorsCount[8]=12;

    }

    // Update is called once per frame
    void Update()
    {
        DebugCont.text = errorsCount[this.pagina].ToString();
    }

    public void ResetData()
    {
        Array.Clear(errorsCount,0,errorsCount.Length);
        Array.Clear(paginasCuento,0,paginasCuento.Length);
    }


}
