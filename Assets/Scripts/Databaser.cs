using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using TMPro;

public class Databaser : MonoBehaviour
{

    public static Databaser Instance;
    public TMP_InputField field1;
    public TMP_InputField field2;

    string conn;
    IDbConnection dbconn;

    private void Awake()
    {
        Instance = this;
        conn = "URI=file:" + Application.dataPath + "/Raggedy.s3db";
    }


    // ------- GENERAL PURPOSE -----------------------------

    public bool DoesNameExistInTable (string fieldName, string tableName)
    {
        if (tableName == "Place")
            tableName = "Locations";
        if (tableName == "Character")
            tableName = "Characters";
        if (tableName == "Item")
            tableName = "Items";
        bool returnValue = false;
        dbconn = new SqliteConnection(conn);
        dbconn.Open(); 
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM " + tableName + " WHERE Name='" + fieldName + "'"; 
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        { 
            var value1 = reader.GetValue(0);
            if (value1 != null)
                returnValue = true;
        } 
        
        reader.Close(); 
        dbcmd.Dispose(); 
        dbconn.Close();
        return returnValue;
    }
    // -------- ELEMENTS -----------------------------


    public void CreateNewElement( string elementName, string placeType, string locationName, string description)
    {
        string tableName = "";
        if (placeType == "Place")
            tableName = "Locations";
        if (placeType == "Character")
            tableName = "Characters";
        if (placeType == "Item")
            tableName = "Items";
        dbconn.Open();
        IDbCommand cmnd = dbconn.CreateCommand();
        cmnd.CommandText = "INSERT INTO "+ tableName + " (Name, AtLocation, Description) VALUES ('" + elementName + "','" + locationName + "','"+ description + "')";
        cmnd.ExecuteNonQuery();
        cmnd.Dispose();
        dbconn.Close();
    }
    public void OverwriteElement(string elementName, string placeType, string locationName, string description)
    {
        string tableName = "";
        if (placeType == "Place")
            tableName = "Locations";
        if (placeType == "Character")
            tableName = "Characters";
        if (placeType == "Item")
            tableName = "Items";
        dbconn.Open();
        IDbCommand cmnd = dbconn.CreateCommand();
        cmnd.CommandText = "REPLACE INTO " + tableName + " (Name, AtLocation, Description) VALUES ('" + elementName + "','" + locationName + "','" + description + "')";
        cmnd.ExecuteNonQuery();
        cmnd.Dispose();
        dbconn.Close();
    }







    // -------- CHAPTERS -----------------------------

    public string GetChapterTextForName(string chapterName)
    {
        string returnString = "No string found!"; 
        dbconn = new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM Chapters WHERE Name='" + chapterName + "'";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        { 
            string value = reader.GetString(1);
            returnString =  value;
        } 
        reader.Close(); 
        dbcmd.Dispose(); 
        dbconn.Close();
        return returnString;
    }
    public string GetChapterEntryNames( )
    {
        string returnString = "";
        dbconn = new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM Chapters";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader(); 
        while (reader.Read())
        { 
            returnString += reader.GetString(0); 
            returnString += ", "; 
        }
        if (returnString.Length > 2)
        returnString = returnString.Substring(0, returnString.Length - 2);
        reader.Close();
        dbcmd.Dispose();
        dbconn.Close();  
        return returnString;
    }
    public void CreateNewChapter (string chapterName, string chapterContent)
    {
        dbconn.Open();
        IDbCommand cmnd = dbconn.CreateCommand();
        cmnd.CommandText = "INSERT INTO Chapters (Name, Content) VALUES ('" + chapterName + "','" + chapterContent + "')";
        cmnd.ExecuteNonQuery();
        cmnd.Dispose();
        dbconn.Close();
    }
    public void OverwriteChapter(string chapterName, string chapterContent)
    {
        dbconn.Open();
        IDbCommand cmnd = dbconn.CreateCommand();
        cmnd.CommandText = "REPLACE INTO Chapters (Name, Content) VALUES ('" + chapterName + "','" + chapterContent + "')";
        cmnd.ExecuteNonQuery();
        cmnd.Dispose();
        dbconn.Close();
    }




    public void ClickTestButton()
    {
        OutputTableField(field1.text, field2.text);
    }
    
    // output each value for one field, identified by name 
    public void OutputTableField (string tableName, string fieldName)
    {

        // open connection
        dbconn = new SqliteConnection(conn);
        dbconn.Open();



        IDbCommand dbcmd = dbconn.CreateCommand();
         string sqlQuery = "SELECT * FROM " + tableName +" WHERE Name='"+fieldName+ "'" ;
      //  string sqlQuery = "SELECT * FROM " + tableName  ;
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {

            //var value1 = reader.GetValue(0);
            //var value2 = reader.GetValue(1); 

            //Debug.Log("value= " + value1 + "  value2 =" + value2 );


            var value1 = reader.GetValue(0).GetType();
            //if (value1 == typeof(String))
            //    Debug.Log("value1= " + reader.GetValue(0));

            var value2 = reader.GetValue(1).GetType();
            //if (value2 == typeof(String))
            //    Debug.Log("value2= " + reader.GetValue(1));
            Debug.Log("value1= " + value1);
            Debug.Log("value2= " + value2);

        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();

    }





     


}
