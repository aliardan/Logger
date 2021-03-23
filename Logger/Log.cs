using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Logger
{
    /// <summary>Реализация интерфейса работы с логом</summary>
    public class Log : ILog
    {
        private string _folder;
        private readonly Dictionary<string, DateTime> _warningUniqueMessages;
        private readonly Dictionary<string, DateTime> _errorUniqueMessages;

        public Log(string folder)
        {
            _folder = folder;
            _warningUniqueMessages = new Dictionary<string, DateTime>();
            _errorUniqueMessages = new Dictionary<string, DateTime>();
        }

        /// <summary>
        /// Критичная ошибка:приложение не может далее функционировать
        /// </summary>
        /// <param name="message">сообщение</param>
        public void Fatal(string message)
        {
            CommonLog(message, "Fatal", "FATAL");
        }

        /// <summary>
        /// Критичная ошибка:приложение не может далее функционировать
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="e">Exception</param>
        public void Fatal(string message, Exception e)
        {
            CommonExceptionLog(message, e, "Fatal", "FATAL");
        }

        /// <summary>
        /// Ошибка в работе приложения: операция расчета завершается, но приложение продолжает работу
        /// </summary>
        /// <param name="message">сообщение</param>
        public void Error(string message)
        {
            CommonLog(message, "Error", "ERROR");
        }

        /// <summary>
        /// Ошибка в работе приложения: операция расчета завершается, но приложение продолжает работу
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="e">Exception</param>
        public void Error(string message, Exception e)
        {
            CommonExceptionLog(message, e, "Error", "ERROR");
        }

        /// <summary>
        /// Ошибка в работе приложения: операция расчета завершается, но приложение продолжает работу
        /// </summary>
        /// <param name="ex">Exception</param>
        public void Error(Exception ex)
        {
            CommonExceptionLog(ex, "Error", "ERROR");
        }

        /// <summary>
        /// Запись уникальных ошибок
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="e">Exception</param>
        public void ErrorUnique(string message, Exception e)
        {
            CommonUniqueException(message, e, "Error_Unique", "ERROR_UNIQUE", _errorUniqueMessages);
        }

        /// <summary>
        /// Предупреждение: на работу приложения не влияет, 
        /// но может сообщать о потенциальных проблемах в расчете
        /// </summary>
        /// <param name="message">сообщение</param>
        public void Warning(string message)
        {
            CommonLog(message, "Warning", "WARNING");
        }

        /// <summary>
        /// Предупреждение: на работу приложения не влияет, 
        /// но может сообщать о потенциальных проблемах в расчете
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="e">Exception</param>
        public void Warning(string message, Exception e)
        {
            CommonExceptionLog(message, e, "Warning", "WARNING");
        }


        /// <summary>
        /// Пишет в лог уникальные в течении дня ошибки 
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <remarks>
        /// Если в течении дня поступают сообщения с одинаковым содержанием,
        ///  то в лог попадут только первые вхождения. 
        /// По прошествию дня уникальность возобновляется.
        /// </remarks>>
        public void WarningUnique(string message)
        {
            CommonUnique(message, "Warning_Unique", "WARNING_UNIQUE", _warningUniqueMessages);
        }

        /// <summary>
        /// Информирование: не влияет на работу приложения,
        /// является инструментом информирования
        /// </summary>
        /// <param name="message">сообщение</param>
        public void Info(string message)
        {
            CommonLog(message, "Info", "INFO");
        }

        /// <summary>
        /// Информирование: не влияет на работу приложения,
        /// является инструментом информирования
        /// </summary>
        /// <param name="message">сообщение</param>
        ///  /// <param name="e">Exception</param>
        public void Info(string message, Exception e)
        {
            CommonExceptionLog(message, e, "Info", "INFO");
        }

        /// <summary>
        /// Информирование: не влияет на работу приложения,
        /// является инструментом информирования
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="args">аргументы</param>
        public void Info(string message, params object[] args)
        {
            var str = string.Format(message, args);
            CommonLog(str, "Info", "INFO");
        }

        /// <summary>
        /// Дебагирование: инструмент для трассировки и отладки
        /// </summary>
        /// <param name="message">сообщение</param>
        public void Debug(string message)
        {
            CommonLog(message, "Debug", "DEBUG");
        }

        /// <summary>
        /// Дебагирование: инструмент для трассировки и отладки
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="e">Exception</param>
        public void Debug(string message, Exception e)
        {
            CommonExceptionLog(message, e, "Debug", "DEBUG");
        }

        /// <summary>
        /// Дебагирование: инструмент для трассировки и отладки
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="args">аргументы</param>
        public void DebugFormat(string message, params object[] args)
        {
            var str = string.Format(message, args);
            CommonLog(str, "Debug", "DEBUG");
        }

        /// <summary>
        /// Запись системных логов информационного характера
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="properties">свойства</param>
        public void SystemInfo(string message, Dictionary<object, object> properties = null)
        {
            List<string> strings = new List<string>() {};
            var str = (properties?.Select(x => x.Key.ToString() + "," + x.Value.ToString())) ?? strings;
            var str2 = String.Join(",", str);
            CommonLog(message + " " + str2, "System_Info", "SYSTEM_INFO");
        }

        /// <summary>
        /// Общий внутренний метод для обработки логов
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="filename">имя файла</param>
        /// <param name="logType">тип лога</param>
        private void CommonLog(string message, string filename, string logType)
        {
            var dateTimeNow = DateTime.Now;
            var dateString = dateTimeNow.ToString("yyyy-MM-dd");
            var currentDirectory = Path.Combine(_folder, dateString);
            var fileName = Path.Combine(currentDirectory, $"{filename}.txt");
            Directory.CreateDirectory(currentDirectory);
            var currentDateTimePrefix = dateTimeNow.ToString("dd.MM.yyyy HH:mm:ss");
            File.AppendAllText(fileName, currentDateTimePrefix + $" ({logType}): " + message + Environment.NewLine);
        }

        /// <summary>
        /// Общий внутренний метод для обработки логов с исключениями
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="ex">Exception</param>
        /// <param name="filename">имя файла</param>
        /// <param name="logType">тип лога</param>
        private void CommonExceptionLog(string message, Exception ex, string filename, string logType)
        {
            CommonLog(message + ' ' + ex.ToString(), filename, logType);
        }

        /// <summary>
        /// Общий внутренний метод для обработки исключений
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="filename">имя файла</param>
        /// <param name="logType">тип лога</param>
        private void CommonExceptionLog(Exception ex, string filename, string logType)
        {
            CommonLog(ex.ToString(), filename, logType);
        }

        /// <summary>
        /// Общий внутренний метод для обработки уникальных логов
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="filename">имя файла</param>
        /// <param name="logType">тип лога</param>
        /// <param name="uniqueMessages">уникальное сообщение</param>
        private void CommonUnique(string message, string filename, string logType, Dictionary<string, DateTime> uniqueMessages)
        {
            var dateTimeNow = DateTime.Now;

            if (uniqueMessages.ContainsKey(message))
            {
                var dateTime = uniqueMessages[message];
                TimeSpan timeSinceLastLog = dateTimeNow - dateTime;
                if (timeSinceLastLog > TimeSpan.FromDays(1))
                {
                    CommonLog(message, filename, logType);
                    uniqueMessages[message] = dateTimeNow;
                }
            }
            else
            {
                uniqueMessages.Add(message, dateTimeNow);
                CommonLog(message, filename, logType);
            }
        }

        /// <summary>
        /// Общий внутренний метод для обработки уникальных логов с исключениями
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="ex">Exception</param>
        /// <param name="filename">имя файла</param>
        /// <param name="logType">тип лога</param>
        /// <param name="uniqueMessages">уникальное сообщение</param>
        private void CommonUniqueException(string message, Exception ex, string filename, string logType, Dictionary<string, DateTime> uniqueMessages)
        {
            CommonUnique(message + " " + ex.ToString(), filename, logType, uniqueMessages);
        }
    }
}