using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace Goforth_Howser_FinalProject
{
    public partial class Form1 : Form
    {
        private string filepath;
        private string resultsOutputFP;
        private StreamWriter swSummary;



        public Form1()
        {
            InitializeComponent();
            filepath = "";

            swSummary = new StreamWriter("Goforth-Howser_Summary.csv");
            swSummary.WriteLine("filename,castSearch,directorSearch,keywordSearch");


        }

        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                filepath = openFileDialog1.FileName;
            }
            else
            {
                filepath = "";
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
                                                  

            if (filepath == "")
            {
                MessageBox.Show("Select database first");
                return;
            }

            string connectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};", filepath);

            OleDbConnection dbConn = new OleDbConnection(connectionString);

            dbConn.Open();

            OleDbCommand dbCommand = new OleDbCommand();
            dbCommand.Connection = dbConn;
            dbCommand.CommandText = "SELECT * FROM MoviePlots";

            OleDbDataReader dbReader = dbCommand.ExecuteReader();


            string[] castArray = crit_array(textBox1.Text);
            string[] directorArray = crit_array(textBox2.Text);
            string[] keywordArray = crit_array(textBox3.Text);

            //convert criteria arrays to strings to create search results output file name
            string concat_cast="";
            string concat_director = "";
            string concat_keyword = "";

            for(int i =0; i<castArray.Length; i++)
            {
                concat_cast += castArray[i];
            }

            for (int i = 0; i < directorArray.Length; i++)
            {
                concat_director += directorArray[i];
            }

            for (int i = 0; i < keywordArray.Length; i++)
            {
                concat_keyword += keywordArray[i];
            }

            //create search-results output file name
            resultsOutputFP = "Goforth_Howser_" + concat_cast + concat_director + concat_keyword + ".csv";

            //write to the summary output
            swSummary.WriteLine("{0},{1},{2},{3}",resultsOutputFP, concat_cast, concat_director, concat_keyword);


            //instantiate Search-results output CSV file with title line
            StreamWriter swResults = new StreamWriter(resultsOutputFP);
            swResults.WriteLine("ReleaseYear,Title,Director,Cast,Summary");

            string[] search_criteria = new string[3];
            //q1 = new List<FirstQueryObjects>();
            //FirstQueryObjects q = new FirstQueryObjects();

            bool match=false;

            while (dbReader.Read())//read row by row until no rows left
            { 

                string mpCast = dbReader[5].ToString();
                string mpDirector = dbReader[4].ToString();
                string mpPlot = dbReader[8].ToString(); //raw database plot summary
                string summary = ""; //mpPlot.Substring(0, 25)  **the first 25 characters of mpPlot
                string mpYear = dbReader[1].ToString();
                string mpTitle = dbReader[2].ToString();

                search_criteria[0] = textBox1.Text;
                search_criteria[1] = textBox2.Text;
                search_criteria[2] = textBox3.Text;

                int result_Count = 0;
                int crit_Counter = 0;


                if (search_criteria[0] != "")
                {
                    crit_Counter++;

                    bool queryTF = query_Criteria(castArray, mpCast);
                    if (queryTF == true) //(queryTF == true)//if the above method returns true / there's a match... 
                    {
                        result_Count++;
                    }
                }

                if (search_criteria[1] != "")
                {
                    crit_Counter++;

                    bool queryTF1 = query_Criteria(directorArray, mpDirector);
                    if (queryTF1 == true)
                    {
                        result_Count++;
                    }
                }

                if (search_criteria[2] != "")
                {
                    crit_Counter++;

                    bool queryTF2 = query_Criteria(keywordArray, mpPlot);
                    if (queryTF2 == true)
                    {
                        result_Count++;
                    }
                }

                //check summary length
                if (mpPlot.Count() < 25)
                {
                    summary = mpPlot;
                }
                else
                {
                    summary = mpPlot.Substring(0, 25);
                }

                //if the line matches the search criteria, print
                if (crit_Counter == result_Count)
                {
                    match = true;

                    richTextBox1.AppendText(String.Format("{0},  {1},  {2},  {3}, {4}\n", mpYear, mpTitle, mpDirector, mpCast, summary));

                    //put quotes around strings that contain a comma
                    if (mpYear.Contains(','))
                    {
                        mpYear = $"\"{mpYear}\"";
                    }

                    if (mpDirector.Contains(','))
                    {
                        mpDirector = $"\"{mpDirector}\"";
                    }

                    if (mpCast.Contains(','))
                    {
                        mpCast = $"\"{mpCast}\"";
                    }

                    if (summary.Contains(','))
                    {
                        summary = $"\"{summary}\"";
                    }

                    //remove any rogue new line characters
                    if (mpTitle.Contains("\n"))
                    {
                        mpTitle.Replace("\n", "");
                    }

                    if (mpTitle.Contains("\n"))
                    {
                        mpTitle.Replace("\n", "");
                    }

                    if (mpDirector.Contains("\n"))
                    {
                        mpDirector.Replace("\n", "");
                    }

                    if (mpCast.Contains("\n"))
                    {
                        mpCast.Replace("\n", "");
                    }

                    if (summary.Contains("\n"))
                    {
                        summary.Replace("\n", "");
                    }

                    swResults.WriteLine("{0},{1},{2},{3},{4}", mpYear, mpTitle, mpDirector, mpCast, summary);
                }
                


            }

            //close reader
            dbReader.Close();
            //close DB connection
            dbConn.Close();
            //close StreamWriter for results-output output file
            swResults.Close();

            if (match==false)
            {
                richTextBox1.Text=$"No Matches Found";
            }

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //close StreamWriter for summary CSV when the form closes.
            swSummary.Close();
        }

        //method to validate user input/criteria        
        static string[] crit_array(string SC_text)
        {
            List<string> cList = new List<string>();
            string[] cArray;

            

            if (SC_text.Contains(",") == true)
            {
                
                string[] cSplit = SC_text.Split(',');

                for (int i = 0; i < cSplit.Length; i++)
                {
                    if (cSplit[i].Contains(' ') == true)
                    {
                        string[] spSplit = cSplit[i].Split(' ');

                        for (int n = 0; n < spSplit.Length; n++)
                        {
                            cList.Add(spSplit[n]);
                        }
                    }
                    else if (!cSplit[i].Contains(' '))
                    {
                        cList.Add(cSplit[i]);
                    }
                }
                cArray = cList.ToArray();

                return cArray;
            }
            else
            {
                cArray = SC_text.Split(' ');

                return cArray;
            }
        }

        //query Method: takes in any user variable [after validation] and any database variable
        //returns true if a match is found and the query is not empty
        static bool query_Criteria(string[] cArray, string MP_var)
        {

            var cQuery = from g in cArray
                         where MP_var.ToUpper().Contains(g.ToUpper())
                         select g;

            if (cQuery.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

                                   
        }

        
    }
}
