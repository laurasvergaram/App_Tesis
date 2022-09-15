using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SpeechRecognizerPlugin;

public class SpeechRecognizer : MonoBehaviour, ISpeechRecognizerPlugin
{
    [SerializeField] private Button startListeningBtn = null;
    [SerializeField] private Button stopListeningBtn = null;
    [SerializeField] private Toggle continuousListeningTgle = null;
    [SerializeField] private TMP_Dropdown languageDropdown = null;
    [SerializeField] private TMP_InputField maxResultsInputField = null;
    [SerializeField] private TextMeshProUGUI resultsTxt = null; //cuadro grande que muestra las palabras que son iwales
    [SerializeField] private TextMeshProUGUI resultsTxt2 = null;// cuadro prquegno que muestra lo que se esta hablando en el momento
    [SerializeField] private TextMeshProUGUI errorsTxt = null;

    [SerializeField] private TextMeshProUGUI stringA = null; //Texto del reconocimiento de voz 
    [SerializeField] private TextMeshProUGUI StringB = null; //Texto predefinido del cuento

    private SpeechRecognizerPlugin plugin = null;
    [SerializeField] private string Cuento = null; //String completo del cuento sin mayusculas ni signos de puntuacion
    private string[] CuentoArray = null;// Array completo del cuento
    private int count = 0;// contador global para la posicion de la palabra a comparar del cuento

    private void Start()
    {
        plugin = SpeechRecognizerPlugin.GetPlatformPluginVersion(this.gameObject.name);
        this.CuentoArray = this.Cuento.Split(' ');// asignacion del array del cuento segun el string del cuento
        startListeningBtn.onClick.AddListener(StartListening);
        stopListeningBtn.onClick.AddListener(StopListening);
        continuousListeningTgle.onValueChanged.AddListener(SetContinuousListening);
        languageDropdown.onValueChanged.AddListener(SetLanguage);
        maxResultsInputField.onEndEdit.AddListener(SetMaxResults);
    }

    private void StartListening()
    {
        plugin.StartListening();
    }

    private void StopListening()
    {
        plugin.StopListening();
    }

    private void SetContinuousListening(bool isContinuous)
    {
        plugin.SetContinuousListening(isContinuous);
    }

    private void SetLanguage(int dropdownValue)
    {
        string newLanguage = languageDropdown.options[dropdownValue].text;
        plugin.SetLanguageForNextRecognition(newLanguage);
    }

    private void SetMaxResults(string inputValue)
    {
        if (string.IsNullOrEmpty(inputValue))
            return;

        int maxResults = int.Parse(inputValue);
        plugin.SetMaxResultsForNextRecognition(maxResults);
    }

    public void OnResult(string recognizedResult)
    {
        char[] delimiterChars = { '~' };
        string[] result = recognizedResult.Split(delimiterChars);
        
        for (int i = 0; i < result.Length; i++)
        {
            this.resultsTxt2.text = result[i]; //Muesra lo que se hablo en el momento!!!
            string[] resultArray = result[i].Split(' '); //Crea un array de string que permite comparar palabra por palabra
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
            
        }

        if (count == CuentoArray.Length)
        {
            /* SE COLOCA LA FUNCIONALIDAD AL TERMINAR DE COMPARAR PRRO!!!  */
        }
    }

    public void OnError(string recognizedError)
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
