namespace ChatApp.Configuration.ConfigParameters;

public class TestText
{
    public TestText(string name,string surname,string family)
    {
        Name = name;
        Surname = surname;
        Family = family;
    }

    public string Name { get; }
    public string Family { get; }
    public string Surname { get; }
}