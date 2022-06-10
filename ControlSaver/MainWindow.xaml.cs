using System;
using System.Windows;
using System.Windows.Controls;
using ControlSaver.Model;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace ControlSaver
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
   
    public partial class MainWindow : Window
    {
        private int openNum;
        private readonly Saves model = new Saves();
        public MainWindow()
        {

            InitializeComponent();
            foreach(save s in model.Collection)
            {
                IC.Items.Add(CreateBtn(s));
            }
            IC.Items.Add(new Button { Style = (Style)Resources["AddButton"] });

        }

        private void Option(object sender, RoutedEventArgs e)
        {
            IC.Items.RemoveAt(IC.Items.Count - 1);
            IC.Items.Add((Grid)Resources["Shooser1"]);
            
        }

        private void Shoose_Way(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = @"C:\Users\" + Environment.UserName + @"\Desktop\";

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = @"C:\Users\" + Environment.UserName + @"\Desktop\";
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                model.AddSave("Выберите имя", folder);
            }
            Button btn = (Button)sender;
            int index = IC.Items.IndexOf(btn.Parent);
            IC.Items.RemoveAt(index);
            TextBox textBox = new TextBox { Text = "Выберите имя", Style = (Style)Resources["ReName"] };
            IC.Items.Add(textBox);
            textBox.Focus();
            IC.Items.Add(new Button { Style = (Style)Resources["AddButton"] });
        }

        private void SpaceSave(object sender, RoutedEventArgs e)
        {
            model.AddSave("Выберите имя");
            Button btn = (Button)sender;
            int index = IC.Items.IndexOf(btn.Parent);
            IC.Items.RemoveAt(index);
            TextBox textBox = new TextBox { Text = "Выберите имя", Style = (Style)Resources["ReName"] };
            IC.Items.Add( textBox );
            textBox.Focus();
            IC.Items.Add(new Button { Style = (Style)Resources["AddButton"] });
            
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
            }
        }

        private void ReName(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string text = textBox.Text;
            int num = IC.Items.IndexOf(textBox);
            model.Rename(num, text);
            IC.Items.RemoveAt(num);
            IC.Items.Insert(num, CreateBtn(model.Collection[num]));
        }
        //копирование сохранения
        private void CopySave(object sender, RoutedEventArgs e)
        {
            Button btn = IC.Items.GetItemAt(openNum) as Button;
            string text = btn.ContentStringFormat;
            TextBox textBox = new TextBox { Text = text, Style = (Style)Resources["ReName"] };
            IC.Items.Insert(IC.Items.Count-1, textBox);
            textBox.Focus();
            model.CopySave(text, text);
        }
        //удаление сохранения
        private void Delete(object sender, RoutedEventArgs e)
        {
            IC.Items.RemoveAt(openNum);
            model.Del(openNum);
        }
        //вызов переименования из контекстного меню
        private void CmReName(object sender, RoutedEventArgs e)
        {
            Button btn = IC.Items.GetItemAt(openNum) as Button;
            string text = btn.ContentStringFormat;
            IC.Items.RemoveAt(openNum);
            TextBox textBox = new TextBox { Text = text, Style = (Style)Resources["ReName"] };
            IC.Items.Insert(openNum, textBox);
            textBox.Focus();
        }
        //активация сохранения
        private void Activate(object sender, RoutedEventArgs e)
        { 
            if (model.activeSave != -1)
            {
                Button button1 = IC.Items.GetItemAt(model.activeSave) as Button;
                IC.Items.RemoveAt(model.activeSave);
                IC.Items.Insert(model.activeSave, new Button
                {
                    Style = (Style)Resources["Save"],
                    ContentStringFormat = button1.ContentStringFormat
                });
            }
            Button button = sender as Button;
            int index = IC.Items.IndexOf(button);
            IC.Items.RemoveAt(index);
            IC.Items.Insert(index, new Button
            {
                ContentStringFormat = button.ContentStringFormat,
                Style = (Style)Resources["ActiveSave"]
            });
            model.Activate(index);
            Console.WriteLine(index);
           
        }

        //присвоение переменной класса номер элемента, вызвавшего контекстное меню
        private void Button_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Button btn = sender as Button;
            openNum = IC.Items.IndexOf(btn);
            
        }
        //создание кнопки из экземпляра класса save
        private Button CreateBtn(save s)
        {
            Style style;
            if (s.activated)
            {
                style = (Style)Resources["ActiveSave"];
            }
            else
            {
                style = (Style)Resources["Save"];
            }
            return new Button { Style=style, ContentStringFormat=s.Name};
        }
        //копирует сохранение на рабочий стол
        private void CopyToDesktop(object sender, RoutedEventArgs e)
        {
            model.CopyToDesktop(openNum);
        }

        //открывает папку с сохранениями
        private void OpenSaveFolder(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", @"C:\Users\" + Environment.UserName + @"\AppData\Local\Remedy\Control");
        }
    }



    public class MyGrid : Panel
    {
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(MyGrid), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure), ValidateColumns);
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        private static bool ValidateColumns(object o)
        {
            return (int)o >= 1;
        }

        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("RowHeight", typeof(int), typeof(MyGrid), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure), ValidateRows);
        public int RowHeight
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }
        private static bool ValidateRows(object o)
        {
            return (int)o >= 0;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            int rows;
            int count = InternalChildren.Count;
            if (count % Columns == 0)
            {
                rows = count / Columns;
            }
            else
            {
                rows = count / Columns + 1;
            }
            double MaxWidth = 0;
            for (int i = 0; i < rows; i++)
            {
                double thisWidth = 0;
                for (int j = i*Columns; j < (i+1)*Columns & j < count; j++)
                {
                    UIElement element = InternalChildren[j];
                    element.Measure(availableSize);
                    thisWidth += element.DesiredSize.Width;
                }
                if (thisWidth > MaxWidth)
                {
                    MaxWidth = thisWidth;
                }
            }
            if (availableSize.Width == double.PositiveInfinity)
            {
                double Height = 0;
                if (RowHeight == 0)
                {
                    for (int i = 0; i < rows; i++)
                    {
                        double width = 0;
                        for (int j = i * Columns; j < (i + 1) * Columns & j < count; j++)
                        {
                            UIElement element = InternalChildren[j];
                            element.Measure(availableSize);
                            if (element.DesiredSize.Width > width)
                            {
                                width = element.DesiredSize.Width;
                            }
                        }
                        Height += width / 1.62;
                    }
                }
                else
                {
                    Height = RowHeight * rows;
                }
                return new Size (MaxWidth, Height);
            }
            else
            {
                double Height;
                double CellWidth = availableSize.Width / Columns;
                if (RowHeight == 0)
                {
                    Height = CellWidth / 1.62 * rows;
                }
                else
                {
                    Height = RowHeight * rows;
                }
                Size size = new Size (CellWidth, CellWidth / 1.62);
                foreach(UIElement element in InternalChildren)
                {
                    element.Measure(size);
                }
                return new Size(availableSize.Width, Height);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size cellsize = new Size() { Width = finalSize.Width / Columns };
            if (RowHeight != 0)
            {
                cellsize.Height = RowHeight;
            }
            else { cellsize.Height = cellsize.Width / 1.62; }
            int count = InternalChildren.Count;
            Rect cell = new Rect(new Point(0, 0), cellsize);
            for(int i = 0; i < count; i++)
            {
                UIElement child = InternalChildren[i];
                child.Arrange(cell);
                cell.X += cell.Width;
                if (cell.X >= finalSize.Width)
                {
                    cell.X = 0;
                    cell.Y += cell.Height;
                }
            }
            //Size rezult = new Size(finalSize.Width, cell.Y + cell.Height);
            return finalSize;

        }
    }
}
