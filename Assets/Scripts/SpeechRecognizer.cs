using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SpeechRecognizerPlugin;
using System.Text;
using System.Text.RegularExpressions;



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

    public bool ActiveListening; // Dejar activado la escucha de la app desde la primera pag sin necesidad de darle nuevamente a comenzar

    [SerializeField] private TextMeshProUGUI stringA = null; //Texto del reconocimiento de voz 
    [SerializeField] private TextMeshProUGUI StringB = null; //Texto predefinido del cuento

    private SpeechRecognizerPlugin plugin = null; // Renombra el archivo de SpeechRecognizerPlugin del cual se saca la informacion inicial de idioma, max results, escucha continua, etc

    [SerializeField] private string Cuento = null; //String completo del cuento sin mayusculas ni signos de puntuacion
    private string[] CuentoArray = null;// Array completo del cuento
    private int count = 0;// contador global para la posicion de la palabra a comparar del cuento

    private void Start()
    {
        WinPanel.gameObject.SetActive(false);
        plugin = SpeechRecognizerPlugin.GetPlatformPluginVersion(this.gameObject.name);
        this.CuentoArray = Regex.Replace(this.Cuento.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "").ToLower().Split(' ');// asignacion del array del cuento segun el string del cuento quitando las tildes
        startListeningBtn.onClick.AddListener(StartListening);
        stopListeningBtn.onClick.AddListener(StopListening);// Desactivado NO SE USA, viene con el script que se esta usando de reconocimiento de voz
        continuousListeningTgle.onValueChanged.AddListener(SetContinuousListening);
        languageDropdown.onValueChanged.AddListener(SetLanguage);
        maxResultsInputField.onEndEdit.AddListener(SetMaxResults);

        if(this.NumPagina < 0 ) // Condicional para evitar que se crashee al revisar las paginas completadas
        {
            this.NumPagina = 0;
        }
        else if(this.NumPagina > Constants.paginasCuento.Length - 1)
        {
            this.NumPagina = Constants.paginasCuento.Length - 1;
        }
        
        if(Constants.paginasCuento[this.NumPagina]) // Condicional que activa/desactiva objetos al guardar el estado de la pagina anterior
        {
            WinPanel.gameObject.SetActive(false);
            nextBtn.gameObject.SetActive(true);
            StartPanel.gameObject.SetActive(false);
        }

        if(this.ActiveListening) // Condicion para que siempre que este activado esta opcion desde el unity, se llame al comenzar a escuchar
        {
            StartListening();
        }
    }

    private void StartListening() // Funcion que llama al script SpeechRecognizerPlugin para comenzar a escuchar
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

    public void ExitStartPanel()
    {
        StartPanel.gameObject.SetActive(false);
    }

    private void SetContinuousListening(bool isContinuous) // Funcion que llama al script SpeechRecognizerPlugin para comenzar tener la escucha continua (seteado a true)
    {
        plugin.SetContinuousListening(isContinuous);
    }

    private void SetLanguage(int dropdownValue) // Funcion que llama al script SpeechRecognizerPlugin para saber que lenguaje escuchar principalmente (seteado a ES)
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

    public void OnResult(string recognizedResult) // Funcion que muestra y compara los resultados escuchados
    {
        Debug.Log(recognizedResult);
        char[] delimiterChars = { '~' };
        string[] result = recognizedResult.Split(delimiterChars);
        int tempCount = this.count;
        for (int i = 0; i < result.Length; i++)
        {
            this.resultsTxt2.text = result[i]; //Muesra lo que se hablo en el momento!!!
            string[] resultArray = Regex.Replace(result[i].Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "").ToLower().Split(' '); //Crea un array de string que permite comparar palabra por palabra quitando las tildes
            for (int c = 0; c < resultArray.Length; c++) // este for compara palabra por palabra del plugin vs el cuento
            {
                stringA.text = resultArray[c]; //Texto del reconocimiento de voz 
                StringB.text = CuentoArray[this.count]; // texto predefinido del cuento
                if (string.Compare(resultArray[c], CuentoArray[this.count]) == 0) //valida si las palabras son iguales
                {
                    resultsTxt.text += resultArray[c] + ' ';//Asignacion en el panel de result
                    if (count < CuentoArray.Length)
                    {
                        this.count++; //contador global para la posicion de la palabra del cuanto a comparar
                    }
                }
            }
            // Errores en la lectura (IN PROGRESS) NO TOCAR --------------------------------------------------------------
            if(this.count == tempCount){
                Constants.errorsCount++; 
            }else{
                tempCount = this.count;
            }
            // -----------------------------------------------------------------------------------------------------------
        }

        if (count == CuentoArray.Length) // condicion de terminado al igualar completamente el cuento con lo escuchado por la app
        {
            /* SE COLOCA LA FUNCIONALIDAD AL TERMINAR DE COMPARAR */

            Constants.paginasCuento[this.NumPagina] = true; // activa cada pagina leida y guarda el estado de leido mientras la aplicacion esta activa
            WinPanel.gameObject.SetActive(true);

            if(nextBtn != null) // condicional evita que se crashee la app por falta del boton next en la ultima pagina
            {
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
