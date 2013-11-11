using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RaionReminder.Commands
{
    public static class Commands
    {
        public static readonly RoutedUICommand ExitApp = new RoutedUICommand("ExitApp", "Выход из приложения", typeof(Commands));
        public static readonly RoutedUICommand OpenWindow = new RoutedUICommand("OpenWindow", "Открыть окно", typeof(Commands));
        public static readonly RoutedUICommand DenyPublication = new RoutedUICommand("DenyPublication", "Запретить публикацию", typeof(Commands));
        public static readonly RoutedUICommand OpenClient = new RoutedUICommand("OpenClient", "Открыть клиент БСР", typeof(Commands));
        public static readonly RoutedUICommand OpenSite = new RoutedUICommand("OpenSite", "Открыть сайт для загрузки документов", typeof(Commands));
        public static readonly RoutedUICommand ShowHelp = new RoutedUICommand("ShowHelp", "Открыть файл помощи", typeof(Commands));
        public static readonly RoutedUICommand ShowAbout = new RoutedUICommand("ShowAbout", "О программе", typeof(Commands));
        public static readonly RoutedUICommand DeletePassword = new RoutedUICommand("DeletePassword", "Удалить пароль", typeof(Commands));
        public static readonly RoutedUICommand CheckConnection = new RoutedUICommand("CheckConnection","Провеить соединение с базой",typeof(Commands));
        public static readonly RoutedUICommand ShowExcluded = new RoutedUICommand("ShowExcluded", "Показать список исключенных дел", typeof(Commands));
        public static readonly RoutedUICommand RemoveExcluded = new RoutedUICommand("RemoveExcluded", "Удалить из списка исключений", typeof(Commands));
        public static readonly RoutedUICommand ShowList = new RoutedUICommand("ShowList","Показать окно со списком рассмотренных дел",typeof(Commands));
        public static readonly RoutedUICommand ExportExcel = new RoutedUICommand("ExportExcel","Экспортировать список неопубликованных дел в Excel",typeof(Commands));
        public static readonly RoutedUICommand RefreshMain = new RoutedUICommand("RefreshMain","Обновить список неопубликованных дел",typeof(Commands));
    }

   
}
