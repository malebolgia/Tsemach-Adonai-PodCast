using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using FTP_ProgressBar;
using Microsoft.Win32;
using SaviorClass;

/*Program by: Aaron Jefferson Villanueva */
///<summary>
///  * Tsemach Username Password FTP
///  * FTP Server: your ftp server
///  * Username: username
///  * Password: put your password
///</summary>


namespace TsemachPod
{

    public partial class Form1 : Form
    {
        private Settings settings;

        public Form1()
        {
            InitializeComponent();
            settings = new Settings();

            // Set up the initial directories and file names for the Open and Save file dialogs
            FileInfo fi = new FileInfo(Application.ExecutablePath);
            OpenDialog.InitialDirectory = fi.DirectoryName;
            OpenDialog.FileName = "settings.bin";
            SaveDialog.InitialDirectory = fi.DirectoryName;
            SaveDialog.FileName = "settings.bin";

            this.Read();
        }


        //private XmlDocument xmldoc;
        XmlDocument xmldoc = new XmlDocument();

        //Path to XML Document
        string path = "I:\\Visual Studio Projects\\TsemachPod\\TsemachPod\\bin\\Release\\audio.xml";
        //string path = localXMLPath.Text;
        

        //for edit


        private PodCollection _PodCollection;
        private Pod _Pod;

        private int _PodCollectionCount = 0;
        private int _CurrentRecord = 0;

       

        private void Form1_Load(object sender, EventArgs e)
        {



            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);

            XmlNodeList PodList = xDoc.GetElementsByTagName("item"); //podlist
            XmlNodeList PodChannel = xDoc.GetElementsByTagName("channel"); //podchannel

            _PodCollection = new PodCollection();

            foreach (XmlElement pod in PodList)
            {
                _Pod = new Pod();
                _Pod.Title = pod["title"].InnerText;
                _Pod.Description = pod["description"].InnerText;
                _Pod.PubDate = pod["pubDate"].InnerText;
                _Pod.Enclosure = pod["enclosure"].GetAttribute("url");

                _PodCollection.AddPod(_Pod);
            }



            foreach (XmlElement pod in PodChannel)
            {
                _Pod = new Pod();
                _Pod.TitleMain = pod["title"].InnerText;
                _Pod.Link = pod["link"].InnerText;
                _Pod.DescriptionMain = pod["description"].InnerText;
                _Pod.Language = pod["language"].InnerText;
                _Pod.CopyRight = pod["copyright"].InnerText;
                _Pod.LastBuildDate = pod["lastBuildDate"].InnerText;
                _Pod.Generator = pod["generator"].InnerText;
                _Pod.WebMaster = pod["webMaster"].InnerText;

                _PodCollection.AddPodSettings(_Pod);
            }

            LoadContacts();
            UpdateContact(_CurrentRecord);
            XMLSettings(0); //just get the first channel
            AddTextData(); //Set Sample data to add text
        }

        public void LoadContacts()
        {
            int i = 0;
            int _PodCollectionCount = _PodCollection.Count;

            while (i < _PodCollectionCount)
            {
                lstContacts.Items.Add(_PodCollection.GetPod(i).Title, 0);
                i++;
            }
        }

        private void UpdateContact(int index)
        {
            podTitle.Text = _PodCollection.GetPod(index).Title;
            podDescrption.Text = _PodCollection.GetPod(index).Description;
            podPublish.Text = _PodCollection.GetPod(index).PubDate;
            podEnclosure.Text = _PodCollection.GetPod(index).Enclosure;
        }


        private void XMLSettings(int index)
        {

            titleSettings.Text = _PodCollection.GetPodSettings(index).TitleMain;
            descriptionSettings.Text = _PodCollection.GetPodSettings(index).DescriptionMain;
            linkSettings.Text = _PodCollection.GetPodSettings(index).Link;
            languageSettings.Text = _PodCollection.GetPodSettings(index).Language;
            copyrightSettings.Text = _PodCollection.GetPodSettings(index).CopyRight;
            lastbuildSettings.Text = _PodCollection.GetPodSettings(index).LastBuildDate;
            generatorSettings.Text = _PodCollection.GetPodSettings(index).Generator;
            webMasterSettings.Text = _PodCollection.GetPodSettings(index).WebMaster;
            //xmlurlSettings.Text = _PodCollection.GetPodSettings(index).Enclosure;
        }

        private void AddTextData()
        {
            //<title>Pesach - Sat, 03 Apr 2010</title>
			//<description>Pesach</description>
			//<pubDate>Pesach 03 Apr 2010 07:05:59 GMT</pubDate>
			//<enclosure url="http://www.tsemachadonai.org/mp3/2009-2010/25 2010-04-03 Pesach.mp3"  type="audio/mpeg"/>
            DateTime dt = DateTime.Now; //Getting Date today   //new DateTime(2008, 3, 9, 16, 5, 7, 123); //Set Date
            String dtFormat = String.Format("{0:ddd, dd MMM yyyy}", dt);    // "Sat, 03 Apr 2010"; for example
            String dtTimeFormat = String.Format("{0:dd MMM yyyy HH:mm:ss}", dt);    // "Sat, 03 Apr 2010"; for example

            //Setting sample data for Adding text
            podTitleAdd.Text = "Pesach - " + dtFormat;
            podDescriptionAdd.Text = "Pesach";
            podPublishDateAdd.Text = "Pesach " + dtTimeFormat + " GMT";
            podEnclosureAdd.Text = "http://www.tsemachadonai.org/";

        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            _CurrentRecord = 0;
            UpdateContact(_CurrentRecord);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_CurrentRecord <= 0)
            {
                _CurrentRecord = 0;
                UpdateContact(_CurrentRecord);
            }
            else
            {
                _CurrentRecord--;
                UpdateContact(_CurrentRecord);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            _PodCollectionCount = _PodCollection.Count - 1;

            if (_CurrentRecord >= _PodCollectionCount)
            {
                _CurrentRecord = _PodCollectionCount;
                UpdateContact(_CurrentRecord);
            }
            else
            {
                _CurrentRecord++;
                UpdateContact(_CurrentRecord);
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            _CurrentRecord = _PodCollection.Count - 1;
            UpdateContact(_CurrentRecord);
        }

        private void lstContacts_Click(object sender, EventArgs e)
        {
            int index = lstContacts.SelectedIndices[0];
            _CurrentRecord = index;
            UpdateContact(index);
        }

        private void save_btn_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            xmldoc = new XmlDocument();
            xmldoc.Load(fs);

           
            // the upload button is also used as a cancel button, depending on the state of the FtpProgress thread
            if (this.ftpProgress1.IsBusy)
            {
                this.ftpProgress1.CancelAsync();
                this.save_btn.Text = "Save and Upload";
            }
            else
            {
                // create a new FtpSettings class to store all the paramaters for the FtpProgress thread
                FtpSettings f = new FtpSettings();
                f.Host = this.txtHost.Text;
                f.Username = this.txtUsername.Text;
                f.Password = this.txtPassword.Text;
                f.TargetFolder = this.txtDir.Text;
                f.SourceFile = this.txtUploadFile.Text;
                f.Passive = this.chkPassive.Checked;
                try
                {
                    f.Port = Int32.Parse(this.txtPort.Text);
                }
                catch { }
                this.toolStripProgressBar1.Visible = true;
                this.ftpProgress1.RunWorkerAsync(f);
                this.save_btn.Text = "Cancel";
            }
            

        }

        // Adding a new entry to the catalog

        private void AddPodXML()
        {

            // New XML Element Created
            XmlElement newcatalogentry = xmldoc.CreateElement("item");
            XmlNodeList lstChannel = xmldoc.GetElementsByTagName("channel");
            XmlNodeList lstWebMaster = xmldoc.GetElementsByTagName("webMaster");

            xmldoc.Load(path);

            // New Attribute Created
            //XmlAttribute newcatalogattr = xmldoc.CreateAttribute("ID");

            // Value given for the new attribute
            //newcatalogattr.Value = "005";

            // Attach the attribute to the xml element
            //newcatalogentry.SetAttributeNode(newcatalogattr);

            // First Element - Title - Created
            XmlElement firstelement = xmldoc.CreateElement("title");

            // Value given for the first element
            firstelement.InnerText = podTitleAdd.Text.ToString();

            // Append the newly created element as a child element
            newcatalogentry.AppendChild(firstelement);


            // Second Element - Description - Created
            XmlElement secondelement = xmldoc.CreateElement("description");

            // Value given for the second element
            secondelement.InnerText = podDescriptionAdd.Text.ToString();

            // Append the newly created element as a child element
            newcatalogentry.AppendChild(secondelement);

            // Third Element - Description - Created
            XmlElement thirdelement = xmldoc.CreateElement("pubDate");

            // Value given for the second element
            thirdelement.InnerText = podPublishDateAdd.Text.ToString();

            // Append the newly created element as a child element
            newcatalogentry.AppendChild(thirdelement);

            // Fourth Element - Description - Created
            XmlElement fourthelement = xmldoc.CreateElement("enclosure");

            // Value given for the second element
            //fourthelement.InnerText = podEnclosure.Text.ToString();
            // New Attribute Created
            XmlAttribute newcatalogattr = xmldoc.CreateAttribute("url");
            XmlAttribute newtype = xmldoc.CreateAttribute("type");

            // Value given for the new attribute
            newcatalogattr.Value = podEnclosureAdd.Text.ToString() + txtDir.Text.ToString() + "/" +Path.GetFileName(txtUploadFile.Text.ToString());
            newtype.Value = "audio/mpeg";

            // Attach the attribute to the xml element
            fourthelement.SetAttributeNode(newcatalogattr);
            fourthelement.SetAttributeNode(newtype);

            // Append the newly created element as a child element
            newcatalogentry.AppendChild(fourthelement);

            //y = xmlDoc.getElementsByTagName("book")[3];

            // New XML element inserted into the document
            // xmldoc.DocumentElement.InsertBefore(newcatalogentry, xmldoc.DocumentElement.FirstChild);

            // An instance of FileStream class created
            // First parameter is the path to our XML file - Catalog.xml

            foreach (XmlNode node in lstChannel)
            {
                // Within a video, create a list of its children
                XmlNodeList lstChildren = node.ChildNodes;

                foreach (XmlNode dir in lstChildren)
                {
                    // When you get to a node, look for the element's value
                    // If you find an element whose value is Her Alibi
                    if (dir.InnerText == webMasterSettings.Text.ToString())
                    {
                        FileStream fsxml = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);

                        MessageBox.Show("New Data Added");
                        // Create an element named Actors
                        //XmlElement elmNew = xmlDoc.CreateElement("Actors");
                        //XmlNode elmParent = node.ParentNode;
                        // Add a new element named Actors to it
                        // elmParent.AppendChild(newcatalogentry);
                        // Insert the new node below the Adrian Lyne node Director
                        node.InsertAfter(newcatalogentry, dir);
                        xmldoc.Save(fsxml);
                    }
                }
            }


            // XML Document Saved
            //xmldoc.Save(fsxml);
        }


        private void saveButton_Click(object sender, EventArgs e)
        {
            /*
* <title>Kehilat Tsemach Adonai</title>
* <link>http://www.tsemachadonai.org/mp3/2008-2009/</link>
* <description>Kehilat Tsemach Adonai</description>
* <language>en-us</language>
* <copyright>All rights reserved.</copyright>
* <lastBuildDate>Sat, 31 Oct 2009 07:05:59 GMT</lastBuildDate>
* <generator>www.coralyn.com </generator>		
* <webMaster>coralyn@coralyn.com</webMaster>
*/
            /*
             * titleSettings
            linkSettings
            descriptionSettings
            languageSettings
            copyrightSettings
            lastbuildSettings
            generatorSettings
            webMasterSettings
            xmlurlSettings
            saveButton
             */

            // basically the same as remove node
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            //Start Updating XML
            XmlNode xmlTitleNde = doc.DocumentElement.SelectSingleNode(@"channel/title");
            xmlTitleNde.InnerText = titleSettings.Text.ToString();
            
            XmlNode xmlLinkNde = doc.DocumentElement.SelectSingleNode(@"channel/link");
            xmlLinkNde.InnerText = linkSettings.Text.ToString();

            XmlNode xmlDescriptionNde = doc.DocumentElement.SelectSingleNode(@"channel/description");
            xmlDescriptionNde.InnerText = descriptionSettings.Text.ToString();

            XmlNode xmlLanguageNde = doc.DocumentElement.SelectSingleNode(@"channel/language");
            xmlLanguageNde.InnerText = languageSettings.Text.ToString();

            XmlNode xmlCopyrightNde = doc.DocumentElement.SelectSingleNode(@"channel/copyright");
            xmlCopyrightNde.InnerText = copyrightSettings.Text.ToString();

            XmlNode xmllastBuildNde = doc.DocumentElement.SelectSingleNode(@"channel/lastBuildDate");
            xmllastBuildNde.InnerText = lastbuildSettings.Text.ToString();

            XmlNode xmlGeneratorNde = doc.DocumentElement.SelectSingleNode(@"channel/generator");
            xmlGeneratorNde.InnerText = generatorSettings.Text.ToString();

            XmlNode xmlWebMasterNde = doc.DocumentElement.SelectSingleNode(@"channel/webMaster");
            xmlWebMasterNde.InnerText = webMasterSettings.Text.ToString();
           

            //Saving mode
            doc.Save(path);
            doc.RemoveAll();




        } //save button

        private void updateItem_Click(object sender, EventArgs e)
        {
        
        }

        private void ftpProgress1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Disable some Features

            this.toolStripStatusLabel1.Text = e.UserState.ToString();	// the message will be something like: 45 Kb / 102.12 Mb
            this.toolStripProgressBar1.Value = Math.Min(this.toolStripProgressBar1.Maximum, e.ProgressPercentage);	
        }

        private void ftpProgress1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.ToString(), "FTP error");
            else if (e.Cancelled)
                this.toolStripStatusLabel1.Text = "Upload Canceled";
            else
                this.toolStripStatusLabel1.Text = "Upload Complete";
            //Save data to XML
            AddPodXML();
            this.save_btn.Text = "Save and Upload";
            this.toolStripProgressBar1.Visible = false;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                this.txtUploadFile.Text = this.openFileDialog1.FileName;

                podTitleAdd.Text = "";

                //Get file name with out extension
                string MainString = Path.GetFileNameWithoutExtension(txtUploadFile.Text.ToString());
                string[] Split = MainString.Split(new Char[] { ' ' });
                for (int i = 2; i < Split.Count(); i++)
                {
                    podTitleAdd.Text += Convert.ToString(Split[i]) + " "; //Get the Third Space
                }

                DateTime dt = Convert.ToDateTime(Split[1]); //DateTime.Now; //Getting Date today   //new DateTime(2008, 3, 9, 16, 5, 7, 123); //Set Date
                DateTime dtNow = DateTime.Now; //Getting Date today

                String dtFormat = String.Format("{0:ddd, dd MMM yyyy}", dt);    // "Sat, 03 Apr 2010"; for example
                String dtTimeFormat = String.Format("{0: HH:mm:ss}", dtNow);    // "Sat, 03 Apr 2010"; for example

                podDescriptionAdd.Text = podTitleAdd.Text;
                podPublishDateAdd.Text = podTitleAdd.Text + dtFormat + dtTimeFormat + " GMT";
                //Setting sample data for Adding text
                podTitleAdd.Text = podTitleAdd.Text + " " + dtFormat;
                sermonDate.Text = Convert.ToString(Split[1]);
                sermonNumber.Text = Convert.ToString(Split[0]);
            }
        }

        /// <summary>
        /// Get the first several words from the summary.
        /// </summary>
        public static string FirstWords(string input, int numberWords)
        {
            try
            {
                // Number of words we still want to display.
                int words = numberWords;
                // Loop through entire summary.
                for (int i = 0; i < input.Length; i++)
                {
                    // Increment words on a space.
                    if (input[i] == ' ')
                    {
                        words--;
                    }
                    // If we have no more words to display, return the substring.
                    if (words == 0)
                    {
                        return input.Substring(0, i);
                    }
                }
            }
            catch (Exception)
            {
                // Log the error.
            }
            return string.Empty;
        }

        //Start Registry Thing
        /// <summary>
        /// Save the application settings to the registry
        /// </summary>
        private void Save()
        {
            this.SetSettings();
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\GreenMaggot\\TsemachPod");
            Savior.Save(settings, key);
        }

        /// <summary>
        /// Put all the application information into the Settings class
        /// </summary>
        private void SetSettings()
        {
            // All the values to be saved are stored in the class "Settings" that has been customized
            // for this application
            settings.Username = txtUsername.Text;
            settings.Host = txtHost.Text;
            settings.Password = txtPassword.Text;
            settings.Directory = txtDir.Text;
            settings.XMLPath = localXMLPath.Text;

        }

        /// <summary>
        /// Read all the application settings from the registry and then change the appropriate
        /// elements to display the new settings.
        /// </summary>
        private void Read()
        {
            // Open or create the registry key in which to save application settings 
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\GreenMaggot\\TsemachPod");

            // Read all of the settings from the registry
            Savior.Read(settings, key);

            // Update all the application information with values in the Settings class
            this.GetSettings();
        }

        /// <summary>
        /// Update all the application information with values in the Settings class
        /// </summary>
        private void GetSettings()
        {
            // Here all the application settings are restored by getting the updated values
            // from the "Settings" class
            txtUsername.Text = settings.Username;
            txtHost.Text = settings.Host;
            txtPassword.Text = settings.Password;
            txtDir.Text = settings.Directory;
            localXMLPath.Text = settings.XMLPath;

            
            
        }

        private void browseLocalXMLPath_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() != DialogResult.Cancel)
                this.localXMLPath.Text = this.openFileDialog1.FileName;
        }

        private void SaveFileBtn_Click(object sender, EventArgs e)
        {
            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                this.SetSettings();
                Savior.SaveToFile(settings, SaveDialog.FileName);
            }
        }

        private void ReadFileBtn_Click(object sender, EventArgs e)
        {
            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                settings = (Settings)Savior.ReadFromFile(OpenDialog.FileName);
                this.GetSettings();
            }
        }

        private void saveSettings_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Setting Saved to registry.");
            this.Save();
        }

        private void ReadBtn_Click(object sender, EventArgs e)
        {
            this.Read();
        }




      
    }
        
}
/*Program by: Aaron Jefferson Villanueva */