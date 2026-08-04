using System;
using System.Collections.Generic;
using FlightSimulator;
using FlightSimulator.AI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private Button trainModelButton;
    [SerializeField] private Button testModelButton;
    [SerializeField] private Button startHumanSimulationButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TMP_Dropdown testNetLevelSelect;
    [SerializeField] private TMP_Dropdown weightsSelect;
    [SerializeField] private GameObject testModelPanel;
    [SerializeField] private Button startTestButton;
    [SerializeField] private Button cancelTestButton;
    [SerializeField] private Button generateLevelsButton;
    [SerializeField] private GameObject generateLevelsPanel;
    [SerializeField] private TMP_InputField levelCountInputField;
    [SerializeField] private TMP_InputField ringCountInputField;
    [SerializeField] private TMP_InputField filePrefixInputField;
    [SerializeField] private TMP_InputField avgDistInputField;
    [SerializeField] private TMP_InputField distVarInputField;
    [SerializeField] private TMP_InputField maxAngleInputField;
    [SerializeField] private Button confirmGenerateButton;
    [SerializeField] private Button cancelGenerateButton;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private TMP_Dropdown levelSelect;
    [SerializeField] private Button startLevelButton;
    [SerializeField] private Button cancelLevelButton;
    [SerializeField] private GameObject trainPanel;
    [SerializeField] private TMP_InputField epochInputField;
    [SerializeField] private TMP_InputField framesInputField;
    [SerializeField] private Button startTrainButton;
    [SerializeField] private Button cancelTrainButton;

    public SimulationContext context;

    private void Start()
    {
        trainModelButton.onClick.AddListener(OnTrainModelButtonClicked);
        testModelButton.onClick.AddListener(OnTestModelButtonClicked);
        startHumanSimulationButton.onClick.AddListener(OnStartHumanSimulationButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        startTestButton.onClick.AddListener(OnStartTestButtonClicked);
        cancelTestButton.onClick.AddListener(OnCancelTestButtonClicked);
        generateLevelsButton.onClick.AddListener(OnGenerateLevelsButtonClicked);
        confirmGenerateButton.onClick.AddListener(OnConfirmGenerateButtonClicked);
        cancelGenerateButton.onClick.AddListener(OnCancelGenerateButtonClicked);
        startLevelButton.onClick.AddListener(OnStartLevelButtonClicked);
        cancelLevelButton.onClick.AddListener(OnCancelLevelButtonClicked);
        startTrainButton.onClick.AddListener(OnStartTrainButtonClicked);
        cancelTrainButton.onClick.AddListener(OnCancelTrainButtonClicked);

        levelSelect.options = new List<TMP_Dropdown.OptionData>();
        testNetLevelSelect.options = new List<TMP_Dropdown.OptionData>();
        updateLevelLibrary();
        levelSelect.value = 0;
        testNetLevelSelect.value = 0;

        weightsSelect.options = new List<TMP_Dropdown.OptionData>();
        updateWeightLibrary();
        weightsSelect.value = 0;

        testModelPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        generateLevelsPanel.SetActive(false);
        trainPanel.SetActive(false);
    }

    private void updateLevelLibrary()
    {
        levelSelect.ClearOptions();
        testNetLevelSelect.ClearOptions();
        levelSelect.AddOptions(LevelLoader.getLevelNames());
        testNetLevelSelect.AddOptions(LevelLoader.getLevelNames());
        levelSelect.value = 0;
        testNetLevelSelect.value = 0;
    }

    private void updateWeightLibrary()
    {
        weightsSelect.ClearOptions();
        weightsSelect.AddOptions(NeuralNet.getSavedWeights());
    }

    private void OnTrainModelButtonClicked()
    {
        context.neuralNetWeightsPath = "";
        context.inputType = InputType.AI;
        trainPanel.SetActive(true);
    }

    private void OnTestModelButtonClicked()
    {
        testModelPanel.SetActive(true);
    }

    private void OnCancelTestButtonClicked()
    {
        testModelPanel.SetActive(false);
    }

    private void OnStartTestButtonClicked()
    {
        context.inputType = InputType.AI;
        context.levelName = testNetLevelSelect.options[testNetLevelSelect.value].text;
        context.neuralNetWeightsPath = weightsSelect.options[weightsSelect.value].text;

        SceneManager.LoadScene("SimulationScene");
    }

    private void OnStartHumanSimulationButtonClicked()
    {
        context.inputType = InputType.Human;
        context.neuralNetWeightsPath = "";
        levelSelectPanel.SetActive(true);
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private void OnGenerateLevelsButtonClicked()
    {
        generateLevelsPanel.SetActive(true);
    }

    private void OnConfirmGenerateButtonClicked()
    {
        LevelGenerator.generateLevels(
            int.Parse(levelCountInputField.text),
            int.Parse(avgDistInputField.text),
            int.Parse(distVarInputField.text),
            float.Parse(maxAngleInputField.text),
            int.Parse(ringCountInputField.text),
            filePrefixInputField.text
        );
        updateLevelLibrary();

        generateLevelsPanel.SetActive(false);
    }

    private void OnCancelGenerateButtonClicked()
    {
        generateLevelsPanel.SetActive(false);
    }

    private void OnStartLevelButtonClicked()
    {
        context.levelName = levelSelect.options[levelSelect.value].text;

        SceneManager.LoadScene("SimulationScene");
    }

    private void OnCancelLevelButtonClicked()
    {
        levelSelectPanel.SetActive(false);
    }

    private void OnStartTrainButtonClicked()
    {
        var aiTrainer = new AITrainer();

        aiTrainer.startTrainingSimulation(int.Parse(epochInputField.text), int.Parse(framesInputField.text));

        updateWeightLibrary();
        trainPanel.SetActive(false);
    }

    private void OnCancelTrainButtonClicked()
    {
        trainPanel.SetActive(false);
    }
}