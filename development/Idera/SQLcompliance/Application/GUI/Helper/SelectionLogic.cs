namespace Idera.SQLcompliance.Application.GUI.Helper
{
    /// <summary>
    /// Stores the value of Deselection
    /// </summary>
    public class DeselectionLogic
    {
        public DeselectionLogic(string name, string header, string title, string currentOption, DeselectOptions currentOptionsValue, string othersOption, DeselectOptions othersOptionValue)
        {
            Name = name;
            Header = header;
            Title = title;
            CurrentOption = currentOption;
            CurrentOptionsValue = currentOptionsValue;
            OthersOption = othersOption;
            OthersOptionValue = othersOptionValue;
        }

        public string Name { get; set; }

        public string Header { get; set; }

        public string Title { get; set; }

        public string CurrentOption { get; set; }
        public DeselectOptions CurrentOptionsValue { get; set; }
        public string OthersOption { get; set; }
        public DeselectOptions OthersOptionValue { get; set; }
    }

}
