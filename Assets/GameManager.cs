using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using TMPro;
using UnityEngine;

[System.Serializable]
    public class QuestionData{
        public string indonesiaKata;
        public string answerInggris;
    }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private ScriptableAnswer questionDataScriptable;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private WordData[] AnswerPrefabs;
    [SerializeField] private WordData[] WordPrefabs;
    [SerializeField] private GameObject gameOverWins;
    [SerializeField] private GameObject gameOverLose;
    private char[] charWordArray = new char[12];
    private int currentAnswerIndex = 0;
    private bool isAnswer = true;
    private List<int> WordSelectIndex;
    private int currentQuestionIndex = 0;
    private GameState gameState = GameState.OnPLay;
    private string answerWords;
    private int health = 3;

    public enum GameState{
        OnPLay,
        Next
    }

    private void Awake() {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }

        WordSelectIndex = new List<int>();
    }

    private void Start() {
        WordSelectIndex = new List<int>();
        QuestionSet();
    }

    private void QuestionSet(){
        gameState = GameState.OnPLay;

        answerWords = questionDataScriptable.questions[currentQuestionIndex].answerInggris;
        questionText.text = questionDataScriptable.questions[currentQuestionIndex].indonesiaKata;

        QuestionResets();

        WordSelectIndex.Clear();
        Array.Clear(charWordArray, 0, charWordArray.Length);

        for (int i = 0; i < answerWords.Length; i++)
        {
            charWordArray[i] = char.ToUpper(answerWords[i]);
        }

        for (int i = answerWords.Length; i < WordPrefabs.Length; i++)
        {
            charWordArray[i] = (char)UnityEngine.Random.Range(65, 91);
        }

        charWordArray = ShuffleWordList.ShuffleListItems<char>(charWordArray.ToList()).ToArray();

        for (int i = 0; i < WordPrefabs.Length; i++)
        {
            WordPrefabs[i].SetChar(charWordArray[i]);
        }
    }

    public void SelectedOptions(WordData wordData){

        if(gameState == GameState.Next || currentAnswerIndex >= answerWords.Length){
            return;
        }

        WordSelectIndex.Add(wordData.transform.GetSiblingIndex());
        wordData.gameObject.SetActive(false);
        AnswerPrefabs[currentAnswerIndex].SetChar(wordData.CharValue);

        currentAnswerIndex++;

        if(currentAnswerIndex == answerWords.Length){

            isAnswer = true;

            for (int i = 0; i < answerWords.Length; i++)
            {
                if(char.ToUpper(answerWords[i]) != char.ToUpper(AnswerPrefabs[i].CharValue)){
                    isAnswer = false;
                    break;
                }
            }

            if(isAnswer){
                Debug.Log("Jawaban Benar");
                gameState = GameState.Next;
                currentQuestionIndex++;

                if(currentQuestionIndex < questionDataScriptable.questions.Count){
                    Invoke("QuestionSet", 0.5f);
                }
                else{
                    Debug.Log("Game Selesai");
                    gameOverWins.SetActive(true);
                }
            }
            else if(!isAnswer){
                Debug.Log("Salah");
                health--;
                if(health <= 0){
                    gameOverLose.SetActive(true);
                }
                else{
                    QuestionResets();
                }
            }
        }
    }

    public void QuestionResets(){
        for (int i = 0; i < AnswerPrefabs.Length; i++)
        {
            AnswerPrefabs[i].gameObject.SetActive(true);
            AnswerPrefabs[i].SetChar('_');
        }
        for (int i = answerWords.Length; i < AnswerPrefabs.Length; i++)
        {
            AnswerPrefabs[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < WordPrefabs.Length; i++)
        {
            WordPrefabs[i].gameObject.SetActive(true);
        }
        currentAnswerIndex = 0;
    }

    public void LastWordReset(){
        if(WordSelectIndex.Count > 0){
            int index = WordSelectIndex[WordSelectIndex.Count - 1];
            WordPrefabs[index].gameObject.SetActive(true);
            WordSelectIndex.RemoveAt(WordSelectIndex.Count - 1);
            
            currentAnswerIndex--;
            AnswerPrefabs[currentAnswerIndex].SetChar('_');
        }
    }

}

