using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChapterController : MonoBehaviour
{
    public static ChapterController Instance;
    public TMP_InputField chapterNameField;
    public TMP_InputField chapterContentField;
    public Button finishButton;
    public TextMeshProUGUI finishButtonText;
    public TMP_InputField currentEntryText;
    public Toggle entryToggle;
    public TextMeshProUGUI entryToggleText;
    public TextMeshProUGUI headerText;

    enum ButtonStatus { Create, Overwrite}
    ButtonStatus buttonStatus;
    enum ToggleMode { Entries, Current}
    ToggleMode toggleMode;

    bool nameFieldHasContent;
    bool nameFieldExistsInDB;
    bool contentFieldHasContent;
    bool contentFieldIsIdenticalToDB;

    bool justCreatedEntry = false;
    bool justOverwroteEntry = false;

     


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        chapterNameField.onValueChanged.AddListener(delegate { ChapterFieldHandler(); });
        chapterContentField.onValueChanged.AddListener(delegate { ChapterFieldHandler(); });
    }


    public void ClickFinishButton()
    {
        if (buttonStatus == ButtonStatus.Create)
        {
            // create the field in the DB
            Databaser.Instance.CreateNewChapter(chapterNameField.text, chapterContentField.text);
            justCreatedEntry = true;
        }
        else if (buttonStatus == ButtonStatus.Overwrite)
        {
            // change the field in the DB
            Databaser.Instance.OverwriteChapter(chapterNameField.text, chapterContentField.text);
            justOverwroteEntry = true;
        }
        ChapterFieldHandler();
    }



    private void ChapterFieldHandler()
    { 
        if (chapterNameField.text == ""  )
            nameFieldHasContent = false;
        else
            nameFieldHasContent = true;

        if (chapterContentField.text == "")
            contentFieldHasContent = false;
        else
            contentFieldHasContent = true;

        if (nameFieldHasContent == true)
            nameFieldExistsInDB = Databaser.Instance.DoesNameExistInTable(chapterNameField.text, "Chapters");

        UpdateFinishButton();

      
    }

    void UpdateFinishButton()
    { 
        if (justCreatedEntry == true)
        {
            finishButton.interactable = false;
            finishButtonText.text = "Chapter Saved";
            justCreatedEntry = false;
            UpdateToggle(ToggleMode.Entries); 
            return;
        } 
        if (justOverwroteEntry == true)
        {
            finishButton.interactable = false;
            finishButtonText.text = "Chapter Replaced";
            justOverwroteEntry = false;
            UpdateToggle(ToggleMode.Entries);
            return;
        }
        // check if there is any text at all
        if (nameFieldHasContent == false || contentFieldHasContent == false)
        {
            finishButton.interactable = false;
            finishButtonText.text = "Save Chapter";
            UpdateToggle(ToggleMode.Entries);
            if (nameFieldHasContent == true && nameFieldExistsInDB == true)
                UpdateToggle(ToggleMode.Current);
        }
        else
        { 
            finishButton.interactable = true;
            // check if chapter title already exists. 
            if (nameFieldExistsInDB == true)
            {
                UpdateToggle(ToggleMode.Current);
                buttonStatus = ButtonStatus.Overwrite;
                finishButtonText.text = "Overwrite Chapter";
                //  If so, check that content entry is different.
                string currentChapterText = Databaser.Instance.GetChapterTextForName(chapterNameField.text);
                 
                // same content
                if (currentChapterText == chapterContentField.text) {
                    UpdateToggle(ToggleMode.Current);
                    finishButtonText.text = "Content Identical";
                    finishButton.interactable = false;
                }else
                {
                    finishButtonText.text = "Overwrite Chapter";
                    finishButton.interactable = true;
                }

            }
            else
            {
                UpdateToggle(ToggleMode.Entries);

                finishButtonText.text = "Save Chapter";
                buttonStatus = ButtonStatus.Create;
            }

            //if (nameFieldExistsInDB == false)
            //    finishButton.interactable = true;
            //else
            //    finishButton.interactable = false;
        }


    }

    void UpdateToggle (ToggleMode mode)
    {
       
        if (mode == ToggleMode.Current)
        {
            entryToggleText.text = "Show Chapter Content";
        }
        else if (mode == ToggleMode.Entries)
        {
            entryToggleText.text = "Show Chapter Names";
        }
        toggleMode = mode;
        if (toggleMode == ToggleMode.Current)
        {
            currentEntryText.text = Databaser.Instance.GetChapterTextForName(chapterNameField.text);
            headerText.text = "Chapter Text";
        }
        else if (toggleMode == ToggleMode.Entries)
        {
            currentEntryText.text = Databaser.Instance.GetChapterEntryNames();
          //  headerText.text = "Chapters";
        }
    }
    public void CurrentDataToggle (bool toggled)
    {
        if (toggled == true)
        {
            if (toggleMode == ToggleMode.Current) 
                 headerText.text = "Chapter Text";
            else if (toggleMode == ToggleMode.Entries)
                headerText.text = "Chapters";
            currentEntryText.text = Databaser.Instance.GetChapterTextForName(chapterNameField.text);
            currentEntryText.gameObject.SetActive(true);
            chapterContentField.gameObject.SetActive(false);
        }
        else
        {
            headerText.text = "Chapter Text";
            currentEntryText.gameObject.SetActive(false);
            chapterContentField.gameObject.SetActive(true);
        }
        if (toggleMode == ToggleMode.Current)
            currentEntryText.text = Databaser.Instance.GetChapterTextForName(chapterNameField.text);
        else if (toggleMode == ToggleMode.Entries)
            currentEntryText.text = Databaser.Instance.GetChapterEntryNames();
    }
}
