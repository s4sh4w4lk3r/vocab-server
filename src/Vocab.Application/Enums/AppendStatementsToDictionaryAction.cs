namespace Vocab.Application.Enums
{
    /// <summary>
    /// Указывает число выражений, которое может вернуться вместе со словарем.
    /// </summary>
    public enum AppendStatementsAction
    {
        /// <summary>
        /// Не включать выражения при запросе.
        /// </summary>
        NotRequired = 0,

        /// <summary>
        /// Количество выражений. при предпросмотре словаря.
        /// </summary>
        Preview = 15,

        /// <summary>
        /// Количество выражений при открытии.
        /// </summary>
        DictionaryOpened = 150
    }
}
