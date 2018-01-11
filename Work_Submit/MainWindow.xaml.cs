using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Work_Submit
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _Model = new MainModelSum();
            this.DataContext = _Model;
        }
        private MainModelSum _Model;
        private string FileName;
        //打开XMl文档操作
        private void OnOpenFile_Exedcuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                OpenFileDialog aDlg = new OpenFileDialog();
                aDlg.Filter = "文本文件|*.xml;*.txt;*.html;*.log|所有文件|*.*";
                if (aDlg.ShowDialog() != true) return;
                FileName = aDlg.FileName;
                _Model.LoadTargetTextFrom(aDlg.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //判断是否可执行打开XML文档操作
        private void OnOpenFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        //查询Xml文档操作
        private void OnQueryXml_Exedcuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                _Model.GetMatches();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //判断是否可执行查询Xml文档操作
        private void OnQueryXml_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        //解析Xml文档
        private void AnalysisXml_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                _Model.LoadXmlFile(FileName);
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.Message);
            }

        }
        //判断是否解析Xml文档
        private void AnalysisXml_CanExedcuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        //保存到数据库
        private void SaveToSql_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //创建数据库
         //   _Model.Create_DataBase();
            _Model.Save_To_Database();
        }


        //判断是否保存到Xml文档
        private void SaveToSql_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        //根据Id号删除数据
        private void DeleData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _Model.Dele_Data();
        }

        private void DeleData_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
