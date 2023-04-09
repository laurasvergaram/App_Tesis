using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SpeechRecognizerPlugin;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;



public class SpeechRecognizer : MonoBehaviour, ISpeechRecognizerPlugin
{
    [SerializeField] private Button startListeningBtn = null; // Boton de comenzar
    [SerializeField] private Button stopListeningBtn = null; // Desactivado NO SE USA, viene con el script que se esta usando de reconocimiento de voz
    [SerializeField] private Toggle continuousListeningTgle = null; // Desactivado NO SE USA, viene con el script que se esta usando de reconocimiento de voz
    [SerializeField] private TMP_Dropdown languageDropdown = null; // Desactivado NO SE USA, viene con el script que se esta usando de reconocimiento de voz
    [SerializeField] private TMP_InputField maxResultsInputField = null;  // Desactivado NO SE USA, viene con el script que se esta usando de reconocimiento de voz
    [SerializeField] private TextMeshProUGUI resultsTxt = null; //cuadro grande que muestra las palabras que son iwales
    [SerializeField] private TextMeshProUGUI resultsTxt2 = null;// cuadro prquegno que muestra lo que se esta hablando en el momento
    [SerializeField] private TextMeshProUGUI errorsTxt = null; // Desactivado NO SE USA, viene con el script que se esta usando de reconocimiento de voz

    [SerializeField] private Button nextBtn = null; // Boton pasar de pagina activo solo al terminar de leer
    public int NumPagina = 0; // Numero de pagina seteado dentro del unity (0-8, 9 paginas en total)
    public GameObject WinPanel; // Panel de felicitaciones
    public GameObject StartPanel; // Panel de recomendaciones/inicio
    public GameObject ResetPanel = null; // Panel de resetear


    public bool ActiveListening; // Dejar activado la escucha de la app desde la primera pag sin necesidad de darle nuevamente a comenzar

    [SerializeField] private TextMeshProUGUI stringA = null; //Texto del reconocimiento de voz 
    [SerializeField] private TextMeshProUGUI StringB = null; //Texto predefinido del cuento

    private SpeechRecognizerPlugin plugin = null; // Renombra el archivo de SpeechRecognizerPlugin del cual se saca la informacion inicial de idioma, max results, escucha continua, etc

    [SerializeField] private string Cuento = null; //String completo del cuento sin mayusculas ni signos de puntuacion
    private string[] CuentoArray = null;// Array completo del cuento
    private int count = 0;// contador global para la posicion de la palabra a comparar del cuento

    /**
     * Start
     * * Este método es el encargado de inicializar el  SpeechRecognizer 
     * *    el cual fué modificado para hacerlo compatible con las necesidades
     * *    que presenta este proyecto.
     * TODO: Este método fue modificado para la integración con el proyecto.
     * @param NaN
     */
    private void Start()
    {
        ResetPanel.gameObject.SetActive(false);
        WinPanel.gameObject.SetActive(false);
        plugin = SpeechRecognizerPlugin.GetPlatformPluginVersion(this.gameObject.name);

        //? Asignación del cuento procesando los caracteres especiales
        this.CuentoArray = Regex.Replace(this.Cuento.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "").ToLower().Split(' ');
        
        startListeningBtn.onClick.AddListener(StartListening);

        //! Desactivado NO SE USA, viene con el script que se esta usando de reconocimiento de voz
        stopListeningBtn.onClick.AddListener(StopListening); 
        
        continuousListeningTgle.onValueChanged.AddListener(SetContinuousListening);
        languageDropdown.onValueChanged.AddListener(SetLanguage);
        maxResultsInputField.onEndEdit.AddListener(SetMaxResults);

        //? Este condiciconal asegura la integridad del valor mínimo y máximo de las páginas.
        if(this.NumPagina < 0 ){
            this.NumPagina = 0;
        } else if(this.NumPagina > Constants.paginasCuento.Length - 1)
        {
            this.NumPagina = Constants.paginasCuento.Length - 1;
        }
        
        /**
         * ? Este condicional permite activar y desactivar dependiendo si se ha completado
         *      anteriormente la página durante la iteración actual.
         */ 
        if(Constants.paginasCuento[this.NumPagina]){
            WinPanel.gameObject.SetActive(false);
            nextBtn.gameObject.SetActive(true);
            StartPanel.gameObject.SetActive(false);
            resultsTxt.text = Cuento;
             // * Una vez el cuento se complete se activa esta condicional permitiendo reiniciar la iteración.
            if(this.NumPagina == 0 && Constants.paginasCuento.Aggregate(true,(acumulado,valorActual) => acumulado && valorActual)) {
                ResetPanel.gameObject.SetActive(true);
            }

        }
        else{
            resultsTxt.text = "";
        }

        //? Permite activar por defecto el plugin de escucha.
        if(this.ActiveListening) {
            StartListening();
        }
    }

    private void StartListening() // Función que llama al script SpeechRecognizerPlugin para comenzar a escuchar
    {
        plugin.StartListening();
    }

    private void StopListening() // No se usa, viene con el script que se esta usando de reconocimiento de voz
    {
        plugin.StopListening();
    }

    public void ExitCongrats() 
    {
        WinPanel.gameObject.SetActive(false);
    }

    public void ExitReset() 
    {
        ResetPanel.gameObject.SetActive(false);
    }

    public void ExitStartPanel()
    {
        StartPanel.gameObject.SetActive(false);
    }

    private void SetContinuousListening(bool isContinuous) // Función que llama al script SpeechRecognizerPlugin para comenzar tener la escucha continua (seteado a true)
    {
        plugin.SetContinuousListening(isContinuous);
    }

    private void SetLanguage(int dropdownValue) // Función que llama al script SpeechRecognizerPlugin para saber que lenguaje escuchar principalmente (seteado a ES)
    {
        string newLanguage = languageDropdown.options[dropdownValue].text;
        plugin.SetLanguageForNextRecognition(newLanguage);
    }

    private void SetMaxResults(string inputValue) // Funcion que llama al script SpeechRecognizerPlugin para tener un maximo de resultados posibles (seteado a 1)
    {
        if (string.IsNullOrEmpty(inputValue))
            return;

        int maxResults = int.Parse(inputValue);
        plugin.SetMaxResultsForNextRecognition(maxResults);
    }
    /**
     * OnResult
     * * Este método es el encargado del procesamiento pesado de la cadena retornada por el plugin 
     * *    el cual lleva el acumulado de los aciertos y errores en cada iteración por cada página.   
     * TODO: Este método fue modificado para la integración con el proyecto.
     * @param recognizedResult Es la cadena resultante que nos retorna el plugin de las palabras captadas.
     */
    public void OnResult(string recognizedResult){
        Debug.Log(recognizedResult);
        char[] delimiterChars = { '~' };
        string[] result = recognizedResult.Split(delimiterChars);
        int tempCount = this.count;
        for (int i = 0; i < result.Length; i++)
        {
            this.resultsTxt2.text = result[i]; //? Muestra las últimas palabras captadas.

            //? Trasforma el valor primitivo en un array el cual prodrá ser analizado
            string[] resultArray = Regex.Replace(result[i].Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "").ToLower().Split(' ');
            
            //? Este ciclo recorre todas las palabras captadas por el plugin y procesadas anteriormente.
            for (int c = 0; c < resultArray.Length; c++){
                stringA.text = resultArray[c]; //? Muestra la última palabra captada por el plugin 
                StringB.text = CuentoArray[this.count]; //? Muestra la palabra actual del cuento

                //? Valida la concordancia entre la palabra del plugin y la palabra actual del cuento
                if (string.Compare(resultArray[c], CuentoArray[this.count]) == 0){
                    resultsTxt.text += resultArray[c] + ' '; //? Asigna las palabras correctas al panel de resultados
                    if (count < CuentoArray.Length){
                        this.count++; //? Contador para la posición de la palabra del cuento a comparar
                    }
                }
                else{
                    if (!Constants.paginasCuento[this.NumPagina]){
                    Constants.errorsCount[this.NumPagina]++; //? Acumula los errores al momento de leer el cuento
                    }
                }
            }
        }

        //? Esta condición se activa cuando la cantidad de aciertos sea la misma al tamaño del cuento
        if (count == CuentoArray.Length) {
            /** 
             * *SE COLOCA LA FUNCIONALIDAD AL TERMINAR DE COMPARAR 
             */

            //? Activa cada página leída y guarda el estado de leído mientras la aplicación está activa
            Constants.paginasCuento[this.NumPagina] = true; 
            WinPanel.gameObject.SetActive(true);

            //? Condicional evita que se crashee la app por falta del botón next en la última página
            if(nextBtn != null){
                nextBtn.gameObject.SetActive(true);
            }
        }
    }



    public void OnError(string recognizedError) // No se usa, viene con el script que se esta usando de reconocimiento de voz
    {
        ERROR error = (ERROR)int.Parse(recognizedError);
        switch (error)
        {
            case ERROR.UNKNOWN:
                Debug.Log("<b>ERROR: </b> Unknown");
                errorsTxt.text += "Unknown";
                break;
            case ERROR.INVALID_LANGUAGE_FORMAT:
                Debug.Log("<b>ERROR: </b> Language format is not valid");
                errorsTxt.text += "Language format is not valid";
                break;
            default:
                break;
        }
    }
}
