using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace Work_Submit
{
    class MainModelSum : INotifyPropertyChanged
    {
        //打开XMl文档并进行匹配
        #region
        public string Pattern
        {
            get { return _Pattern; }
            set
            {
                if (_Pattern == value) return; _Pattern = value; OnPropertyChanged(nameof(Pattern));
            }
        }
        private string _Pattern;

        public string ReplacePattern { get { return _ReplacePattern; } set { if (_ReplacePattern == value) return; _ReplacePattern = value; OnPropertyChanged(nameof(ReplacePattern)); } }
        private string _ReplacePattern;

        public string TargetText { get { return _TargetText; } set { if (_TargetText == value) return; _TargetText = value; OnPropertyChanged(nameof(TargetText)); } }
        private string _TargetText;

        public List<MatchView> Matches { get { return _Matches; } private set { if (_Matches == value) return; _Matches = value; OnPropertyChanged(nameof(Matches)); } }
        private List<MatchView> _Matches;

        public string CollectedText { get { return _CollectedText; } private set { if (_CollectedText == value) return; _CollectedText = value; OnPropertyChanged(nameof(CollectedText)); } }
        private string _CollectedText;

        public string ReplaceResult { get { return _ReplaceResult; } private set { if (_ReplaceResult == value) return; _ReplaceResult = value; OnPropertyChanged(nameof(ReplaceResult)); } }
        private string _ReplaceResult;

        private static Encoding[] _Encodings = new Encoding[] { Encoding.Default, Encoding.ASCII, Encoding.UTF8, Encoding.Unicode, Encoding.UTF7, Encoding.UTF32, Encoding.BigEndianUnicode };
        public Encoding[] Encodings { get { return _Encodings; } }

        public Encoding CurrentEncoding { get { return _CurrentEncoding; } set { if (_CurrentEncoding == value) return; _CurrentEncoding = value; OnPropertyChanged(nameof(CurrentEncoding)); } }
        private Encoding _CurrentEncoding = Encoding.Default;

        public bool CanStartMatch
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Pattern) && !string.IsNullOrWhiteSpace(TargetText);
            }
        }

        public bool CanStartReplace
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Pattern) && !string.IsNullOrWhiteSpace(ReplacePattern) && !string.IsNullOrWhiteSpace(TargetText);
            }
        }

        public void LoadTargetTextFrom(string aFileName)
        {
            TargetText = File.ReadAllText(aFileName, CurrentEncoding);
        }

        public class GroupView
        {
            public int Index { get; set; }
            public string Value { get; set; }
        }
        public class MatchView
        {
            public int Index { get; set; }
            public string Value { get; set; }
            public List<GroupView> Groups { get; set; }
        }
        public void GetMatches()
        {
            Regex aRegex = new Regex(Pattern);
            MatchCollection aMatches = aRegex.Matches(TargetText);
            List<MatchView> aMatchViews = new List<MatchView>();
            for (int i = 0; i < aMatches.Count; i++)
            {
                List<GroupView> aGroupViews = new List<GroupView>();
                for (int j = 0; j < aMatches[i].Groups.Count; j++)
                {
                    aGroupViews.Add(new GroupView { Index = j, Value = aMatches[i].Groups[j].Value });
                }
                aMatchViews.Add(new MatchView { Index = i, Value = aMatches[i].Value, Groups = aGroupViews });
            }
            Matches = aMatchViews;

            StringBuilder aStringBuilder = new StringBuilder();
            foreach (Match aMatch in aMatches)
            {
                aStringBuilder.AppendLine(aMatch.Value);
            }
            CollectedText = aStringBuilder.ToString();
        }

        public void Replace()
        {
            Regex aRegex = new Regex(Pattern);
            ReplaceResult = aRegex.Replace(TargetText, ReplacePattern);
        }

        private void OnPropertyChanged(string aPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        //解析XMl文档
        #region
        public XmlDocument CurrentXml { get { return _CurrentXml; } set { if (_CurrentXml == value) return; _CurrentXml = value; OnPropertyChanged(nameof(CurrentXml)); } }
        private XmlDocument _CurrentXml;
        private string _FileName;

        public void LoadXmlFile(string aFileName)
        {
            XmlDocument aXml = new XmlDocument();
            aXml.Load(aFileName);
            CurrentXml = aXml;
            _FileName = aFileName;
        }
        private const string ConfigFileName = "xmllesson01.cfg";
        public void SaveConfig()
        {
            XDocument aXDocument = new XDocument(
                new XElement("Config",
                    new XElement("LastFile", _FileName)
                )
            );
            aXDocument.Save(ConfigFileName);
        }
        public void LoadConfig()
        {
            XDocument aXDocument = XDocument.Load(ConfigFileName);
            _FileName = aXDocument.Root.Element("LastFile").Value;
            LoadXmlFile(_FileName);
        }
        #endregion

        //连接并存入数据库
        #region 
        const string ConnectionString = @"Data Source=(localdb)\Projects;Initial Catalog=Contect;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;";

        public static ContactDataContext DataContext = new ContactDataContext(ConnectionString);

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value) return; _Name = value; OnPropertyChanged(nameof(Name));
            }
        }
        private string _Mobile;
        public string Mobile
        {
            get { return _Mobile; }
            set
            {
                if (_Mobile == value) return; _Mobile = value; OnPropertyChanged(nameof(Mobile));
            }
        }
        private string _Memo;
        public string Memo
        {
            get { return _Memo; }
            set
            {
                if (_Memo == value) return; _Memo = value; OnPropertyChanged(nameof(Memo));
            }
        }

        private int _id;
        public int Id
        {
            get { return _id; }
            set {
                if (_id == value)
                    return;
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public void Create_DataBase()
        {
            try
            {
                // 连接数据库引擎
                using (ContactDataContext aDataContext1 = new ContactDataContext(ConnectionString))
                {
                    if (!aDataContext1.DatabaseExists())
                    {
                        aDataContext1.CreateDatabase();
                        MessageBox.Show("数据库已经创建！");
                    }
                    else
                    {
                        //MessageBox.Show("数据库已经存在！");
                    }
                }
                ContactDataContext aDataContext = new ContactDataContext(ConnectionString);
                DataContext = new ContactDataContext(ConnectionString);
                aDataContext.SubmitChanges();
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.Message);
            }
        }


        public void Save_To_Database()
        {
            if (Name != null)
            {
                Create_DataBase();
                Contact aNewContact = new Contact { Name = Name, Mobile = Mobile, Memo = Memo };
                ContactDataContext aDataContext = new ContactDataContext(ConnectionString);
                aDataContext.Contact.InsertOnSubmit(aNewContact);
                aDataContext.SubmitChanges();
                Contacts = aDataContext.Contact;
            }
            Update();
        }

        public void Dele_Data()
        {
            ContactDataContext aDataContext = new ContactDataContext(ConnectionString);
            Contact aExistContact = (from r in aDataContext.Contact where r.Id ==Id select r).FirstOrDefault();
            if (aExistContact != null)
            {
                aDataContext.Contact.DeleteOnSubmit(aExistContact);
                aDataContext.SubmitChanges();
                Contacts = aDataContext.Contact;
            }
            Update();
        }

        public void Update()
        {
            ContactDataContext aDataContext = new ContactDataContext(ConnectionString);
            aDataContext.SubmitChanges();
        }

        private Table<Contact> _contacts;

        public Table<Contact> Contacts
        {
            get {
                ContactDataContext aDataContext = new ContactDataContext(ConnectionString);
                return aDataContext.Contact; }
            set {
                if (_contacts==value)
                    return;
                _contacts = DataContext.Contact;
                OnPropertyChanged(nameof(Contacts));
            }
        }



        #endregion





    }
}
