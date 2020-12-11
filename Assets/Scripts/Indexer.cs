using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Indexer : MonoBehaviour
{
    
    enum IndexerState { Unfocused, Focused, ReadyToSave, JustSaved}
    IndexerState indexerState;

    public Sprite loSprite;
    public Sprite hiSprite;
    public TMP_InputField chapterContentField;
    public TMP_InputField nameField;
    public TMP_InputField typeField;
    public TMP_InputField locationField;
    public TMP_InputField descriptionField;
    public TextMeshProUGUI feedbackText;

    bool justStartedIndexing;
    string chapterText;

    void Start()
    {
        GetComponent<Image>().sprite = loSprite;

        nameField.onValueChanged.AddListener(delegate { NameFieldHandler(); });
        typeField.onValueChanged.AddListener(delegate { TypeFieldHandler(); });

        CloseIndexer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            UpdateFeedbackText();

            if (indexerState == IndexerState.Unfocused)
            {
                if (chapterContentField.isFocused == true)
                    OpenIndexer();
            } 
            else if (indexerState == IndexerState.Focused) 
                    CloseIndexer();
            else if (indexerState == IndexerState.ReadyToSave)
                SaveFromIndexer();
        }
        if (Input.GetKeyUp(KeyCode.Return))
        {
           
            if (indexerState == IndexerState.Focused)
            { 
                if (nameField.text != "")
                {
                    if ( typeField.text.ToLower() != "place" && typeField.text.ToLower() != "character" && typeField.text.ToLower() != "item")
                    {
                        typeField.Select();
                        typeField.ActivateInputField();
                    }
                    else if (locationField.text == "")
                    {
                        locationField.Select();
                        locationField.ActivateInputField();
                    }
                    else if (descriptionField.text == "")
                    {
                        descriptionField.Select();
                        descriptionField.ActivateInputField();
                    }

                }

                UpdateFeedbackText();
            }
        }
    } 

    void SaveFromIndexer()
    {
        // overwrite if exists. otherwise save new.
        bool existsInDB = Databaser.Instance.DoesNameExistInTable(nameField.text, typeField.text);

        string feedbackString = "";
        if (existsInDB == true)
        {
            Databaser.Instance.OverwriteElement(nameField.text, typeField.text, locationField.text, descriptionField.text);
            feedbackString += "Overwrote ";
        }
        else
        {
            Databaser.Instance.CreateNewElement(nameField.text, typeField.text, locationField.text, descriptionField.text);
            feedbackString += "Saved "; 
        }
        indexerState = IndexerState.JustSaved;
        feedbackString += nameField.text + "! ";
        CloseIndexer();
        feedbackText.text = feedbackString;

    }

    void NameFieldHandler()
    {
        if (indexerState != IndexerState.JustSaved)
        {
            if (justStartedIndexing == true)
            {
                chapterText = chapterContentField.text;
                if (chapterText.Length > 2)
                    if (chapterText.Substring(chapterText.Length - 2, chapterText.Length - 1) != " ")
                        chapterText += " ";
                justStartedIndexing = false;
            }
            chapterContentField.text = chapterText + nameField.text;
            UpdateFeedbackText();
        } 

    }
    void TypeFieldHandler()
    {
        if (typeField.text.ToLower() == "p")
        {
            typeField.text = "Place";

            typeField.caretPosition = typeField.text.Length;
            typeField.selectionAnchorPosition = 5; 
            typeField.ForceLabelUpdate();
        }
        else if (typeField.text.ToLower() == "c")
            typeField.text = "Character";
        else if (typeField.text.ToLower() == "i")
            typeField.text = "Item";


        UpdateFeedbackText();

    }
    void OpenIndexer()
    {
        nameField.readOnly = false;
        typeField.readOnly = false;
        locationField.readOnly = false;
        descriptionField.readOnly = false;
        justStartedIndexing = true;
        indexerState = IndexerState.Focused;
        GetComponent<Image>().sprite = hiSprite; 
        nameField.ActivateInputField();

        UpdateFeedbackText();
    }



    void CloseIndexer()
    {
        nameField.readOnly = true;
        typeField.readOnly = true;
        locationField.readOnly = true;
        descriptionField.readOnly = true;
        GetComponent<Image>().sprite = loSprite;
        chapterContentField.ActivateInputField();
        //   chapterContentField.text = chapterText + nameField.text;
        nameField.text = "";
        typeField.text = "";
        locationField.text = "";
        descriptionField.text = "";
        indexerState = IndexerState.Unfocused;
        UpdateFeedbackText();
    }

    void UpdateFeedbackText()
    {
        if (indexerState == IndexerState.Unfocused)
        {
            feedbackText.text = "Press L-CTRL to Index";
        }
        else if (indexerState == IndexerState.Focused)
        {
            if (nameField.text == "")
            {
                feedbackText.text = "Enter Name";
            }
            else if (typeField.text.ToLower() != "place" && typeField.text.ToLower() != "character" && typeField.text.ToLower() != "item")
            {
                feedbackText.text = "Enter Type";
            }
            else if (locationField.text == "")
            {
                // TODO: check if enter location exists!
                feedbackText.text = "Enter location or '-'";
            }
            else if (descriptionField.text == "")
            {
                feedbackText.text = "Enter description then press ENTER";
            }
            else   
            {
                feedbackText.text = "Press LCTRL to save or overwrite!";
                indexerState = IndexerState.ReadyToSave;
            }
        }
    }

}
