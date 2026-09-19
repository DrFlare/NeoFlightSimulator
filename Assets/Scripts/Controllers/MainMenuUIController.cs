using AI;
using Architecture;
using Level;
using PlaneInput;
using Simulator;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class MainMenuUIController : MonoBehaviour
    {
        #region Serialize Fields

        [SerializeField] private TMP_Dropdown testNetLevelSelect;
        [SerializeField] private TMP_Dropdown weightsSelect;
        [SerializeField] private GameObject testModelPanel;
        [SerializeField] private GameObject generateLevelsPanel;
        [SerializeField] private TMP_InputField levelCountInputField;
        [SerializeField] private TMP_InputField ringCountInputField;
        [SerializeField] private TMP_InputField filePrefixInputField;
        [SerializeField] private TMP_InputField avgDistInputField;
        [SerializeField] private TMP_InputField distVarInputField;
        [SerializeField] private TMP_InputField maxAngleInputField;
        [SerializeField] private GameObject levelSelectPanel;
        [SerializeField] private TMP_Dropdown levelSelect;
        [SerializeField] private GameObject trainPanel;
        [SerializeField] private TMP_InputField epochInputField;
        [SerializeField] private TMP_InputField framesInputField;

        [SerializeField] private SimulationContext context;

        #endregion

        #region Functions
        
        #region Panels

        public void OpenTrainModelPanel()
        {
            context.neuralNetWeightsPath = "";
            context.inputType = InputType.AI;
            trainPanel.SetActive(true);
        }

        public void CloseTrainModelPanel() => trainPanel.SetActive(false);

        public void OpenTestModelPanel() => testModelPanel.SetActive(true);

        public void CloseTestModelPanel() => testModelPanel.SetActive(false);

        public void OpenGenerateLevelsPanel() => generateLevelsPanel.SetActive(true);

        public void CloseGenerateLevelsPanel() => generateLevelsPanel.SetActive(false);

        public void OpenLevelSelectPanel()
        {
            context.inputType = InputType.Human;
            context.neuralNetWeightsPath = "";
            levelSelectPanel.SetActive(true);
        }

        public void CloseLevelSelectPanel() => levelSelectPanel.SetActive(false);

        public void StartTrainingSimulation()
        {
            var aiTrainer = new AITrainer();

            aiTrainer.StartTrainingSimulation(int.Parse(epochInputField.text), int.Parse(framesInputField.text));

            UpdateWeightLibrary();
            trainPanel.SetActive(false);
        }

        public void GenerateLevels()
        {
            LevelGenerator.GenerateLevels(
                int.Parse(levelCountInputField.text),
                int.Parse(avgDistInputField.text),
                int.Parse(distVarInputField.text),
                float.Parse(maxAngleInputField.text),
                int.Parse(ringCountInputField.text),
                filePrefixInputField.text
            );
            UpdateLevelLibrary();

            generateLevelsPanel.SetActive(false);
        }

        #endregion

        #region Scene Transitions

        public void StartAITest()
        {
            context.inputType = InputType.AI;
            context.levelName = testNetLevelSelect.options[testNetLevelSelect.value].text;
            context.neuralNetWeightsPath = weightsSelect.options[weightsSelect.value].text;

            SceneManager.LoadScene(Constants.SimulationSceneName);
        }

        public void StartLevel()
        {
            context.levelName = levelSelect.options[levelSelect.value].text;

            SceneManager.LoadScene(Constants.SimulationSceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        #endregion

        #region Settings

        private void UpdateLevelLibrary()
        {
            levelSelect.ClearOptions();
            testNetLevelSelect.ClearOptions();
            levelSelect.AddOptions(LevelLoader.GetLevelNames());
            testNetLevelSelect.AddOptions(LevelLoader.GetLevelNames());
            levelSelect.value = 0;
            testNetLevelSelect.value = 0;
        }

        private void UpdateWeightLibrary()
        {
            weightsSelect.ClearOptions();
            weightsSelect.AddOptions(NeuralNet.getSavedWeights());
        }

        #endregion
        
        #endregion

        #region Unity Functions

        private void Start()
        {
            UpdateLevelLibrary();
            UpdateWeightLibrary();
        }

        #endregion
    }
}